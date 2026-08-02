using BiometricClockingAPI.Data;
using BiometricClockingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BiometricClockingAPI.Services;

public class AttendanceService : IAttendanceService
{
    private readonly ApplicationDbContext _context;

    public AttendanceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AttendanceRecord> ClockAsync(int employeeId)
    {
        var employeeExists = await _context.Employees
            .AnyAsync(employee => employee.EmployeeId == employeeId);

        if (!employeeExists)
        {
            throw new InvalidOperationException(
                "Employee was not found."
            );
        }

        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(now);

        var attendance = await _context.AttendanceRecords
            .FirstOrDefaultAsync(record =>
                record.EmployeeId == employeeId &&
                record.AttendanceDate == today);

        if (attendance is null)
        {
            attendance = new AttendanceRecord
            {
                EmployeeId = employeeId,
                AttendanceDate = today,
                ClockInTime = now,
                ClockOutTime = null,
                TotalWorkedHours = 0,
                OvertimeHours = 0,
                Status = "Present",
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.AttendanceRecords.Add(attendance);
        }
        else if (attendance.ClockOutTime is null)
        {
            attendance.ClockOutTime = now;

            if (attendance.ClockInTime.HasValue)
            {
                var workedDuration =
                    attendance.ClockOutTime.Value -
                    attendance.ClockInTime.Value;

                attendance.TotalWorkedHours =
                    Math.Round(
                        (decimal)workedDuration.TotalHours,
                        2
                    );

                attendance.OvertimeHours =
                    Math.Max(
                        0,
                        attendance.TotalWorkedHours - 8
                    );
            }

            attendance.UpdatedAt = now;
        }
        else
        {
            throw new InvalidOperationException(
                "The employee has already clocked in and out today."
            );
        }

        await _context.SaveChangesAsync();

        return attendance;
    }

    public async Task<AttendanceRecord?> GetTodayAsync(int employeeId)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return await _context.AttendanceRecords
            .AsNoTracking()
            .Include(record => record.Employee)
            .FirstOrDefaultAsync(record =>
                record.EmployeeId == employeeId &&
                record.AttendanceDate == today);
    }

    public async Task<List<AttendanceRecord>> GetHistoryAsync(
        int employeeId)
    {
        return await _context.AttendanceRecords
            .AsNoTracking()
            .Include(record => record.Employee)
            .Where(record =>
                record.EmployeeId == employeeId)
            .OrderByDescending(record =>
                record.AttendanceDate)
            .ToListAsync();
    }

    public async Task<List<AttendanceRecord>> GetAllAsync()
    {
        return await _context.AttendanceRecords
            .AsNoTracking()
            .Include(a => a.Employee)
            .OrderByDescending(a => a.AttendanceDate)
            .ThenBy(a => a.Employee.Name)
            .ToListAsync();
    }

    public async Task<List<AttendanceRecord>> GetTodayAllAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return await _context.AttendanceRecords
            .AsNoTracking()
            .Include(record => record.Employee)
            .Where(record => record.AttendanceDate == today)
            .OrderBy(record => record.ClockInTime)
            .ToListAsync();
    }

    public async Task<List<AttendanceRecord>> GetAllHistoryAsync()
    {
        return await _context.AttendanceRecords
            .AsNoTracking()
            .Include(record => record.Employee)
            .OrderByDescending(record => record.AttendanceDate)
            .ThenBy(record => record.Employee.Name)
            .ToListAsync();
    }
}