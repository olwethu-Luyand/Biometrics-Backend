using BiometricClockingAPI.Data;
using BiometricClockingAPI.DTOs.Payroll;
using BiometricClockingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BiometricClockingAPI.Services;

public class PayrollService : IPayrollService
{
    private readonly ApplicationDbContext _context;

    public PayrollService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PayrollResponseDto> CalculateAsync(
    CalculatePayrollDto request)
{
    if (request.PayEnd < request.PayStart)
    {
        throw new ArgumentException(
            "PayEnd cannot be earlier than PayStart.");
    }

    var employee = await _context.Employees
        .FirstOrDefaultAsync(employee =>
            employee.EmployeeId == request.EmployeeId);

    if (employee is null)
    {
        throw new KeyNotFoundException("Employee not found.");
    }

    var duplicateExists = await _context.Payrolls
        .AnyAsync(payroll =>
            payroll.EmployeeId == request.EmployeeId &&
            payroll.PayStart == request.PayStart &&
            payroll.PayEnd == request.PayEnd);

    if (duplicateExists)
    {
        throw new InvalidOperationException(
            "Payroll already exists for this employee and pay period.");
    }

    var attendanceRecords = await _context.AttendanceRecords
        .AsNoTracking()
        .Where(record =>
            record.EmployeeId == request.EmployeeId &&
            record.AttendanceDate >= request.PayStart &&
            record.AttendanceDate <= request.PayEnd)
        .ToListAsync();

    if (attendanceRecords.Count == 0)
    {
        throw new InvalidOperationException(
            "No attendance records were found for this pay period.");
    }

    var presentRecords = attendanceRecords
        .Where(record =>
            !string.Equals(
                record.Status,
                "Absent",
                StringComparison.OrdinalIgnoreCase))
        .ToList();

    var absentDays = attendanceRecords.Count(record =>
        string.Equals(
            record.Status,
            "Absent",
            StringComparison.OrdinalIgnoreCase));

    var hoursWorked = presentRecords.Sum(record =>
        record.TotalWorkedHours);

    var overtimeHours = presentRecords.Sum(record =>
        record.OvertimeHours);

    var regularHours = Math.Max(
        0,
        hoursWorked - overtimeHours);

    var regularPay = Math.Round(
        regularHours * request.HourlyRate,
        2);

    var overtimePay = Math.Round(
        overtimeHours * request.OvertimeRate,
        2);

    var dailyRate = request.HourlyRate * 8;

    var deductions = Math.Round(
        absentDays * dailyRate,
        2);

    var grossPay = regularPay + overtimePay;

    var netPay = Math.Max(
        0,
        grossPay - deductions);

    var payroll = new Payroll
    {
        EmployeeId = request.EmployeeId,
        PayStart = request.PayStart,
        PayEnd = request.PayEnd,
        HoursWorked = hoursWorked,
        OvertimeHours = overtimeHours,
        AbsentDays = absentDays,
        HourlyRate = request.HourlyRate,
        OvertimeRate = request.OvertimeRate,
        RegularPay = regularPay,
        OvertimePay = overtimePay,
        Deductions = deductions,
        GrossPay = grossPay,
        NetPay = netPay,
        Status = "Calculated",
        CalculatedAt = DateTime.UtcNow
    };

    _context.Payrolls.Add(payroll);
    await _context.SaveChangesAsync();

    payroll.Employee = employee;

    return MapToResponse(payroll);
}

    public async Task<List<PayrollResponseDto>> GetAllAsync()
    {
        return await _context.Payrolls
            .AsNoTracking()
            .Include(payroll => payroll.Employee)
            .OrderByDescending(payroll => payroll.CalculatedAt)
            .Select(payroll => MapToResponse(payroll))
            .ToListAsync();
    }

    public async Task<List<PayrollResponseDto>> GetByEmployeeAsync(
        int employeeId)
    {
        return await _context.Payrolls
            .AsNoTracking()
            .Include(payroll => payroll.Employee)
            .Where(payroll =>
                payroll.EmployeeId == employeeId)
            .OrderByDescending(payroll =>
                payroll.CalculatedAt)
            .Select(payroll => MapToResponse(payroll))
            .ToListAsync();
    }

    public async Task<PayrollResponseDto?> GetByIdAsync(
        int payrollId)
    {
        var payroll = await _context.Payrolls
            .AsNoTracking()
            .Include(record => record.Employee)
            .FirstOrDefaultAsync(record =>
                record.PayrollId == payrollId);

        return payroll is null
            ? null
            : MapToResponse(payroll);
    }

    public async Task<bool> ApproveAsync(int payrollId)
    {
        var payroll = await _context.Payrolls
            .FirstOrDefaultAsync(record =>
                record.PayrollId == payrollId);

        if (payroll is null)
        {
            return false;
        }

        if (payroll.Status == "Paid")
        {
            throw new InvalidOperationException(
                "A paid payroll record cannot be approved again.");
        }

        payroll.Status = "Approved";
        payroll.ApprovedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> MarkPaidAsync(int payrollId)
    {
        var payroll = await _context.Payrolls
            .FirstOrDefaultAsync(record =>
                record.PayrollId == payrollId);

        if (payroll is null)
        {
            return false;
        }

        if (payroll.Status != "Approved")
        {
            throw new InvalidOperationException(
                "Payroll must be approved before it can be marked as paid.");
        }

        payroll.Status = "Paid";
        payroll.PaidAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    private static PayrollResponseDto MapToResponse(
    Payroll payroll)
    {
        return new PayrollResponseDto
        {
            PayrollId = payroll.PayrollId,
            EmployeeId = payroll.EmployeeId,
            EmployeeName = payroll.Employee is null
                ? string.Empty
                : $"{payroll.Employee.Name} {payroll.Employee.Surname}",
            PayStart = payroll.PayStart,
            PayEnd = payroll.PayEnd,
            HoursWorked = payroll.HoursWorked,
            OvertimeHours = payroll.OvertimeHours,
            AbsentDays = payroll.AbsentDays,
            HourlyRate = payroll.HourlyRate,
            OvertimeRate = payroll.OvertimeRate,
            RegularPay = payroll.RegularPay,
            OvertimePay = payroll.OvertimePay,
            Deductions = payroll.Deductions,
            GrossPay = payroll.GrossPay,
            NetPay = payroll.NetPay,
            Status = payroll.Status,
            CalculatedAt = payroll.CalculatedAt
        };
    }
}