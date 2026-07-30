using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public enum EmployeeRole { Employee, Manager, HR, Admin }
public enum EmploymentType { FullTime, PartTime, Contractor }

public class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(20)]
    public string EmployeeCode { get; set; } = string.Empty; // e.g. EMP-0001

    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public EmployeeRole Role { get; set; }

    [MaxLength(100)]
    public string? Department { get; set; }

    [MaxLength(100)]
    public string? JobTitle { get; set; }

    public EmploymentType? EmploymentType { get; set; }

    public DateOnly? HireDate { get; set; }

    [MaxLength(100)]
    public string? WorkSchedule { get; set; }

    public Guid? ManagerId { get; set; }
    [ForeignKey(nameof(ManagerId))]
    public Employee? Manager { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Attendance> AttendanceRecords { get; set; } = new List<Attendance>();
}