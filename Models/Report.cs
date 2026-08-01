using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;
    }
}
