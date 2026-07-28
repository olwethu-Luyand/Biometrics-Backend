using BiometricClockingAPI.Data;
using BiometricClockingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiometricClockingAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    [HttpPost("login")]
    public IActionResult Login()
    {
        return Ok(new { message = "Authentication endpoint ready" });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmailAddress == request.Email);
        if (employee == null)
        {
            return NotFound(new { message = "Employee with that email not found" });
        }

        var token = Guid.NewGuid().ToString("N");

        var prt = new PasswordResetToken
        {
            Token = token,
            EmployeeId = employee.EmployeeId,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            Used = false
        };

        _context.PasswordResetTokens.Add(prt);
        await _context.SaveChangesAsync();

        // In production, send this token via email. For now return it in the response for testing.
        return Ok(new { message = "Password reset token created", token });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var prt = await _context.PasswordResetTokens.Include(p => p.Employee).FirstOrDefaultAsync(p => p.Token == request.Token);
        if (prt == null || prt.Used || prt.ExpiresAt < DateTime.UtcNow)
        {
            return BadRequest(new { message = "Invalid or expired token" });
        }

        prt.Used = true;
        prt.Employee.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        await _context.SaveChangesAsync();
        return Ok(new { message = "Password reset successfully" });
    }
}
