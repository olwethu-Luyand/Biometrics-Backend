using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiometricClockingAPI.Models;

public class Audit
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AuditId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public string Location { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime TimeOut { get; set; }

    public DateTime? LogoutTime { get; set; }

    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;

    public Employee Employee { get; set; } = null!;
}
