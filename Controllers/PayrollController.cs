using BiometricClockingAPI.DTOs.Payroll;
using BiometricClockingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiometricClockingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "HR")]
public class PayrollController : ControllerBase
{
    private readonly IPayrollService _payrollService;

    public PayrollController(
        IPayrollService payrollService)
    {
        _payrollService = payrollService;
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> Calculate(
        [FromBody] CalculatePayrollDto request)
    {
        try
        {
            var payroll = await _payrollService
                .CalculateAsync(request);

            return Ok(payroll);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _payrollService.GetAllAsync());
    }

    [HttpGet("{payrollId:int}")]
    public async Task<IActionResult> GetById(int payrollId)
    {
        var payroll = await _payrollService
            .GetByIdAsync(payrollId);

        if (payroll is null)
        {
            return NotFound(new
            {
                message = "Payroll record not found."
            });
        }

        return Ok(payroll);
    }

    [HttpGet("employee/{employeeId:int}")]
    public async Task<IActionResult> GetByEmployee(
        int employeeId)
    {
        return Ok(
            await _payrollService.GetByEmployeeAsync(employeeId)
        );
    }

    [HttpPut("{payrollId:int}/approve")]
    public async Task<IActionResult> Approve(int payrollId)
    {
        try
        {
            var approved = await _payrollService
                .ApproveAsync(payrollId);

            if (!approved)
            {
                return NotFound(new
                {
                    message = "Payroll record not found."
                });
            }

            return Ok(new
            {
                message = "Payroll approved successfully."
            });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
    }

    [HttpPut("{payrollId:int}/mark-paid")]
    public async Task<IActionResult> MarkPaid(int payrollId)
    {
        try
        {
            var paid = await _payrollService
                .MarkPaidAsync(payrollId);

            if (!paid)
            {
                return NotFound(new
                {
                    message = "Payroll record not found."
                });
            }

            return Ok(new
            {
                message = "Payroll marked as paid."
            });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
    }
}