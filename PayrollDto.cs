namespace PayollModule.DTOs;

public class PayrollDto
{
    public int PayrollId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime PayStart { get; set; }
    public DateTime PayEnd { get; set; }
    public decimal HoursWorked { get; set; }
    public decimal OvertimeHours { get; set; }
    public decimal GrossPay { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetPay { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string PaymentStatus { get; set; } = "Pending";
}
