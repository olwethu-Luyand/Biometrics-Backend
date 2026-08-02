using System.ComponentModel.DataAnnotations;

namespace BiometricClockingAPI.DTOs.Employee;

public class UpdateEmployeeDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Surname { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "Employee";

    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; } = string.Empty;

    [MinLength(8)]
    public string? Password { get; set; }
}