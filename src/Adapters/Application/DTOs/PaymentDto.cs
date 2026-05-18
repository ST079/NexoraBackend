namespace NexoraBackend.Application.DTOs;

public record PaymentDto(
    Guid Id,
    Guid OrderId,
    decimal Amount,
    string Currency,
    string Status,
    string PaymentMethod,
    string? StripePaymentIntentId,
    string? StripeChargeId,
    string? FailureReason,
    DateTime CreatedAt
    );
