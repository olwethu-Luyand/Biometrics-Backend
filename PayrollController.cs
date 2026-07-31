using Microsoft.AspNetCore.Mvc;
using PayollModule.DTOs;
using PayollModule.Services;

namespace PayollModule.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayrollController : ControllerBase
{
    private readonly IPayrollService _payrollService;

    public PayrollController(IPayrollService payrollService)
    {
        _payrollService = payrollService;
    }

    [HttpPost("calculate")]
    public IActionResult Calculate([FromBody] CalculatePayrollDto dto)
    {
        var payroll = _payrollService.CalculatePayroll(dto);
        return Ok(new
        {
            payroll.EmployeeId,
            payroll.PayStart,
            payroll.PayEnd,
            payroll.HoursWorked,
            payroll.OvertimeHours,
            payroll.GrossPay,
            payroll.Deductions,
            payroll.NetPay,
            payroll.PaymentStatus
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CalculatePayrollDto dto, CancellationToken cancellationToken)
    {
        var result = await _payrollService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.PayrollId }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _payrollService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _payrollService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("employee/{employeeId:int}")]
    public async Task<IActionResult> GetByEmployee(int employeeId, CancellationToken cancellationToken)
    {
        var result = await _payrollService.GetByEmployeeIdAsync(employeeId, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, CancellationToken cancellationToken)
    {
        var result = await _payrollService.ApproveAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _payrollService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
