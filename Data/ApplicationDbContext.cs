using BiometricClockingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BiometricClockingAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Models.PasswordResetToken> PasswordResetTokens { get; set; } = null!;
    }
}
