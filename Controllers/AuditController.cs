using AuditModule.DTOs;
using AuditModule.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuditModule.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuditResponseDto>> Login([FromBody] CreateAuditDto dto)
    {
        var result = await _auditService.LoginAsync(dto);
        return Ok(result);
    }

    [HttpPut("location/{employeeId:guid}")]
    public async Task<ActionResult<AuditResponseDto>> UpdateLocation(Guid employeeId, [FromBody] UpdateLocationDto dto)
    {
        var result = await _auditService.UpdateLocationAsync(employeeId, dto.Location);
        return Ok(result);
    }

    [HttpPost("logout/{employeeId:guid}")]
    public async Task<IActionResult> Logout(Guid employeeId)
    {
        await _auditService.LogoutAsync(employeeId);
        return Ok(new { message = "Logout recorded successfully." });
    }

    [HttpGet("check-session/{employeeId:guid}")]
    public async Task<ActionResult<AuditResponseDto>> CheckSession(Guid employeeId)
    {
        var result = await _auditService.CheckSessionAsync(employeeId);
        return Ok(result);
    }

    [HttpGet("history/{employeeId:guid}")]
    public async Task<ActionResult<IEnumerable<AuditResponseDto>>> GetHistory(Guid employeeId)
    {
        var result = await _auditService.GetAuditHistoryAsync(employeeId);
        return Ok(result);
    }
}
