namespace BiometricClockingAPI.Services;

public interface IEmailService
{
    Task SendOtpAsync(
        string recipientEmail,
        string recipientName,
        string otp,
        string purpose);
}