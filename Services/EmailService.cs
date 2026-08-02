using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace BiometricClockingAPI.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendOtpAsync(
        string recipientEmail,
        string recipientName,
        string otp,
        string purpose)
    {
        var subject = purpose == "Login"
            ? "Your login verification code"
            : "Your password reset code";

        var action = purpose == "Login"
            ? "complete your login"
            : "reset your password";

        var body = $"""
            Hello {recipientName},

            Your PrimeOak verification code is:

            {otp}

            Use this code to {action}.

            The code expires in 5 minutes and can only be used once.

            If you did not request this code, ignore this email.
            """;

        using var message = new MailMessage
        {
            From = new MailAddress(
                _settings.SenderEmail,
                _settings.SenderName
            ),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        message.To.Add(recipientEmail);

        using var smtpClient = new SmtpClient(
            _settings.Host,
            _settings.Port
        )
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(
                _settings.SenderEmail,
                _settings.AppPassword
            )
        };

        await smtpClient.SendMailAsync(message);
    }
}