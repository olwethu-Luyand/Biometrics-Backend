using BiometricClockingAPI.DTOs.Payroll;

namespace BiometricClockingAPI.Services;

public interface IPayrollService
{
    Task<PayrollResponseDto> CalculateAsync(
        CalculatePayrollDto request);

    Task<List<PayrollResponseDto>> GetAllAsync();

    Task<List<PayrollResponseDto>> GetByEmployeeAsync(
        int employeeId);

    Task<PayrollResponseDto?> GetByIdAsync(int payrollId);

    Task<bool> ApproveAsync(int payrollId);

    Task<bool> MarkPaidAsync(int payrollId);
}