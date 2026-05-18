// src/Adapters/NexoraBackend.Application/Commands/Payments/CreatePaymentIntentCommand.cs
using MediatR;
using NexoraBackend.Application.Behaviors;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Application.Interfaces.Services;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Domain;
using NexoraBackend.Core.Enums;

namespace NexoraBackend.Application.Commands.Payments;

/// <summary>
/// Creates a Stripe PaymentIntent. Returns client_secret to the frontend.
/// Card data goes directly to Stripe — never touches our server (PCI-DSS compliant).
/// </summary>
public record CreatePaymentIntentCommand(Guid OrderId, Guid UserId, string PaymentMethod = "card")
    : IRequest<Result<CreatePaymentIntentDto>>, ITransactionalCommand;


public class CreatePaymentIntentCommandHandler
    : IRequestHandler<CreatePaymentIntentCommand, Result<CreatePaymentIntentDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IPaymentService _paymentService;

    public CreatePaymentIntentCommandHandler(IUnitOfWork uow, IPaymentService paymentService)
    { _uow = uow; _paymentService = paymentService; }

    public async Task<Result<CreatePaymentIntentDto>> Handle(
        CreatePaymentIntentCommand cmd, CancellationToken ct)
    {
        var order = await _uow.Orders.GetWithItemsAsync(cmd.OrderId, ct);
        if (order is null) return Result<CreatePaymentIntentDto>.Failure(Error.NotFound("Order", cmd.OrderId));
        if (order.UserId != cmd.UserId) return Result<CreatePaymentIntentDto>.Failure(Error.Unauthorized());
        if (order.Status != OrderStatus.Confirmed)
            return Result<CreatePaymentIntentDto>.Failure(
                Error.Validation($"Order must be Confirmed to initiate payment. Status: {order.Status}"));
        if (order.Payment != null)
            return Result<CreatePaymentIntentDto>.Failure(
                Error.Conflict("A payment has already been initiated for this order."));

        var (intentIdObj, clientSecretObj) = await _paymentService.CreatePaymentIntentAsync(order.Total, cmd.OrderId, ct);

        var intentId = intentIdObj?.ToString()?? throw new Exception("Stripe PaymentIntent ID is null");
        var clientSecret = clientSecretObj?.ToString()?? throw new Exception("Stripe Client Secret is null");

        var payment = Payment.Create(cmd.OrderId, order.Total, cmd.PaymentMethod);
        payment.SetStripeIntent(intentId);

        await _uow.Payments.AddAsync(payment, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<CreatePaymentIntentDto>.Success(new CreatePaymentIntentDto(
            intentId, clientSecret, order.Total.Amount, order.Total.Currency));
    }
}