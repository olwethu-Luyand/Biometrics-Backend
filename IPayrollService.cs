using PayollModule.DTOs;
using PayollModule.Models;

namespace PayollModule.Services;

public interface IPayrollService
{
    Payroll CalculatePayroll(CalculatePayrollDto dto);
    Task<IReadOnlyList<PayrollDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PayrollDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PayrollDto>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
    Task<PayrollDto> CreateAsync(CalculatePayrollDto dto, CancellationToken cancellationToken = default);
    Task<PayrollDto?> ApproveAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
