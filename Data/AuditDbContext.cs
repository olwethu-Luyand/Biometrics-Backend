using AuditModule.Models;
using Microsoft.EntityFrameworkCore;

namespace AuditModule.Data;

public class AuditDbContext : DbContext
{
    public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options)
    {
    }

    public DbSet<Audit> Audits => Set<Audit>();
}
