using System.ComponentModel.DataAnnotations;

namespace BiometricClockingAPI.DTOs.Auth;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; } = string.Empty;
}