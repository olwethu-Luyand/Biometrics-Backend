using System.ComponentModel.DataAnnotations;

namespace BiometricClockingAPI.DTOs.Report;

public class CreateReportDto
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;
}