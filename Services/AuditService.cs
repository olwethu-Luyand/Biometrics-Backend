using AuditModule.Data;
using AuditModule.DTOs;
using AuditModule.Interfaces;
using AuditModule.Models;
using Microsoft.EntityFrameworkCore;

namespace AuditModule.Services;

public class AuditService : IAuditService
{
    private readonly AuditDbContext _context;
    private readonly TimeSpan _timeout = TimeSpan.FromMinutes(5);
    private readonly string _assignedWorkstation;

    public AuditService(AuditDbContext context, IConfiguration configuration)
    {
        _context = context;
        _assignedWorkstation = configuration["AuditSettings:AssignedWorkstation"] ?? "Office A";
    }

    public async Task<AuditResponseDto> LoginAsync(CreateAuditDto dto)
    {
        var now = DateTime.UtcNow;
        var audit = new Audit
        {
            AuditId = Guid.NewGuid(),
            EmployeeId = dto.EmployeeId,
            Location = dto.Location,
            Status = DetermineStatus(dto.Location, _assignedWorkstation),
            CreatedAt = now,
            TimeOut = now.Add(_timeout),
            LastActivityAt = now
        };

        _context.Audits.Add(audit);
        await _context.SaveChangesAsync();

        return MapToResponse(audit);
    }

    public async Task<AuditResponseDto> UpdateLocationAsync(Guid employeeId, string location)
    {
        var audit = await GetActiveAuditAsync(employeeId);

        if (audit is null)
        {
            throw new InvalidOperationException("No active audit session found for this employee.");
        }

        if (IsExpired(audit))
        {
            await ExpireSessionAsync(audit);
            return MapToResponse(audit, "Session expired. Please login again.");
        }

        audit.Location = location;
        audit.Status = DetermineStatus(location, _assignedWorkstation);
        audit.LastActivityAt = DateTime.UtcNow;
        audit.TimeOut = DateTime.UtcNow.Add(_timeout);

        await _context.SaveChangesAsync();
        return MapToResponse(audit);
    }

    public async Task LogoutAsync(Guid employeeId)
    {
        var audit = await GetActiveAuditAsync(employeeId);

        if (audit is null)
        {
            return;
        }

        audit.LogoutTime = DateTime.UtcNow;
        audit.LastActivityAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<AuditResponseDto> CheckSessionAsync(Guid employeeId)
    {
        var audit = await GetActiveAuditAsync(employeeId);

        if (audit is null)
        {
            throw new InvalidOperationException("No active audit session found for this employee.");
        }

        if (IsExpired(audit))
        {
            await ExpireSessionAsync(audit);
            return MapToResponse(audit, "Session expired. Please login again.");
        }

        return MapToResponse(audit);
    }

    public async Task<IEnumerable<AuditResponseDto>> GetAuditHistoryAsync(Guid employeeId)
    {
        var audits = await _context.Audits
            .Where(a => a.EmployeeId == employeeId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return audits.Select(a => MapToResponse(a));
    }

    private async Task<Audit?> GetActiveAuditAsync(Guid employeeId)
    {
        return await _context.Audits
            .Where(a => a.EmployeeId == employeeId && a.LogoutTime == null)
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync();
    }

    private bool IsExpired(Audit audit)
    {
        return DateTime.UtcNow - audit.LastActivityAt > _timeout;
    }

    private async Task ExpireSessionAsync(Audit audit)
    {
        audit.TimeOut = DateTime.UtcNow;
        audit.LogoutTime = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    private static string DetermineStatus(string location, string assignedWorkstation)
    {
        return string.IsNullOrWhiteSpace(location)
            ? "Not at workstation"
            : location.Equals(assignedWorkstation, StringComparison.OrdinalIgnoreCase)
                ? "Present at workstation"
                : "Not at workstation";
    }

    private static AuditResponseDto MapToResponse(Audit audit, string? message = null)
    {
        return new AuditResponseDto
        {
            AuditId = audit.AuditId,
            EmployeeId = audit.EmployeeId,
            Location = audit.Location,
            Status = audit.Status,
            CreatedAt = audit.CreatedAt,
            TimeOut = audit.TimeOut,
            LogoutTime = audit.LogoutTime,
            Message = message
        };
    }
}
