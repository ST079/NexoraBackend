namespace NexoraBackend.Application.DTOs;

public record ProductSummaryDto(
    Guid Id,
    string Name,
    string Slug,
    decimal Price,
    string Currency,
    int StockQuantity,
    bool IsActive,
    string? FirstImageUrl,
    decimal AverageRating
);