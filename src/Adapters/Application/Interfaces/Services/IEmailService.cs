namespace NexoraBackend.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendOrderConfirmationAsync(string toEmail, string userName, string orderNumber, CancellationToken ct = default);
    Task SendOrderShippedAsync(string toEmail, string userName, string orderNumber, string trackingNumber, CancellationToken ct = default);
    Task SendEmailVerificationAsync(string toEmail, string userName, string verificationToken, CancellationToken ct = default);
    Task SendPasswordResetAsync(string toEmail, string userName, string resetToken, CancellationToken ct = default);
}