using FluentValidation;
using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Exceptions;
using NexoraBackend.Core.ValueObjects;

namespace NexoraBackend.Application.Commands.Coupons;


/// <summary>
/// Validates a coupon and returns the discount preview — does NOT commit anything.
/// The actual discount is passed into PlaceOrderCommand. Separation keeps checkout UX smooth.
/// </summary>
public record ApplyCouponCommand(Guid UserId, string CouponCode)
    : IRequest<Result<AppliedCouponDto>>;

public class ApplyCouponCommandValidator : AbstractValidator<ApplyCouponCommand>
{
    public ApplyCouponCommandValidator()
    {
        RuleFor(x => x.CouponCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.UserId).NotEmpty();
    }
}

public class ApplyCouponCommandHandler : IRequestHandler<ApplyCouponCommand, Result<AppliedCouponDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public ApplyCouponCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<AppliedCouponDto>> Handle(ApplyCouponCommand cmd, CancellationToken ct)
    {
        var coupon = await _unitOfWork.Coupons.GetByCodeAsync(cmd.CouponCode.ToUpperInvariant(), ct);
        if (coupon is null) return Result<AppliedCouponDto>.Failure(Error.NotFound("Coupon", cmd.CouponCode));

        var cart = await _unitOfWork.Carts.GetByUserAsync(cmd.UserId, ct);
        if (cart is null || !cart.Items.Any())
            return Result<AppliedCouponDto>.Failure(Error.Validation("Your cart is empty."));

        var userUsageCount = await _unitOfWork.Coupons.GetUserUsageCountAsync(cmd.CouponCode, cmd.UserId, ct);

        Money discountAmount;
        try { discountAmount = coupon.CalculateDiscount(cart.Total, userUsageCount); }
        catch (DomainException ex) { return Result<AppliedCouponDto>.Failure(Error.Validation(ex.Message)); }

        return Result<AppliedCouponDto>.Success(new AppliedCouponDto(
            coupon.Code, coupon.Description, coupon.Type.ToString(),
            discountAmount.Amount, discountAmount.Currency));
    }
}