using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AttendanceController(ApplicationDbContext context)
    {
        _context = context;
    }

    private Guid CurrentEmployeeId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("clock-in")]
    public async Task<IActionResult> ClockIn([FromBody] ClockRequest request)
    {
        var employeeId = CurrentEmployeeId;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var existing = await _context.AttendanceRecords
            .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == today);

        if (existing != null && existing.ClockIn != null && existing.ClockOut == null)
            return Conflict(new { message = "Already clocked in. Clock out before clocking in again." });

        if (existing != null && existing.ClockIn != null && existing.ClockOut != null)
            return Conflict(new { message = "Already completed attendance for today." });

        var now = DateTime.UtcNow;
        var scheduledStartHour = 9;
        var status = (now.Hour > scheduledStartHour ||
                      (now.Hour == scheduledStartHour && now.Minute > 15))
            ? AttendanceStatus.Late
            : AttendanceStatus.Present;

        if (existing == null)
        {
            existing = new Attendance
            {
                EmployeeId = employeeId,
                Date = today,
                ClockIn = now,
                ClockInMethod = request.Method,
                Status = status
            };
            _context.AttendanceRecords.Add(existing);
        }
        else
        {
            existing.ClockIn = now;
            existing.ClockInMethod = request.Method;
            existing.Status = status;
        }

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = existing.Id }, existing);
    }

    [HttpPost("clock-out")]
    public async Task<IActionResult> ClockOut([FromBody] ClockRequest request)
    {
        var employeeId = CurrentEmployeeId;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var existing = await _context.AttendanceRecords
            .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == today);

        if (existing == null || existing.ClockIn == null)
            return BadRequest(new { message = "You must clock in before clocking out." });

        if (existing.ClockOut != null)
            return Conflict(new { message = "Already clocked out for today." });

        var now = DateTime.UtcNow;
        existing.ClockOut = now;
        existing.ClockOutMethod = request.Method;
        existing.HoursWorked = (decimal)(now - existing.ClockIn.Value).TotalHours;

        await _context.SaveChangesAsync();

        return Ok(existing);
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetTodayStatus()
    {
        var employeeId = CurrentEmployeeId;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var record = await _context.AttendanceRecords
            .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == today);

        return Ok(record);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var record = await _context.AttendanceRecords
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (record == null)
            return NotFound(new { message = "Attendance record not found." });

        return Ok(record);
    }

    [HttpGet("employee/{employeeId}")]
    [Authorize(Roles = "Admin,HR,Manager")]
    public async Task<IActionResult> GetByEmployee(
        Guid employeeId,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 30)
    {
        var employeeExists = await _context.Employees.AnyAsync(e => e.Id == employeeId);
        if (!employeeExists)
            return NotFound(new { message = "Employee not found." });

        var query = _context.AttendanceRecords
            .Where(a => a.EmployeeId == employeeId);

        if (startDate.HasValue)
            query = query.Where(a => a.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(a => a.Date <= endDate.Value);

        var total = await query.CountAsync();

        var records = await query
            .OrderByDescending(a => a.Date)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return Ok(new { attendance = records, total, page, limit });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AttendanceUpdateRequest request)
    {
        var record = await _context.AttendanceRecords.FindAsync(id);
        if (record == null)
            return NotFound(new { message = "Attendance record not found." });

        if (request.ClockIn.HasValue) record.ClockIn = request.ClockIn;
        if (request.ClockOut.HasValue) record.ClockOut = request.ClockOut;
        if (request.Status.HasValue) record.Status = request.Status;
        if (request.Notes != null) record.Notes = request.Notes;

        if (record.ClockIn.HasValue && record.ClockOut.HasValue)
        {
            record.HoursWorked = (decimal)(record.ClockOut.Value - record.ClockIn.Value).TotalHours;
        }

        await _context.SaveChangesAsync();
        return Ok(record);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var record = await _context.AttendanceRecords.FindAsync(id);
        if (record == null)
            return NotFound(new { message = "Attendance record not found." });

        _context.AttendanceRecords.Remove(record);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Attendance record deleted." });
    }

    // ----- READ: Full attendance overview (Admin/HR/Manager) -----
    [HttpGet]
    [Authorize(Roles = "Admin,HR,Manager")]
    public async Task<IActionResult> List(
        [FromQuery] string? search,
        [FromQuery] DateOnly? date,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20)
    {
        var query = _context.AttendanceRecords
            .Include(a => a.Employee)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(a =>
                a.Employee.EmployeeCode.Contains(search) ||
                a.Employee.FullName.Contains(search));
        }

        if (date.HasValue)
            query = query.Where(a => a.Date == date.Value);

        var total = await query.CountAsync();

        var records = await query
            .OrderByDescending(a => a.Date)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(a => new
            {
                EmployeeId = a.Employee.EmployeeCode,
                Date = a.Date,
                CheckIn = a.ClockIn,
                CheckOut = a.ClockOut,
                OvertimeHrs = a.HoursWorked.HasValue && a.HoursWorked.Value > 8
                    ? Math.Round(a.HoursWorked.Value - 8, 2)
                    : 0,
                Status = a.Status
            })
            .ToListAsync();

        return Ok(new { attendance = records, total, page, limit });
    }

    // ----- READ: Attendance by exact Employee Code (e.g. "00003333") -----
    [HttpGet("by-code/{employeeCode}")]

    [Authorize(Roles = "Admin,HR,Manager")]
    public async Task<IActionResult> GetByEmployeeCode(
        string employeeCode,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 30)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode);

        if (employee == null)
            return NotFound(new { message = "Employee not found." });

        var query = _context.AttendanceRecords
            .Where(a => a.EmployeeId == employee.Id);

        if (startDate.HasValue)
            query = query.Where(a => a.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(a => a.Date <= endDate.Value);

        var total = await query.CountAsync();

        var records = await query
            .OrderByDescending(a => a.Date)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(a => new
            {
                EmployeeId = employee.EmployeeCode,
                EmployeeName = employee.FullName,
                Date = a.Date,
                CheckIn = a.ClockIn,
                CheckOut = a.ClockOut,
                OvertimeHrs = a.HoursWorked.HasValue && a.HoursWorked.Value > 8
                    ? Math.Round(a.HoursWorked.Value - 8, 2)
                    : 0,
                Status = a.Status
            })
            .ToListAsync();

        return Ok(new { attendance = records, total, page, limit });
    }
}

public class ClockRequest
{
    public ClockMethod Method { get; set; } = ClockMethod.Web;
}

public class AttendanceUpdateRequest
{
    public DateTime? ClockIn { get; set; }
    public DateTime? ClockOut { get; set; }
    public AttendanceStatus? Status { get; set; }
    public string? Notes { get; set; }
}