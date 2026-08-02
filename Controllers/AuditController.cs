using BiometricClockingAPI.DTOs;
using BiometricClockingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiometricClockingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }


    [HttpPut("location/{employeeId:int}")]
    public async Task<IActionResult> UpdateLocation(
        int employeeId,
        [FromBody] UpdateLocationDto request)
    {
        try
        {
            var result = await _auditService
                .UpdateLocationAsync(
                    employeeId,
                    request.Location);

            return Ok(result);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }

    [HttpPost("logout/{employeeId:int}")]
    public async Task<IActionResult> Logout(int employeeId)
    {
        await _auditService.LogoutAsync(employeeId);

        return Ok(new
        {
            message = "Logout recorded successfully."
        });
    }

    [HttpGet("check-session/{employeeId:int}")]
    public async Task<IActionResult> CheckSession(int employeeId)
    {
        try
        {
            var result = await _auditService
                .CheckSessionAsync(employeeId);

            return Ok(result);
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
    }

    [Authorize(Roles = "HR")]
    [HttpGet("history/{employeeId:int}")]
    public async Task<IActionResult> GetHistory(int employeeId)
    {
        var result = await _auditService
            .GetAuditHistoryAsync(employeeId);

        return Ok(result);
    }

    [Authorize(Roles = "HR")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _auditService.GetAllAsync();
        return Ok(result);
    }
}