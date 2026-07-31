using Microsoft.EntityFrameworkCore;
using PayollModule.Data;
using PayollModule.Models;

namespace PayollModule.Repositories;

public class PayrollRepository : IPayrollRepository
{
    private readonly ApplicationDbContext _context;

    public PayrollRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Payroll?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Payrolls.FindAsync(new object?[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<Payroll>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Payrolls.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Payroll>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.Payrolls.AsNoTracking().Where(p => p.EmployeeId == employeeId).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Payroll payroll, CancellationToken cancellationToken = default)
    {
        await _context.Payrolls.AddAsync(payroll, cancellationToken);
    }

    public Task UpdateAsync(Payroll payroll, CancellationToken cancellationToken = default)
    {
        _context.Payrolls.Update(payroll);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Payroll payroll, CancellationToken cancellationToken = default)
    {
        _context.Payrolls.Remove(payroll);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
