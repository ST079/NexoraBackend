using AutoMapper;
using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Exceptions;

namespace NexoraBackend.Application.Commands.Carts;

public record RemoveCartItemCommand(Guid UserId, Guid ProductId) : IRequest<Result<CartDto>>;

public class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand, Result<CartDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RemoveCartItemCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) { _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<Result<CartDto>> Handle(RemoveCartItemCommand cmd, CancellationToken ct)
    {
        var cart = await _unitOfWork.Carts.GetByUserAsync(cmd.UserId, ct);
        if (cart is null) return Result<CartDto>.Failure(Error.NotFound("Cart", cmd.UserId));

        try { cart.RemoveItem(cmd.ProductId); }
        catch (DomainException ex) { return Result<CartDto>.Failure(Error.Validation(ex.Message)); }

        _unitOfWork.Carts.Update(cart);
        await _unitOfWork.SaveChangesAsync(ct);

        var updated = await _unitOfWork.Carts.GetByUserAsync(cmd.UserId, ct);
        return Result<CartDto>.Success(_mapper.Map<CartDto>(updated!));
    }
}