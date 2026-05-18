namespace NexoraBackend.Application.DTOs;

public record CategorySummaryDto(
    Guid Id,
    string Name,
    string Slug,
    string? ImageUrl,
    int SortOrder,
    int ProductCount);