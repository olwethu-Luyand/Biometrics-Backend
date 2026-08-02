using System.ComponentModel.DataAnnotations;

namespace BiometricClockingAPI.DTOs.Biometric;

public class VerifyFingerprintDto
{
    [Required]
    public string FingerprintTemplate { get; set; } = string.Empty;

    public string? ScannerDeviceId { get; set; }
}