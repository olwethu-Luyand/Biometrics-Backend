using System.ComponentModel.DataAnnotations;

namespace BiometricClockingAPI.DTOs.Employee;

public class RegisterFingerprintDto
{
    [Required]
    public string FingerprintTemplate { get; set; } = string.Empty;

    [Required]
    public string ScannerDeviceId { get; set; } = string.Empty;
}