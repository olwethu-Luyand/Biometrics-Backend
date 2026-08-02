using System.ComponentModel.DataAnnotations;

namespace BiometricClockingAPI.DTOs.Payroll;

public class CalculatePayrollDto
{
    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public DateOnly PayStart { get; set; }

    [Required]
    public DateOnly PayEnd { get; set; }

    [Range(0, double.MaxValue)]
    public decimal HourlyRate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal OvertimeRate { get; set; }
}