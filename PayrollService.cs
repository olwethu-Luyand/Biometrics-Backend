using PayollModule.DTOs;
using PayollModule.Models;
using PayollModule.Repositories;

namespace PayollModule.Services;

public class PayrollService : IPayrollService
{
    private const decimal OvertimeMultiplier = 1.5m;
    private readonly IPayrollRepository _repository;

    public PayrollService(IPayrollRepository repository)
    {
        _repository = repository;
    }

    public Payroll CalculatePayroll(CalculatePayrollDto dto)
    {
        const decimal hourlyRate = 30m;
        var grossPay = (dto.HoursWorked * hourlyRate) + (dto.OvertimeHours * hourlyRate * OvertimeMultiplier);
        var netPay = grossPay - dto.Deductions;

        return new Payroll
        {
            EmployeeId = dto.EmployeeId,
            PayStart = dto.PayStart,
            PayEnd = dto.PayEnd,
            HoursWorked = dto.HoursWorked,
            OvertimeHours = dto.OvertimeHours,
            HourlyRate = hourlyRate,
            GrossPay = grossPay,
            Deductions = dto.Deductions,
            NetPay = netPay,
            PaymentStatus = "Pending"
        };
    }

    public async Task<IReadOnlyList<PayrollDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var payrolls = await _repository.GetAllAsync(cancellationToken);
        return payrolls.Select(MapToDto).ToList();
    }

    public async Task<PayrollDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var payroll = await _repository.GetByIdAsync(id, cancellationToken);
        return payroll is null ? null : MapToDto(payroll);
    }

    public async Task<IReadOnlyList<PayrollDto>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        var payrolls = await _repository.GetByEmployeeIdAsync(employeeId, cancellationToken);
        return payrolls.Select(MapToDto).ToList();
    }

    public async Task<PayrollDto> CreateAsync(CalculatePayrollDto dto, CancellationToken cancellationToken = default)
    {
        var payroll = CalculatePayroll(dto);
        await _repository.AddAsync(payroll, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return MapToDto(payroll);
    }

    public async Task<PayrollDto?> ApproveAsync(int id, CancellationToken cancellationToken = default)
    {
        var payroll = await _repository.GetByIdAsync(id, cancellationToken);
        if (payroll is null)
        {
            return null;
        }

        payroll.PaymentStatus = "Paid";
        payroll.PaymentDate = DateTime.UtcNow;
        await _repository.UpdateAsync(payroll, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return MapToDto(payroll);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var payroll = await _repository.GetByIdAsync(id, cancellationToken);
        if (payroll is null)
        {
            return false;
        }

        await _repository.DeleteAsync(payroll, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static PayrollDto MapToDto(Payroll payroll) => new()
    {
        PayrollId = payroll.PayrollId,
        EmployeeId = payroll.EmployeeId,
        PayStart = payroll.PayStart,
        PayEnd = payroll.PayEnd,
        HoursWorked = payroll.HoursWorked,
        OvertimeHours = payroll.OvertimeHours,
        GrossPay = payroll.GrossPay,
        Deductions = payroll.Deductions,
        NetPay = payroll.NetPay,
        PaymentDate = payroll.PaymentDate,
        PaymentStatus = payroll.PaymentStatus
    };
}
