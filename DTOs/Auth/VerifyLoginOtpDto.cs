using System.ComponentModel.DataAnnotations;

namespace BiometricClockingAPI.DTOs.Auth;

public class VerifyLoginOtpDto
{
    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{6}$")]
    public string Otp { get; set; } = string.Empty;
}