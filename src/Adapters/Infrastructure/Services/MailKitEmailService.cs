namespace NexoraBackend.Infrastructure.Services;

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NexoraBackend.Application.Interfaces.Services;
using NexoraBackend.Config;

/// <summary>
/// MailKit is used over SmtpClient (built-in) because it supports async,
/// modern TLS, OAuth2, and is actively maintained.
/// All email content is inline HTML — for production, use a templating engine
/// like Fluid or Scriban to keep HTML out of C# code.
/// </summary>
public class MailKitEmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<MailKitEmailService> _logger;

    public EmailSettings Settings => _settings;

    public MailKitEmailService(IOptions<EmailSettings> options,
        ILogger<MailKitEmailService> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task SendOrderConfirmationAsync(string toEmail, string userName,
        string orderNumber, CancellationToken ct = default)
    {
        var body = $@"
            <h2>Order Confirmed!</h2>
            <p>Hi {userName},</p>
            <p>Your order <strong>{orderNumber}</strong> has been confirmed and is being processed.</p>
            <p>We'll send you another email when it ships.</p>";

        await SendAsync(toEmail, userName, $"Order Confirmed: {orderNumber}", body, ct);
    }

    public async Task SendOrderShippedAsync(string toEmail, string userName,
        string orderNumber, string trackingNumber, CancellationToken ct = default)
    {
        var body = $@"
            <h2>Your Order Has Shipped!</h2>
            <p>Hi {userName},</p>
            <p>Order <strong>{orderNumber}</strong> is on its way.</p>
            <p>Tracking number: <strong>{trackingNumber}</strong></p>";

        await SendAsync(toEmail, userName, $"Order Shipped: {orderNumber}", body, ct);
    }

    public async Task SendEmailVerificationAsync(string toEmail, string userName,
        string verificationToken, CancellationToken ct = default)
    {
        var link = $"https://nexora-ecom.com/verify-email?token={verificationToken}";
        var body = $@"
            <h2>Verify Your Email</h2>
            <p>Hi {userName}, click below to verify your account:</p>
            <a href='{link}' style='padding:12px 24px;background:#4F46E5;color:white;text-decoration:none;border-radius:6px'>
                Verify Email
            </a>";

        await SendAsync(toEmail, userName, "Verify Your Email Address", body, ct);
    }

    public async Task SendPasswordResetAsync(string toEmail, string userName,
        string resetToken, CancellationToken ct = default)
    {
        var link = $"https://nexora-ecom.com/reset-password?token={resetToken}";
        var body = $@"
            <h2>Password Reset</h2>
            <p>Hi {userName}, click below to reset your password. This link expires in 1 hour.</p>
            <a href='{link}' style='padding:12px 24px;background:#DC2626;color:white;text-decoration:none;border-radius:6px'>
                Reset Password
            </a>
            <p>If you didn't request this, ignore this email.</p>";

        await SendAsync(toEmail, userName, "Reset Your Password", body, ct);
    }

    private async Task SendAsync(string toEmail, string toName,
        string subject, string htmlBody, CancellationToken ct)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(Settings.FromName, Settings.FromAddress));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(Settings.Host, Settings.Port,
                Settings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None, ct);
            await client.AuthenticateAsync(Settings.Username, Settings.Password, ct);
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);

            _logger.LogInformation("Email '{Subject}' sent to {Email}", subject, toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email '{Subject}' to {Email}", subject, toEmail);
            // Fire-and-forget: don't propagate email failures to callers
        }
    }
}