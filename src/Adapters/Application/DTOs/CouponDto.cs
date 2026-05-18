namespace NexoraBackend.Application.DTOs;

public record CouponDto(
    Guid Id,
    string Code,
    string Description,
    string Type,
    decimal DiscountValue,
    decimal? MaximumDiscountAmount,
    string? MaximumDiscountCurrency,
    decimal? MinimumOrderAmount,
    string? MinimumOrderCurrency,
    int? MaxUsageCount,
    int? MaxUsagePerUser,
    int CurrentUsageCount,
    DateTime? ExpiresAt,
    bool IsActive
    );