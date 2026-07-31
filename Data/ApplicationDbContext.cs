using Microsoft.EntityFrameworkCore;
using PayollModule.Models;

namespace PayollModule.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Payroll> Payrolls { get; set; } = null!;
}
