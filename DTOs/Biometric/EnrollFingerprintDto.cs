using System.ComponentModel.DataAnnotations;

namespace BiometricClockingAPI.DTOs.Biometric;

public class EnrollFingerprintDto
{
    [Required]
    public string FingerprintTemplate { get; set; } = string.Empty;

    [Required]
    public string ScannerDeviceId { get; set; } = string.Empty;
}