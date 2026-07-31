using PayollModule.Models;

namespace PayollModule.Repositories;

public interface IPayrollRepository
{
    Task<Payroll?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payroll>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payroll>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
    Task AddAsync(Payroll payroll, CancellationToken cancellationToken = default);
    Task UpdateAsync(Payroll payroll, CancellationToken cancellationToken = default);
    Task DeleteAsync(Payroll payroll, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
