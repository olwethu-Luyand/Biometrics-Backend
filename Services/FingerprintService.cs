using BiometricClockingAPI.Data;
using BiometricClockingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BiometricClockingAPI.Services;

public class FingerprintService : IFingerprintService
{
    private readonly ApplicationDbContext _context;

    public FingerprintService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Employee?> EnrollAsync(
        int employeeId,
        string fingerprintTemplate,
        string scannerDeviceId)
    {
        var employee = await _context.Employees.FindAsync(employeeId);

        if (employee is null)
        {
            return null;
        }

        employee.FingerprintTemplate = fingerprintTemplate;
        employee.ScannerDeviceId = scannerDeviceId;
        employee.FingerprintEnrolled = true;
        employee.FingerprintEnrolledAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return employee;
    }

    public async Task<Employee?> VerifyAsync(
        string fingerprintTemplate)
    {
        /*
         * Temporary exact comparison.
         *
         * Replace this when the physical scanner is selected.
         * A real scanner SDK must perform biometric template matching
         * and return a match score.
         */
        return await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(employee =>
                employee.FingerprintEnrolled &&
                employee.FingerprintTemplate == fingerprintTemplate);
    }

    public async Task<bool> RemoveAsync(int employeeId)
    {
        var employee = await _context.Employees.FindAsync(employeeId);

        if (employee is null)
        {
            return false;
        }

        employee.FingerprintTemplate = null;
        employee.ScannerDeviceId = null;
        employee.FingerprintEnrolled = false;
        employee.FingerprintEnrolledAt = null;

        await _context.SaveChangesAsync();

        return true;
    }
}