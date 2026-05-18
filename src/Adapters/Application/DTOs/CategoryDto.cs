namespace NexoraBackend.Application.DTOs;


public record CategoryDto(
    Guid Id,
    string Name,
    string Description,
    string Slug,
    string? ImageUrl,
    bool IsActive,
    int SortOrder,
    Guid? ParentId,
    string? ParentName,
    int ProductCount,
    IReadOnlyList<CategorySummaryDto> Children
    );