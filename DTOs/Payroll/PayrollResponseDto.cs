namespace BiometricClockingAPI.DTOs.Payroll;

public class PayrollResponseDto
{
    public int PayrollId { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public DateOnly PayStart { get; set; }

    public DateOnly PayEnd { get; set; }

    public decimal HoursWorked { get; set; }

    public decimal OvertimeHours { get; set; }

    public int AbsentDays { get; set; }

    public decimal HourlyRate { get; set; }

    public decimal OvertimeRate { get; set; }

    public decimal RegularPay { get; set; }

    public decimal OvertimePay { get; set; }

    public decimal Deductions { get; set; }

    public decimal GrossPay { get; set; }

    public decimal NetPay { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CalculatedAt { get; set; }
}