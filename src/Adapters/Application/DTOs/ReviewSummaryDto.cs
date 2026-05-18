namespace NexoraBackend.Application.DTOs;

public record ReviewSummaryDto(
    Guid Id,
    Guid UserId,
    string UserFullName,
    int Rating,
    string Title,
    bool IsVerifiedPurchase,
    int HelpfulCount,
    DateTime CreatedAt);