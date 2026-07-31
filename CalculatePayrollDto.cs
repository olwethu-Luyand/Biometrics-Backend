namespace PayollModule.DTOs;

public class CalculatePayrollDto
{
    public int EmployeeId { get; set; }
    public DateTime PayStart { get; set; }
    public DateTime PayEnd { get; set; }
    public decimal HoursWorked { get; set; }
    public decimal OvertimeHours { get; set; }
    public decimal Deductions { get; set; }
}
