using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiometricClockingAPI.Models;

public class OtpCode
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OtpCodeId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public string CodeHash { get; set; } = string.Empty;

    [Required]
    public string Purpose { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public bool Used { get; set; }

    public int FailedAttempts { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Employee Employee { get; set; } = null!;
}