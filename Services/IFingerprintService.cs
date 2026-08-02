using BiometricClockingAPI.Models;

namespace BiometricClockingAPI.Services;

public interface IFingerprintService
{
    Task<Employee?> EnrollAsync(
        int employeeId,
        string fingerprintTemplate,
        string scannerDeviceId);

    Task<Employee?> VerifyAsync(string fingerprintTemplate);

    Task<bool> RemoveAsync(int employeeId);
}