using BiometricClockingAPI.Data;
using BiometricClockingAPI.DTOs.Employee;
using BiometricClockingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiometricClockingAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EmployeeController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/Employee
    
    [HttpPost]
    public async Task<IActionResult> AddEmployee(
        [FromBody] RegisterEmployeeDto request)
    {
        var emailExists = await _context.Employees
            .AnyAsync(employee =>
                employee.EmailAddress == request.EmailAddress);

        if (emailExists)
        {
            return Conflict(new
            {
                message = "An employee with this email address already exists."
            });
        }

        var fingerprintProvided =
            !string.IsNullOrWhiteSpace(request.FingerprintTemplate);

        var employee = new Employee
        {
            Name = request.Name.Trim(),
            Surname = request.Surname.Trim(),
            Role = request.Role.Trim(),
            EmailAddress = request.EmailAddress.Trim().ToLowerInvariant(),

            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                request.Password
            ),

            FingerprintTemplate = request.FingerprintTemplate,
            ScannerDeviceId = request.ScannerDeviceId,

            FingerprintEnrolled = fingerprintProvided,

            FingerprintEnrolledAt = fingerprintProvided
                ? DateTime.UtcNow
                : null
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(SearchEmployee),
            new { id = employee.EmployeeId },
            ToEmployeeResponse(employee)
        );
    }

    // GET: api/Employee
    [HttpGet]
    public async Task<IActionResult> GetEmployees()
    {
        var employees = await _context.Employees
            .AsNoTracking()
            .Select(employee => new
            {
                employee.EmployeeId,
                employee.Name,
                employee.Surname,
                employee.Role,
                employee.EmailAddress,
                employee.ScannerDeviceId,
                employee.FingerprintEnrolled,
                employee.FingerprintEnrolledAt
            })
            .ToListAsync();

        return Ok(employees);
    }

    // GET: api/Employee/1
    [Authorize(Roles = "HR")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> SearchEmployee(int id)
    {
        var employee = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(employee =>
                employee.EmployeeId == id);

        if (employee is null)
        {
            return NotFound(new
            {
                message = "Employee not found."
            });
        }

        return Ok(ToEmployeeResponse(employee));
    }

    // PUT: api/Employee/1
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> EditEmployee(
        int id,
        [FromBody] UpdateEmployeeDto request)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee is null)
        {
            return NotFound(new
            {
                message = "Employee not found."
            });
        }

        var normalizedEmail =
            request.EmailAddress.Trim().ToLowerInvariant();

        var emailExists = await _context.Employees
            .AnyAsync(existingEmployee =>
                existingEmployee.EmployeeId != id &&
                existingEmployee.EmailAddress == normalizedEmail);

        if (emailExists)
        {
            return Conflict(new
            {
                message = "Another employee already uses this email address."
            });
        }

        employee.Name = request.Name.Trim();
        employee.Surname = request.Surname.Trim();
        employee.Role = request.Role.Trim();
        employee.EmailAddress = normalizedEmail;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            employee.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(request.Password);
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Employee updated successfully.",
            employee = ToEmployeeResponse(employee)
        });
    }

    // POST: api/Employee/1/fingerprint
    
    [HttpPost("{id:int}/fingerprint")]
    public async Task<IActionResult> RegisterFingerprint(
        int id,
        [FromBody] RegisterFingerprintDto request)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee is null)
        {
            return NotFound(new
            {
                message = "Employee not found."
            });
        }

        employee.FingerprintTemplate =
            request.FingerprintTemplate;

        employee.ScannerDeviceId =
            request.ScannerDeviceId;

        employee.FingerprintEnrolled = true;
        employee.FingerprintEnrolledAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Fingerprint registered successfully.",
            employee = ToEmployeeResponse(employee)
        });
    }

    // DELETE: api/Employee/1/fingerprint
    [Authorize(Roles = "HR")]
    [HttpDelete("{id:int}/fingerprint")]
    public async Task<IActionResult> RemoveFingerprint(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee is null)
        {
            return NotFound(new
            {
                message = "Employee not found."
            });
        }

        employee.FingerprintTemplate = null;
        employee.ScannerDeviceId = null;
        employee.FingerprintEnrolled = false;
        employee.FingerprintEnrolledAt = null;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Fingerprint removed successfully."
        });
    }

    // DELETE: api/Employee/1
    [Authorize(Roles = "HR")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee is null)
        {
            return NotFound(new
            {
                message = "Employee not found."
            });
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Employee deleted successfully."
        });
    }

    private static object ToEmployeeResponse(Employee employee)
    {
        return new
        {
            employee.EmployeeId,
            employee.Name,
            employee.Surname,
            employee.Role,
            employee.EmailAddress,
            employee.ScannerDeviceId,
            employee.FingerprintEnrolled,
            employee.FingerprintEnrolledAt
        };
    }
}