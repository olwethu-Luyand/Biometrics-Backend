using BiometricClockingAPI.Models;

namespace BiometricClockingAPI.Services;

public interface IJwtTokenService
{
    string GenerateToken(Employee employee);
}