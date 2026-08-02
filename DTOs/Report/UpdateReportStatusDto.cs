using System.ComponentModel.DataAnnotations;

namespace BiometricClockingAPI.DTOs.Report;

public class UpdateReportStatusDto
{
    [Required]
    [RegularExpression(
        "^(Open|In Progress|Closed)$",
        ErrorMessage = "Status must be Open, In Progress, or Closed."
    )]
    public string Status { get; set; } = string.Empty;
}