using BiometricClockingAPI.Data;
using BiometricClockingAPI.DTOs;
using BiometricClockingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BiometricClockingAPI.Services;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;
    private readonly TimeSpan _timeout;
    private readonly string _assignedWorkstation;

    public AuditService(
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        _context = context;

        _assignedWorkstation =
            configuration["AuditSettings:AssignedWorkstation"]
            ?? "Office A";

        var timeoutMinutes = int.TryParse(
            configuration["AuditSettings:TimeoutMinutes"],
            out var configuredTimeout)
                ? configuredTimeout
                : 5;

        _timeout = TimeSpan.FromMinutes(timeoutMinutes);
    }

    public async Task<AuditResponseDto> LoginAsync(
        CreateAuditDto request)
    {
        var employeeExists = await _context.Employees
            .AnyAsync(employee =>
                employee.EmployeeId == request.EmployeeId);

        if (!employeeExists)
        {
            throw new KeyNotFoundException("Employee not found.");
        }

        var now = DateTime.UtcNow;

        var currentSession = await GetActiveAuditAsync(
            request.EmployeeId);

        if (currentSession is not null)
        {
            currentSession.LogoutTime = now;
            currentSession.TimeOut = now;
            currentSession.LastActivityAt = now;
        }

        var audit = new Audit
        {
            EmployeeId = request.EmployeeId,
            Location = request.Location.Trim(),
            Status = DetermineStatus(
                request.Location,
                _assignedWorkstation),
            CreatedAt = now,
            LastActivityAt = now,
            TimeOut = now.Add(_timeout)
        };

        _context.Audits.Add(audit);
        await _context.SaveChangesAsync();

        return MapToResponse(audit);
    }

    public async Task<AuditResponseDto> UpdateLocationAsync(
        int employeeId,
        string location)
    {
        var audit = await GetActiveAuditAsync(employeeId);

        if (audit is null)
        {
            throw new InvalidOperationException(
                "No active audit session found.");
        }

        if (IsExpired(audit))
        {
            await ExpireSessionAsync(audit);

            return MapToResponse(
                audit,
                "Session expired. Please login again.");
        }

        var now = DateTime.UtcNow;

        audit.Location = location.Trim();
        audit.Status = DetermineStatus(
            location,
            _assignedWorkstation);
        audit.LastActivityAt = now;
        audit.TimeOut = now.Add(_timeout);

        await _context.SaveChangesAsync();

        return MapToResponse(audit);
    }

    public async Task LogoutAsync(int employeeId)
    {
        var audit = await GetActiveAuditAsync(employeeId);

        if (audit is null)
        {
            return;
        }

        var now = DateTime.UtcNow;

        audit.LogoutTime = now;
        audit.LastActivityAt = now;
        audit.TimeOut = now;

        await _context.SaveChangesAsync();
    }

    public async Task<AuditResponseDto> CheckSessionAsync(
        int employeeId)
    {
        var audit = await GetActiveAuditAsync(employeeId);

        if (audit is null)
        {
            throw new InvalidOperationException(
                "No active audit session found.");
        }

        if (IsExpired(audit))
        {
            await ExpireSessionAsync(audit);

            return MapToResponse(
                audit,
                "Session expired. Please login again.");
        }

        return MapToResponse(audit);
    }

    public async Task<IEnumerable<AuditResponseDto>>
        GetAuditHistoryAsync(int employeeId)
    {
        var audits = await _context.Audits
            .AsNoTracking()
            .Where(audit =>
                audit.EmployeeId == employeeId)
            .OrderByDescending(audit =>
                audit.CreatedAt)
            .ToListAsync();

        return audits.Select(MapToResponse);
    }

    public async Task<IEnumerable<AuditResponseDto>> GetAllAsync()
    {
        var audits = await _context.Audits
            .AsNoTracking()
            .OrderByDescending(audit =>
                audit.CreatedAt)
            .ToListAsync();

        return audits.Select(MapToResponse);
    }

    private async Task<Audit?> GetActiveAuditAsync(
        int employeeId)
    {
        return await _context.Audits
            .Where(audit =>
                audit.EmployeeId == employeeId &&
                audit.LogoutTime == null)
            .OrderByDescending(audit =>
                audit.CreatedAt)
            .FirstOrDefaultAsync();
    }

    private bool IsExpired(Audit audit)
    {
        return DateTime.UtcNow - audit.LastActivityAt >
               _timeout;
    }

    private async Task ExpireSessionAsync(Audit audit)
    {
        var now = DateTime.UtcNow;

        audit.TimeOut = now;
        audit.LogoutTime = now;
        audit.LastActivityAt = now;

        await _context.SaveChangesAsync();
    }

    private static string DetermineStatus(
        string location,
        string assignedWorkstation)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            return "Not at workstation";
        }

        return location.Equals(
            assignedWorkstation,
            StringComparison.OrdinalIgnoreCase)
                ? "Present at workstation"
                : "Not at workstation";
    }

    private static AuditResponseDto MapToResponse(
        Audit audit)
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
            LastActivityAt = audit.LastActivityAt
        };
    }

    private static AuditResponseDto MapToResponse(
        Audit audit,
        string message)
    {
        var response = MapToResponse(audit);
        response.Message = message;
        return response;
    }

}