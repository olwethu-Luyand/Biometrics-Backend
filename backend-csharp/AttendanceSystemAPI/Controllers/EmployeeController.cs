using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeeController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EmployeeController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
    {
        var emailExists = await _context.Employees.AnyAsync(e => e.Email == request.Email);
        if (emailExists)
            return Conflict(new { message = "Email already registered." });

        var count = await _context.Employees.CountAsync() + 1;
        var employeeCode = $"EMP-{count:D4}";

        var employee = new Employee
        {
            EmployeeCode = employeeCode,
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            Department = request.Department,
            JobTitle = request.JobTitle,
            EmploymentType = request.EmploymentType,
            HireDate = request.HireDate,
            WorkSchedule = request.WorkSchedule,
            ManagerId = request.ManagerId
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, new
        {
            employee.Id,
            employee.EmployeeCode,
            employee.FullName,
            employee.Email,
            employee.Role
        });
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HR,Manager")]
    public async Task<IActionResult> List(
        [FromQuery] string? search,
        [FromQuery] string? department,
        [FromQuery] EmployeeRole? role,
        [FromQuery] EmploymentType? employmentType,
        [FromQuery] bool isActive = true,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20)
    {
        var query = _context.Employees.Where(e => e.IsActive == isActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e =>
                e.FullName.Contains(search) ||
                e.Email.Contains(search) ||
                e.EmployeeCode.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(department))
            query = query.Where(e => e.Department == department);

        if (role.HasValue)
            query = query.Where(e => e.Role == role.Value);

        if (employmentType.HasValue)
            query = query.Where(e => e.EmploymentType == employmentType.Value);

        var total = await query.CountAsync();

        var employees = await query
            .OrderBy(e => e.FullName)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(e => new
            {
                e.Id,
                e.EmployeeCode,
                e.FullName,
                e.Email,
                e.Phone,
                e.Role,
                e.Department,
                e.JobTitle,
                e.EmploymentType,
                e.HireDate,
                e.WorkSchedule,
                e.ManagerId,
                e.IsActive,
                e.LastLogin
            })
            .ToListAsync();

        return Ok(new { employees, total, page, limit });
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,HR,Manager")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var employee = await _context.Employees
            .Include(e => e.Manager)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
            return NotFound(new { message = "Employee not found." });

        return Ok(new
        {
            employee.Id,
            employee.EmployeeCode,
            employee.FullName,
            employee.Email,
            employee.Phone,
            employee.Role,
            employee.Department,
            employee.JobTitle,
            employee.EmploymentType,
            employee.HireDate,
            employee.WorkSchedule,
            employee.IsActive,
            employee.LastLogin,
            employee.CreatedAt,
            ManagerName = employee.Manager != null ? employee.Manager.FullName : null,
            ManagerCode = employee.Manager != null ? employee.Manager.EmployeeCode : null
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequest request)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);

        if (employee == null)
            return NotFound(new { message = "Employee not found or already archived." });

        if (request.FullName != null) employee.FullName = request.FullName;
        if (request.Phone != null) employee.Phone = request.Phone;
        if (request.Department != null) employee.Department = request.Department;
        if (request.JobTitle != null) employee.JobTitle = request.JobTitle;
        if (request.EmploymentType.HasValue) employee.EmploymentType = request.EmploymentType;
        if (request.HireDate.HasValue) employee.HireDate = request.HireDate;
        if (request.WorkSchedule != null) employee.WorkSchedule = request.WorkSchedule;
        if (request.ManagerId.HasValue) employee.ManagerId = request.ManagerId;
        if (request.Role.HasValue) employee.Role = request.Role.Value;

        employee.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Employee updated.",
            employee = new
            {
                employee.Id,
                employee.EmployeeCode,
                employee.FullName,
                employee.Email,
                employee.Role,
                employee.Department,
                employee.JobTitle
            }
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Archive(Guid id)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);

        if (employee == null)
            return NotFound(new { message = "Employee not found or already archived." });

        employee.IsActive = false;
        employee.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Employee archived.",
            employee = new { employee.Id, employee.EmployeeCode, employee.FullName }
        });
    }
}

public class CreateEmployeeRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Password { get; set; } = string.Empty;
    public EmployeeRole Role { get; set; }
    public string? Department { get; set; }
    public string? JobTitle { get; set; }
    public EmploymentType? EmploymentType { get; set; }
    public DateOnly? HireDate { get; set; }
    public string? WorkSchedule { get; set; }
    public Guid? ManagerId { get; set; }
}

public class UpdateEmployeeRequest
{
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public string? JobTitle { get; set; }
    public EmploymentType? EmploymentType { get; set; }
    public DateOnly? HireDate { get; set; }
    public string? WorkSchedule { get; set; }
    public Guid? ManagerId { get; set; }
    public EmployeeRole? Role { get; set; }
}