using AuditModule.DTOs;

namespace AuditModule.Interfaces;

public interface IAuditService
{
    Task<AuditResponseDto> LoginAsync(CreateAuditDto dto);

    Task<AuditResponseDto> UpdateLocationAsync(Guid employeeId, string location);

    Task LogoutAsync(Guid employeeId);

    Task<AuditResponseDto> CheckSessionAsync(Guid employeeId);

    Task<IEnumerable<AuditResponseDto>> GetAuditHistoryAsync(Guid employeeId);
}
