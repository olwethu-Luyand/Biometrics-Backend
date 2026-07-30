using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Attendance> AttendanceRecords => Set<Attendance>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Employee>()
            .HasIndex(e => e.Email)
            .IsUnique();

        builder.Entity<Employee>()
            .HasIndex(e => e.EmployeeCode)
            .IsUnique();

        builder.Entity<Attendance>()
            .HasIndex(a => new { a.EmployeeId, a.Date })
            .IsUnique();

        // Lowercase table/column names — Postgres convention
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()!.ToLower());
            foreach (var property in entity.GetProperties())
                property.SetColumnName(property.GetColumnName().ToLower());
        }
    }
}