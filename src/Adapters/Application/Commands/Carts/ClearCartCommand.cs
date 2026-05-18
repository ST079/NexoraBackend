using MediatR;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;

namespace NexoraBackend.Application.Commands.Carts;

public record ClearCartCommand(Guid UserId) : IRequest<Result>;

public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    public ClearCartCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result> Handle(ClearCartCommand cmd, CancellationToken ct)
    {
        var cart = await _unitOfWork.Carts.GetByUserAsync(cmd.UserId, ct);
        if (cart is null) return Result.Success(); // idempotent
        cart.Clear();
        _unitOfWork.Carts.Update(cart);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}