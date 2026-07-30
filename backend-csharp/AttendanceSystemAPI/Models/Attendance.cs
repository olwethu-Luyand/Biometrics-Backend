using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public enum ClockMethod { Web, Mobile, Biometric, Manual }
public enum AttendanceStatus { Present, Absent, Late, HalfDay, OnLeave }

public class Attendance
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid EmployeeId { get; set; }
    [ForeignKey(nameof(EmployeeId))]
    public Employee Employee { get; set; } = null!;

    [Required]
    public DateOnly Date { get; set; }

    public DateTime? ClockIn { get; set; }
    public DateTime? ClockOut { get; set; }

    public ClockMethod? ClockInMethod { get; set; }
    public ClockMethod? ClockOutMethod { get; set; }

    public AttendanceStatus? Status { get; set; }
    public decimal? HoursWorked { get; set; }

    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}