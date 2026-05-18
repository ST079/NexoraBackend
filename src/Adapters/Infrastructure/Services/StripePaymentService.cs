namespace NexoraBackend.Infrastructure.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NexoraBackend.Application.Interfaces.Services;
using NexoraBackend.Config;
using NexoraBackend.Core.ValueObjects;
using Stripe;

/// <summary>
/// Wraps Stripe SDK calls. Returns (intentId, clientSecret) tuple so the
/// Application layer only knows about primitive types — no Stripe types leak up.
/// The clientSecret is returned to the browser for Stripe.js to complete payment.
/// </summary>
public class StripePaymentService : IPaymentService
{
    private readonly StripeSettings _settings;
    private readonly ILogger<StripePaymentService> _logger;

    public StripePaymentService(IOptions<StripeSettings> options,
        ILogger<StripePaymentService> logger)
    {
        _settings = options.Value;
        _logger = logger;
        StripeConfiguration.ApiKey = _settings.SecretKey;
    }

    public async Task<(string intentId, string clientSecret)> CreatePaymentIntentAsync(
        Money amount, Guid orderId, CancellationToken ct = default)
    {
        var options = new PaymentIntentCreateOptions
        {
            // Stripe uses smallest currency unit (cents for USD)
            Amount = (long)(amount.Amount * 100),
            Currency = amount.Currency.ToLowerInvariant(),
            Metadata = new Dictionary<string, string>
            {
                ["orderId"] = orderId.ToString()
            },
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            }
        };

        var service = new PaymentIntentService();
        var intent = await service.CreateAsync(options, cancellationToken: ct);

        _logger.LogInformation("Created Stripe PaymentIntent {IntentId} for order {OrderId}",
            intent.Id, orderId);

        return (intent.Id, intent.ClientSecret);
    }

    public async Task<bool> ConfirmPaymentAsync(string paymentIntentId, CancellationToken ct = default)
    {
        var service = new PaymentIntentService();
        var intent = await service.GetAsync(paymentIntentId, cancellationToken: ct);
        return intent.Status == "succeeded";
    }

    public async Task<string> RefundPaymentAsync(string chargeId, Money amount,
        CancellationToken ct = default)
    {
        var options = new RefundCreateOptions
        {
            Charge = chargeId,
            Amount = (long)(amount.Amount * 100)
        };

        var service = new RefundService();
        var refund = await service.CreateAsync(options, cancellationToken: ct);
        return refund.Id;
    }
}