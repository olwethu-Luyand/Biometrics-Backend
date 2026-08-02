using BiometricClockingAPI.DTOs;

namespace BiometricClockingAPI.Services;

public interface IAuditService
{
    Task<AuditResponseDto> LoginAsync(CreateAuditDto request);

    Task<AuditResponseDto> UpdateLocationAsync(
        int employeeId,
        string location);

    Task LogoutAsync(int employeeId);

    Task<AuditResponseDto> CheckSessionAsync(int employeeId);

    Task<IEnumerable<AuditResponseDto>> GetAuditHistoryAsync(
        int employeeId);

    Task<IEnumerable<AuditResponseDto>> GetAllAsync();
}
