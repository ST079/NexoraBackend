using AutoMapper;
using FluentValidation;
using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Domain;
using NexoraBackend.Core.Enums;

namespace NexoraBackend.Application.Commands.Coupons;

public record CreateCouponCommand(
    string Code, string Description, string Type, decimal DiscountValue,
    DateTime? ExpiresAt, int? MaxUsageCount, int? MaxUsagePerUser,
    decimal? MinimumOrderAmount, string Currency = "USD"
) : IRequest<Result<CouponDto>>;

public class CreateCouponCommandValidator : AbstractValidator<CreateCouponCommand>
{
    public CreateCouponCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50)
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("Code must be uppercase alphanumeric.");
        RuleFor(x => x.Type).Must(t => t is "Percentage" or "Fixed")
            .WithMessage("Type must be 'Percentage' or 'Fixed'.");
        RuleFor(x => x.DiscountValue).GreaterThan(0);
        RuleFor(x => x.ExpiresAt).GreaterThan(DateTime.UtcNow).When(x => x.ExpiresAt.HasValue);
    }
}

public class CreateCouponCommandHandler : IRequestHandler<CreateCouponCommand, Result<CouponDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public CreateCouponCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) { _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<Result<CouponDto>> Handle(CreateCouponCommand cmd, CancellationToken ct)
    {
        if (await _unitOfWork.Coupons.CodeExistsAsync(cmd.Code.ToUpperInvariant(), ct))
            return Result<CouponDto>.Failure(Error.Conflict($"Coupon code '{cmd.Code}' already exists."));

        var coupon = Coupon.Create(cmd.Code, cmd.Description, Enum.Parse<CouponType>(cmd.Type),
            cmd.DiscountValue, cmd.ExpiresAt, cmd.MaxUsageCount, cmd.MaxUsagePerUser,
            cmd.MinimumOrderAmount, cmd.Currency);

        await _unitOfWork.Coupons.AddAsync(coupon, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<CouponDto>.Success(_mapper.Map<CouponDto>(coupon));
    }
}