namespace NexoraBackend.Application.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    string Slug,
    decimal Price,
    string Currency,
    decimal? CompareAtPrice,
    int StockQuantity,
    bool IsActive,
    bool IsFeatured,
    Guid CategoryId,
    string CategoryName,
    IReadOnlyList<string> ImageUrls,
    decimal AverageRating,
    int ReviewCount,
    DateTime CreatedAt
    );