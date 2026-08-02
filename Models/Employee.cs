using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiometricClockingAPI.Models;

public class Employee
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EmployeeId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Surname { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "Employee";

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; } = string.Empty;

    public string? FingerprintTemplate { get; set; }

    public string? ScannerDeviceId { get; set; }

    public bool FingerprintEnrolled { get; set; }

    public DateTime? FingerprintEnrolledAt { get; set; }

    // Keep this for the database relationship.
    // It will not appear in POST requests when a DTO is used.
    public ICollection<Report> Reports { get; set; } = new List<Report>();
}