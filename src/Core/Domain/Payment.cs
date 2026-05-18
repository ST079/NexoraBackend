
using NexoraBackend.Core.Enums;
using NexoraBackend.Core.Exceptions;
using NexoraBackend.Core.ValueObjects;

namespace NexoraBackend.Core.Domain;

public class Payment : Auditable
{
    public Guid OrderId { get; private set; }
    public Money Amount { get; private set; } = null!;
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public string? StripePaymentIntentId { get; private set; }
    public string? StripeChargeId { get; private set; }
    public string PaymentMethod { get; private set; } = null!;
    public string? FailureReason { get; private set; }

    private Payment() { }

    public static Payment Create(Guid orderId, Money amount, string paymentMethod)
    {
        return new Payment
        {
            OrderId = orderId,
            Amount = amount,
            PaymentMethod = paymentMethod
        };
    }

    public void SetStripeIntent(string paymentIntentId)
    {
        StripePaymentIntentId = paymentIntentId;
        Status = PaymentStatus.Processing;
    }

    public void Complete(string chargeId)
    {
        StripeChargeId = chargeId;
        Status = PaymentStatus.Completed;
    }

    public void Fail(string reason)
    {
        Status = PaymentStatus.Failed;
        FailureReason = reason;
    }

    public void Refund()
    {
        if (Status != PaymentStatus.Completed)
            throw new DomainException("Only completed payments can be refunded.");
        Status = PaymentStatus.Refunded;
    }
}