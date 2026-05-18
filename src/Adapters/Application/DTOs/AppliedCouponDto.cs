namespace NexoraBackend.Application.DTOs;

public record AppliedCouponDto(
    string Code,
    string Description,
    string DiscountType,
    decimal DiscountAmount,
    string Currency
);
