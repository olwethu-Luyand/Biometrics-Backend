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
        public DbSet<Report> Reports { get; set; } = null!;
        public DbSet<Models.PasswordResetToken> PasswordResetTokens { get; set; } = null!;
        public DbSet<OtpCode> OtpCodes => Set<OtpCode>();
        
        public DbSet<AttendanceRecord> AttendanceRecords =>
                Set<AttendanceRecord>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OtpCode>(entity =>
            {
                entity.ToTable("OtpCodes");

                entity.HasKey(e => e.OtpCodeId);

                entity.HasOne(e => e.Employee)
                    .WithMany()
                    .HasForeignKey(e => e.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.EmployeeId);
                entity.HasIndex(e => e.Purpose);
            });

            modelBuilder.Entity<AttendanceRecord>(entity =>
            {
                entity.ToTable("AttendanceRecords");

                entity.HasKey(record => record.AttendanceId);

                entity.HasOne(record => record.Employee)
                    .WithMany()
                    .HasForeignKey(record => record.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(record => new
                {
                    record.EmployeeId,
                    record.AttendanceDate
                })
                .IsUnique();
            });
        }
    }
}
