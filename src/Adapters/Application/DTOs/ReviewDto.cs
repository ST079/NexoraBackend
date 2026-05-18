namespace NexoraBackend.Application.DTOs;

public record ReviewDto(
    Guid Id,
    Guid ProductId,
    Guid UserId,
    string UserFullName,
    int Rating,
    string Title,
    string Body,
    string Status,
    bool IsVerifiedPurchase,
    int HelpfulCount,
    int NotHelpfulCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt
    );
