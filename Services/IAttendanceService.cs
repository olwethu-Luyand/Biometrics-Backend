using BiometricClockingAPI.Models;

namespace BiometricClockingAPI.Services;

public interface IAttendanceService
{
    Task<AttendanceRecord> ClockAsync(int employeeId);

    Task<AttendanceRecord?> GetTodayAsync(int employeeId);

    Task<List<AttendanceRecord>> GetHistoryAsync(int employeeId);

    Task<List<AttendanceRecord>> GetAllAsync();

    Task<List<AttendanceRecord>> GetTodayAllAsync();

    Task<List<AttendanceRecord>> GetAllHistoryAsync();
}