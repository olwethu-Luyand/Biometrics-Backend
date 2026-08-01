using System;

namespace BiometricClockingAPI.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool Used { get; set; }
    }
}
