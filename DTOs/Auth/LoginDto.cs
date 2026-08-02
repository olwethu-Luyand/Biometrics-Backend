using System.ComponentModel.DataAnnotations;

namespace BiometricClockingAPI.DTOs.Auth;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}