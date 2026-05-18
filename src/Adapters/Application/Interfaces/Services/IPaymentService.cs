// src/Adapters/ECommerce.Application/Interfaces/Services/IPaymentService.cs
using NexoraBackend.Core.ValueObjects;

namespace NexoraBackend.Application.Interfaces.Services;

public interface IPaymentService
{
    /// <summary>
    /// Creates a Stripe PaymentIntent and returns (intentId, clientSecret).
    /// The clientSecret is sent to the browser so Stripe.js can confirm payment.
    /// We return a tuple of primitives — no Stripe types leak into the Application layer.
    /// </summary>
    Task<(string intentId, string clientSecret)> CreatePaymentIntentAsync(
        Money amount, Guid orderId, CancellationToken ct = default);

    Task<bool> ConfirmPaymentAsync(string paymentIntentId, CancellationToken ct = default);
    Task<string> RefundPaymentAsync(string chargeId, Money amount, CancellationToken ct = default);
}