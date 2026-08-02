using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiometricClockingAPI.Models;

public class Payroll
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PayrollId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

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

    public string Status { get; set; } = "Calculated";

    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ApprovedAt { get; set; }

    public DateTime? PaidAt { get; set; }

    public Employee Employee { get; set; } = null!;
}