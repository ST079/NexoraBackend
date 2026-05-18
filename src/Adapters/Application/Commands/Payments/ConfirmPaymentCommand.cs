// src/Adapters/ECommerce.Application/Commands/Payments/ConfirmPaymentCommand.cs
using MediatR;
using NexoraBackend.Application.Behaviors;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;

namespace NexoraBackend.Application.Commands.Payments;
/// <summary>
/// Called from the Stripe webhook handler after verifying the stripe-signature header.
/// Never called from the frontend — only from the verified server-to-server webhook.
/// </summary>
public record ConfirmPaymentCommand(string StripePaymentIntentId, string StripeChargeId)
    : IRequest<Result>, ITransactionalCommand;

public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _mediator;

    public ConfirmPaymentCommandHandler(IUnitOfWork uow, IMediator mediator)
    { _uow = uow; _mediator = mediator; }

    public async Task<Result> Handle(ConfirmPaymentCommand cmd, CancellationToken ct)
    {
        var payment = await _uow.Payments.GetByStripeIntentIdAsync(cmd.StripePaymentIntentId, ct);
        if (payment is null) return Result.Failure(Error.NotFound("Payment", cmd.StripePaymentIntentId));

        var order = await _uow.Orders.GetWithItemsAsync(payment.OrderId, ct);
        if (order is null) return Result.Failure(Error.NotFound("Order", payment.OrderId));

        payment.Complete(cmd.StripeChargeId);
        order.MarkAsPaid(payment);

        _uow.Payments.Update(payment);
        _uow.Orders.Update(order);
        await _uow.SaveChangesAsync(ct);

        foreach (var de in order.DomainEvents) await _mediator.Publish(de, ct);
        order.ClearDomainEvents();

        return Result.Success();
    }
}