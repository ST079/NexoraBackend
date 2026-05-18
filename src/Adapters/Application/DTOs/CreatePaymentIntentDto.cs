namespace NexoraBackend.Application.DTOs;

public record CreatePaymentIntentDto(
        string PaymentIntentId,
        string ClientSecret,
        decimal Amount,
        string Currency);