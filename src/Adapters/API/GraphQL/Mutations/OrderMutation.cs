using HotChocolate.Authorization;
using MediatR;
using NexoraBackend.API.Extensions;
using NexoraBackend.API.GraphQL.Types;
using NexoraBackend.Application.Commands.Carts;
using NexoraBackend.Application.Commands.Orders;
using NexoraBackend.Application.DTOs;

namespace NexoraBackend.API.GraphQL.Mutations;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class OrderMutations
{
    [Authorize]
    public async Task<OrderPayload> PlaceOrder(
        PlaceOrderInput input,
        [Service] IMediator mediator,
        [Service] IHttpContextAccessor httpContext,
        CancellationToken ct = default)
    {
        var userId = httpContext.HttpContext!.GetUserId();
        var result = await mediator.Send(new PlaceOrderCommand(
            userId,
            new AddressDto(input.Line1, input.Line2, input.City,
                input.State, input.PostalCode, input.Country)), ct);

        return result.Match(
            onSuccess: dto => new OrderPayload { Data = dto },
            onFailure: err => new OrderPayload { Error = err.Message });
    }

    [Authorize]
    public async Task<OrderPayload> CancelOrder(
        Guid orderId, string reason,
        [Service] IMediator mediator,
        [Service] IHttpContextAccessor httpContext,
        CancellationToken ct = default)
    {
        var userId = httpContext.HttpContext!.GetUserId();
        var result = await mediator.Send(new CancelOrderCommand(orderId, userId, reason), ct);
        return result.Match(
            onSuccess: dto => new OrderPayload { Data = dto },
            onFailure: err => new OrderPayload { Error = err.Message });
    }

    [Authorize]
    public async Task<CartPayload> AddToCart(
        Guid productId, int quantity,
        [Service] IMediator mediator,
        [Service] IHttpContextAccessor httpContext,
        CancellationToken ct = default)
    {
        var userId = httpContext.HttpContext!.GetUserId();
        var result = await mediator.Send(new AddToCartCommand(userId, productId, quantity), ct);
        return result.Match(
            onSuccess: dto => new CartPayload { Data = dto },
            onFailure: err => new CartPayload { Error = err.Message });
    }
}