using AutoMapper;
using FluentValidation;
using MediatR;
using NexoraBackend.Application.Behaviors;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Domain;

namespace NexoraBackend.Application.Commands.Carts;


public record AddToCartCommand(Guid UserId, Guid ProductId, int Quantity)
    : IRequest<Result<CartDto>>, ITransactionalCommand;

public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0).LessThanOrEqualTo(100);
    }
}

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Result<CartDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddToCartCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) { _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<Result<CartDto>> Handle(AddToCartCommand cmd, CancellationToken ct)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(cmd.ProductId, ct);
        if (product is null) return Result<CartDto>.Failure(Error.NotFound("Product", cmd.ProductId));
        if (!product.IsActive) return Result<CartDto>.Failure(Error.Validation("Product is not available."));
        if (product.StockQuantity < cmd.Quantity)
            return Result<CartDto>.Failure(Error.Validation($"Only {product.StockQuantity} unit(s) available."));

        var cart = await _unitOfWork.Carts.GetByUserAsync(cmd.UserId, ct);
        if (cart is null) { cart = Cart.Create(cmd.UserId); await _unitOfWork.Carts.AddAsync(cart, ct); }

        cart.AddItem(product, cmd.Quantity);
        _unitOfWork.Carts.Update(cart);
        await _unitOfWork.SaveChangesAsync(ct);

        var updated = await _unitOfWork.Carts.GetByUserAsync(cmd.UserId, ct);
        return Result<CartDto>.Success(_mapper.Map<CartDto>(updated!));
    }
}