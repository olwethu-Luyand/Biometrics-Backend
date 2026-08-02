using BiometricClockingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BiometricClockingAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Report> Reports { get; set; } = null!;
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;
    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<Audit> Audits => Set<Audit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OtpCode>(entity =>
        {
            entity.ToTable("OtpCodes");

            entity.HasKey(otp => otp.OtpCodeId);

            entity.HasOne(otp => otp.Employee)
                .WithMany()
                .HasForeignKey(otp => otp.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(otp => otp.EmployeeId);
            entity.HasIndex(otp => otp.Purpose);
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

        modelBuilder.Entity<Audit>(entity =>
        {
            entity.ToTable("Audits");

            entity.HasKey(audit => audit.AuditId);

            entity.HasOne(audit => audit.Employee)
                .WithMany()
                .HasForeignKey(audit => audit.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(audit => audit.EmployeeId);
            entity.HasIndex(audit => audit.CreatedAt);
        });
    }
}
