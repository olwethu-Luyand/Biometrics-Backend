using System.ComponentModel.DataAnnotations;

namespace BiometricClockingAPI.Models
{
    public class Report
    {
        [Key]
        public int ReportId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Open";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
