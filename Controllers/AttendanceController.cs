using BiometricClockingAPI.DTOs.Biometric;
using BiometricClockingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiometricClockingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;
    private readonly IFingerprintService _fingerprintService;

    
    public AttendanceController(
        IAttendanceService attendanceService,
        IFingerprintService fingerprintService)
    {
        _attendanceService = attendanceService;
        _fingerprintService = fingerprintService;
    }

    // POST: api/Attendance/clock
    [AllowAnonymous]
    [HttpPost("clock")]
    public async Task<IActionResult> Clock(
        [FromBody] VerifyFingerprintDto request)
    {
        var employee = await _fingerprintService.VerifyAsync(
            request.FingerprintTemplate);

        if (employee == null)
        {
            return Unauthorized(new
            {
                matched = false,
                message = "Fingerprint not recognized."
            });
        }

        try
        {
            var attendance = await _attendanceService.ClockAsync(
                employee.EmployeeId);

            var action = attendance.ClockOutTime == null
                ? "Clock In"
                : "Clock Out";

            return Ok(new
            {
                message = $"{action} successful.",
                employee = new
                {
                    employee.EmployeeId,
                    employee.Name,
                    employee.Surname,
                    employee.Role
                },
                attendance = new
                {
                    attendance.AttendanceId,
                    attendance.AttendanceDate,
                    attendance.ClockInTime,
                    attendance.ClockOutTime,
                    attendance.TotalWorkedHours,
                    attendance.OvertimeHours,
                    attendance.Status
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    // GET: api/Attendance/today/2
    [HttpGet("today/{employeeId:int}")]
    public async Task<IActionResult> GetToday(int employeeId)
    {
        var attendance = await _attendanceService.GetTodayAsync(employeeId);

        if (attendance == null)
        {
            return NotFound(new
            {
                message = "No attendance record found for today."
            });
        }

        return Ok(new
        {
            attendance.AttendanceId,
            attendance.EmployeeId,
            attendance.AttendanceDate,
            attendance.ClockInTime,
            attendance.ClockOutTime,
            attendance.TotalWorkedHours,
            attendance.OvertimeHours,
            attendance.Status,
            attendance.CreatedAt,
            attendance.UpdatedAt,

            employee = attendance.Employee == null
                ? null
                : new
                {
                    attendance.Employee.EmployeeId,
                    attendance.Employee.Name,
                    attendance.Employee.Surname,
                    attendance.Employee.EmailAddress,
                    attendance.Employee.Role
                }
        });
    }

    // GET: api/Attendance/history/2
    [Authorize(Roles = "HR")]
    [HttpGet("history/{employeeId:int}")]
    public async Task<IActionResult> GetHistory(int employeeId)
    {
        var history = await _attendanceService.GetHistoryAsync(employeeId);

        return Ok(history.Select(attendance => new
        {
            attendance.AttendanceId,
            attendance.AttendanceDate,
            attendance.ClockInTime,
            attendance.ClockOutTime,
            attendance.TotalWorkedHours,
            attendance.OvertimeHours,
            attendance.Status,

            employee = attendance.Employee == null
                ? null
                : new
                {
                    attendance.Employee.EmployeeId,
                    attendance.Employee.Name,
                    attendance.Employee.Surname
                }
        }));
    }

    [Authorize(Roles = "HR")]
    [HttpGet]
    public async Task<IActionResult> GetAllAttendance()
    {
        var attendance = await _attendanceService.GetAllAsync();

        return Ok(attendance.Select(a => new
        {
            a.AttendanceId,
            a.AttendanceDate,
            a.ClockInTime,
            a.ClockOutTime,
            a.TotalWorkedHours,
            a.OvertimeHours,
            a.Status,

            Employee = new
            {
                a.Employee.EmployeeId,
                a.Employee.Name,
                a.Employee.Surname,
                a.Employee.EmailAddress,
                a.Employee.Role
            }
        }));
    }

    [Authorize(Roles = "HR")]
    [HttpGet("today")]
    public async Task<IActionResult> GetTodayAttendance()
    {
        var records = await _attendanceService.GetTodayAllAsync();

        var response = records.Select(attendance => new
        {
            attendance.AttendanceId,
            attendance.EmployeeId,
            attendance.AttendanceDate,
            attendance.ClockInTime,
            attendance.ClockOutTime,
            attendance.TotalWorkedHours,
            attendance.OvertimeHours,
            attendance.Status,
            attendance.CreatedAt,
            attendance.UpdatedAt,

            employee = attendance.Employee is null
                ? null
                : new
                {
                    attendance.Employee.EmployeeId,
                    attendance.Employee.Name,
                    attendance.Employee.Surname,
                    attendance.Employee.EmailAddress,
                    attendance.Employee.Role
                }
        });

        return Ok(response);
    }

    [Authorize(Roles = "HR")]
    [HttpGet("history")]
    public async Task<IActionResult> GetAttendanceHistory()
    {
        var records = await _attendanceService.GetAllHistoryAsync();

        var response = records.Select(attendance => new
        {
            attendance.AttendanceId,
            attendance.EmployeeId,
            attendance.AttendanceDate,
            attendance.ClockInTime,
            attendance.ClockOutTime,
            attendance.TotalWorkedHours,
            attendance.OvertimeHours,
            attendance.Status,
            attendance.CreatedAt,
            attendance.UpdatedAt,

            employee = attendance.Employee is null
                ? null
                : new
                {
                    attendance.Employee.EmployeeId,
                    attendance.Employee.Name,
                    attendance.Employee.Surname,
                    attendance.Employee.EmailAddress,
                    attendance.Employee.Role
                }
        });

        return Ok(response);
    }

    [Authorize(Roles = "HR")]
    [HttpPost("mark-absent")]
    public async Task<IActionResult> MarkAbsentEmployees(
        [FromQuery] DateOnly attendanceDate)
    {
        if (attendanceDate > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return BadRequest(new
            {
                message = "You cannot mark attendance for a future date."
            });
        }

        var absentCount = await _attendanceService
            .MarkAbsentEmployeesAsync(attendanceDate);

        return Ok(new
        {
            message = absentCount == 0
                ? "No employees were marked absent."
                : $"{absentCount} employee(s) marked absent.",
            attendanceDate,
            absentCount
        });
    }
}
