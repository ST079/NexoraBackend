using AutoMapper;
using FluentValidation;
using MediatR;
using NexoraBackend.Application.Behaviors;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Exceptions;


namespace NexoraBackend.Application.Commands.Orders;    


public record CancelOrderCommand(Guid OrderId, Guid UserId, string Reason)
    : IRequest<Result<OrderDto>>, ITransactionalCommand;

public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public CancelOrderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator)
    { _unitOfWork = unitOfWork; _mapper = mapper; _mediator = mediator; }

    public async Task<Result<OrderDto>> Handle(CancelOrderCommand cmd, CancellationToken ct)
    {
        var order = await _unitOfWork.Orders.GetWithItemsAsync(cmd.OrderId, ct);
        if (order is null)
            return Result<OrderDto>.Failure(Error.NotFound("Order", cmd.OrderId));

        if (order.UserId != cmd.UserId)
            return Result<OrderDto>.Failure(Error.Unauthorized());

        try { order.Cancel(cmd.Reason); }
        catch (InvalidOrderStateException ex)
        { return Result<OrderDto>.Failure(Error.Validation(ex.Message)); }

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync(ct);

        // Dispatch OrderCancelledEvent for each item so stock is released
        foreach (var domainEvent in order.DomainEvents)
            await _mediator.Publish(domainEvent, ct);
        order.ClearDomainEvents();

        var updated = await _unitOfWork.Orders.GetWithItemsAsync(order.Id, ct);
        return Result<OrderDto>.Success(_mapper.Map<OrderDto>(updated!));
    }
}
