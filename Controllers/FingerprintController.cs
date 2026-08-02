using BiometricClockingAPI.DTOs.Biometric;
using BiometricClockingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiometricClockingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FingerprintController : ControllerBase
{
    private readonly IFingerprintService _fingerprintService;

    public FingerprintController(
        IFingerprintService fingerprintService)
    {
        _fingerprintService = fingerprintService;
    }

    // POST: api/Fingerprint/enroll/5
    [Authorize(Roles = "HR")]
    [HttpPost("enroll/{employeeId:int}")]
    public async Task<IActionResult> EnrollFingerprint(
        int employeeId,
        [FromBody] EnrollFingerprintDto request)
    {
        var employee = await _fingerprintService.EnrollAsync(
            employeeId,
            request.FingerprintTemplate,
            request.ScannerDeviceId
        );

        if (employee is null)
        {
            return NotFound(new
            {
                message = "Employee not found."
            });
        }

        return Ok(new
        {
            message = "Fingerprint enrolled successfully.",
            employee = new
            {
                employee.EmployeeId,
                employee.Name,
                employee.Surname,
                employee.ScannerDeviceId,
                employee.FingerprintEnrolled,
                employee.FingerprintEnrolledAt
            }
        });
    }

    // POST: api/Fingerprint/verify
    [AllowAnonymous]
    [HttpPost("verify")]
    public async Task<IActionResult> VerifyFingerprint(
        [FromBody] VerifyFingerprintDto request)
    {
        var employee = await _fingerprintService.VerifyAsync(
            request.FingerprintTemplate
        );

        if (employee is null)
        {
            return Unauthorized(new
            {
                matched = false,
                message = "Fingerprint was not recognized."
            });
        }

        return Ok(new
        {
            matched = true,
            message = "Fingerprint verified successfully.",
            employee = new
            {
                employee.EmployeeId,
                employee.Name,
                employee.Surname,
                employee.Role
            }
        });
    }

    // DELETE: api/Fingerprint/5
    [Authorize(Roles = "HR")]
    [HttpDelete("{employeeId:int}")]
    public async Task<IActionResult> RemoveFingerprint(int employeeId)
    {
        var removed = await _fingerprintService.RemoveAsync(employeeId);

        if (!removed)
        {
            return NotFound(new
            {
                message = "Employee not found."
            });
        }

        return Ok(new
        {
            message = "Fingerprint removed successfully."
        });
    }
}