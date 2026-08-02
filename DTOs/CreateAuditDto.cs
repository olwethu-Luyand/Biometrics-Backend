using System.ComponentModel.DataAnnotations;

namespace BiometricClockingAPI.DTOs;

public class CreateAuditDto
{
    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public string Location { get; set; } = string.Empty;
}
