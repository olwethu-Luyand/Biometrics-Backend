using System.ComponentModel.DataAnnotations;

namespace BiometricClockingAPI.DTOs;

public class UpdateLocationDto
{
    [Required]
    public string Location { get; set; } = string.Empty;
}
