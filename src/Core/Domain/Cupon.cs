using NexoraBackend.Core.Enums;
using NexoraBackend.Core.Exceptions;
using NexoraBackend.Core.ValueObjects;

namespace NexoraBackend.Core.Domain;

/// Coupon enforces discount rules at the domain level.
/// Why not just store a discount % in the Order?
/// Because coupons have complex invariants: expiry dates, usage limits,
/// minimum order values, and per-user limits — all domain logic.
public class Coupon : Auditable
{
    public string Code { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public CouponType Type { get; private set; }

    // For PERCENTAGE type: 0.0–1.0 (e.g. 0.20 = 20% off)
    // For FIXED type: absolute amount off
    public decimal DiscountValue { get; private set; }

    public Money? MaximumDiscountAmount { get; private set; } // cap for percentage coupons
    public Money? MinimumOrderAmount { get; private set; }   // minimum cart value
    public int? MaxUsageCount { get; private set; }          // null = unlimited
    public int? MaxUsagePerUser { get; private set; }        // null = unlimited
    public int CurrentUsageCount { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Coupon() { }

    public static Coupon Create(
        string code,
        string description,
        CouponType type,
        decimal discountValue,
        DateTime? expiresAt = null,
        int? maxUsageCount = null,
        int? maxUsagePerUser = null,
        decimal? minimumOrderAmount = null,
        string currency = "USD")
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Coupon code is required.");

        if (type == CouponType.Percentage && (discountValue <= 0 || discountValue > 1))
            throw new DomainException("Percentage discount must be between 0 and 1 (e.g. 0.20 for 20%).");

        if (type == CouponType.Fixed && discountValue <= 0)
            throw new DomainException("Fixed discount must be positive.");

        return new Coupon
        {
            Code = code.Trim().ToUpperInvariant(),
            Description = description.Trim(),
            Type = type,
            DiscountValue = discountValue,
            ExpiresAt = expiresAt,
            MaxUsageCount = maxUsageCount,
            MaxUsagePerUser = maxUsagePerUser,
            MinimumOrderAmount = minimumOrderAmount.HasValue
                ? new Money(minimumOrderAmount.Value, currency)
                : null
        };
    }

    /// <summary>
    /// Calculate how much this coupon discounts a given subtotal.
    /// All rules (expiry, minimum, cap) are checked here — domain logic,
    /// not in the application service.
    /// </summary>
    public Money CalculateDiscount(Money subtotal, int userUsageCount = 0)
    {
        ValidateUsability(subtotal, userUsageCount);

        var discount = Type switch
        {
            CouponType.Percentage => subtotal.Multiply(DiscountValue),
            CouponType.Fixed => new Money(
                Math.Min(DiscountValue, subtotal.Amount), subtotal.Currency),
            _ => throw new DomainException($"Unknown coupon type: {Type}")
        };

        // Apply maximum cap for percentage coupons
        if (MaximumDiscountAmount is not null && discount.Amount > MaximumDiscountAmount.Amount)
            discount = new Money(MaximumDiscountAmount.Amount, subtotal.Currency);

        return discount;
    }

    public void ValidateUsability(Money subtotal, int userUsageCount = 0)
    {
        if (!IsActive)
            throw new DomainException($"Coupon '{Code}' is no longer active.");

        if (ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value)
            throw new DomainException($"Coupon '{Code}' expired on {ExpiresAt.Value:yyyy-MM-dd}.");

        if (MaxUsageCount.HasValue && CurrentUsageCount >= MaxUsageCount.Value)
            throw new DomainException($"Coupon '{Code}' has reached its maximum usage limit.");

        if (MaxUsagePerUser.HasValue && userUsageCount >= MaxUsagePerUser.Value)
            throw new DomainException($"You have already used coupon '{Code}' the maximum number of times.");

        if (MinimumOrderAmount is not null && subtotal.Amount < MinimumOrderAmount.Amount)
            throw new DomainException(
                $"Coupon '{Code}' requires a minimum order of {MinimumOrderAmount}. " +
                $"Your subtotal is {subtotal}.");
    }

    public void RecordUsage()
    {
        CurrentUsageCount++;
        if (MaxUsageCount.HasValue && CurrentUsageCount >= MaxUsageCount.Value)
            IsActive = false; // Auto-deactivate when exhausted
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}