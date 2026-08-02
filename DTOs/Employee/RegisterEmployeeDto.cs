using System.ComponentModel.DataAnnotations;

namespace BiometricClockingAPI.DTOs.Employee;

public class RegisterEmployeeDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Surname { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "Employee";

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; } = string.Empty;

    public string? FingerprintTemplate { get; set; }

    public string? ScannerDeviceId { get; set; }
}