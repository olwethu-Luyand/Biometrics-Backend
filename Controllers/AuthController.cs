using System.Security.Cryptography;
using BiometricClockingAPI.Data;
using BiometricClockingAPI.DTOs.Auth;
using BiometricClockingAPI.Models;
using BiometricClockingAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiometricClockingAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IEmailService _emailService;

    public AuthController(
        ApplicationDbContext context,
        IJwtTokenService jwtTokenService,
        IEmailService emailService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _emailService = emailService;
    }

    // POST: api/Auth/login
    // Checks email and password, then sends OTP.
    // It does not return JWT yet.
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto request)
    {
        var normalizedEmail = request.EmailAddress
            .Trim()
            .ToLowerInvariant();

        var employee = await _context.Employees
            .FirstOrDefaultAsync(employee =>
                employee.EmailAddress.ToLower() == normalizedEmail);

        if (employee is null)
        {
            return Unauthorized(new
            {
                message = "Invalid email address or password."
            });
        }

        var passwordIsValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            employee.PasswordHash
        );

        if (!passwordIsValid)
        {
            return Unauthorized(new
            {
                message = "Invalid email address or password."
            });
        }

        // Disable previous unused login OTPs.
        var existingOtps = await _context.OtpCodes
            .Where(otp =>
                otp.EmployeeId == employee.EmployeeId &&
                otp.Purpose == "Login" &&
                !otp.Used)
            .ToListAsync();

        foreach (var existingOtp in existingOtps)
        {
            existingOtp.Used = true;
        }

        var otp = RandomNumberGenerator
            .GetInt32(100000, 1000000)
            .ToString();

        var otpRecord = new OtpCode
        {
            EmployeeId = employee.EmployeeId,
            CodeHash = BCrypt.Net.BCrypt.HashPassword(otp),
            Purpose = "Login",
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            Used = false,
            FailedAttempts = 0,
            CreatedAt = DateTime.UtcNow
        };

        _context.OtpCodes.Add(otpRecord);
        await _context.SaveChangesAsync();

        try
        {
            await _emailService.SendOtpAsync(
                employee.EmailAddress,
                $"{employee.Name} {employee.Surname}",
                otp,
                "Login"
            );
        }
        catch (Exception exception)
        {
            // Prevent an unsent OTP from remaining valid.
            otpRecord.Used = true;
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "The OTP was generated, but the email could not be sent.",
                error = exception.Message
            });
        }

        return Ok(new
        {
            message = "Password verified. An OTP was sent to your email.",
            requiresOtp = true,
            expiresInMinutes = 5
        });
    }

    // POST: api/Auth/verify-login-otp
    // Verifies OTP and only then returns JWT.
    [HttpPost("verify-login-otp")]
    public async Task<IActionResult> VerifyLoginOtp(
        [FromBody] VerifyLoginOtpDto request)
    {
        var normalizedEmail = request.EmailAddress
            .Trim()
            .ToLowerInvariant();

        var employee = await _context.Employees
            .FirstOrDefaultAsync(employee =>
                employee.EmailAddress.ToLower() == normalizedEmail);

        if (employee is null)
        {
            return Unauthorized(new
            {
                message = "Invalid or expired OTP."
            });
        }

        var otpRecord = await _context.OtpCodes
            .Where(otp =>
                otp.EmployeeId == employee.EmployeeId &&
                otp.Purpose == "Login" &&
                !otp.Used)
            .OrderByDescending(otp => otp.CreatedAt)
            .FirstOrDefaultAsync();

        if (otpRecord is null ||
            otpRecord.ExpiresAt <= DateTime.UtcNow)
        {
            return Unauthorized(new
            {
                message = "Invalid or expired OTP."
            });
        }

        if (otpRecord.FailedAttempts >= 5)
        {
            otpRecord.Used = true;
            await _context.SaveChangesAsync();

            return Unauthorized(new
            {
                message = "Too many incorrect attempts. Request another OTP."
            });
        }

        var otpIsValid = BCrypt.Net.BCrypt.Verify(
            request.Otp,
            otpRecord.CodeHash
        );

        if (!otpIsValid)
        {
            otpRecord.FailedAttempts++;
            await _context.SaveChangesAsync();

            return Unauthorized(new
            {
                message = "Invalid or expired OTP."
            });
        }

        otpRecord.Used = true;
        await _context.SaveChangesAsync();

        var jwtToken = _jwtTokenService.GenerateToken(employee);

        return Ok(new
        {
            message = "Login successful.",
            token = jwtToken,
            expiresInMinutes = 60,
            employee = new
            {
                employee.EmployeeId,
                employee.Name,
                employee.Surname,
                employee.EmailAddress,
                employee.Role
            }
        });
    }

    // POST: api/Auth/forgot-password
    // Sends a separate password-reset OTP.
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordDto request)
    {
        var normalizedEmail = request.EmailAddress
            .Trim()
            .ToLowerInvariant();

        var employee = await _context.Employees
            .FirstOrDefaultAsync(employee =>
                employee.EmailAddress.ToLower() == normalizedEmail);

        // Avoid exposing whether the email exists.
        if (employee is null)
        {
            return Ok(new
            {
                message = "If the email exists, a reset OTP has been sent."
            });
        }

        var existingOtps = await _context.OtpCodes
            .Where(otp =>
                otp.EmployeeId == employee.EmployeeId &&
                otp.Purpose == "PasswordReset" &&
                !otp.Used)
            .ToListAsync();

        foreach (var existingOtp in existingOtps)
        {
            existingOtp.Used = true;
        }

        var otp = RandomNumberGenerator
            .GetInt32(100000, 1000000)
            .ToString();

        var otpRecord = new OtpCode
        {
            EmployeeId = employee.EmployeeId,
            CodeHash = BCrypt.Net.BCrypt.HashPassword(otp),
            Purpose = "PasswordReset",
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            Used = false,
            FailedAttempts = 0,
            CreatedAt = DateTime.UtcNow
        };

        _context.OtpCodes.Add(otpRecord);
        await _context.SaveChangesAsync();

        try
        {
            await _emailService.SendOtpAsync(
                employee.EmailAddress,
                $"{employee.Name} {employee.Surname}",
                otp,
                "PasswordReset"
            );
        }
        catch (Exception exception)
        {
            otpRecord.Used = true;
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "The reset OTP could not be sent.",
                error = exception.Message
            });
        }

        return Ok(new
        {
            message = "If the email exists, a reset OTP has been sent.",
            expiresInMinutes = 5
        });
    }

    // POST: api/Auth/reset-password
    // Verifies reset OTP and changes the password.
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordDto request)
    {
        var normalizedEmail = request.EmailAddress
            .Trim()
            .ToLowerInvariant();

        var employee = await _context.Employees
            .FirstOrDefaultAsync(employee =>
                employee.EmailAddress.ToLower() == normalizedEmail);

        if (employee is null)
        {
            return BadRequest(new
            {
                message = "Invalid or expired OTP."
            });
        }

        var otpRecord = await _context.OtpCodes
            .Where(otp =>
                otp.EmployeeId == employee.EmployeeId &&
                otp.Purpose == "PasswordReset" &&
                !otp.Used)
            .OrderByDescending(otp => otp.CreatedAt)
            .FirstOrDefaultAsync();

        if (otpRecord is null ||
            otpRecord.ExpiresAt <= DateTime.UtcNow ||
            otpRecord.FailedAttempts >= 5)
        {
            return BadRequest(new
            {
                message = "Invalid or expired OTP."
            });
        }

        var otpIsValid = BCrypt.Net.BCrypt.Verify(
            request.Otp,
            otpRecord.CodeHash
        );

        if (!otpIsValid)
        {
            otpRecord.FailedAttempts++;
            await _context.SaveChangesAsync();

            return BadRequest(new
            {
                message = "Invalid or expired OTP."
            });
        }

        employee.PasswordHash =
            BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        otpRecord.Used = true;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Password reset successfully."
        });
    }
}