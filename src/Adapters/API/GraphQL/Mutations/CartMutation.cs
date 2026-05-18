using HotChocolate.Authorization;
using MediatR;
using NexoraBackend.API.Extensions;
using NexoraBackend.API.GraphQL.Types;
using NexoraBackend.Application.Commands.Carts;

namespace NexoraBackend.API.GraphQL.Mutations;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class CartMutations
{
    [Authorize]
    public async Task<CartPayload> AddToCart(
        AddToCartInput input, [Service] IMediator mediator,
        [Service] IHttpContextAccessor httpContext, CancellationToken ct = default)
    {
        var userId = httpContext.HttpContext!.GetUserId();
        var result = await mediator.Send(new AddToCartCommand(userId, input.ProductId, input.Quantity), ct);
        return result.Match(
            onSuccess: dto => new CartPayload { Data = dto },
            onFailure: err => new CartPayload { Error = err.Message });
    }

    [Authorize]
    public async Task<CartPayload> UpdateCartItem(
        UpdateCartItemInput input, [Service] IMediator mediator,
        [Service] IHttpContextAccessor httpContext, CancellationToken ct = default)
    {
        var userId = httpContext.HttpContext!.GetUserId();
        var result = await mediator.Send(new UpdateCartItemCommand(userId, input.ProductId, input.Quantity), ct);
        return result.Match(
            onSuccess: dto => new CartPayload { Data = dto },
            onFailure: err => new CartPayload { Error = err.Message });
    }

    [Authorize]
    public async Task<CartPayload> RemoveCartItem(
        Guid productId, [Service] IMediator mediator,
        [Service] IHttpContextAccessor httpContext, CancellationToken ct = default)
    {
        var userId = httpContext.HttpContext!.GetUserId();
        var result = await mediator.Send(new RemoveCartItemCommand(userId, productId), ct);
        return result.Match(
            onSuccess: dto => new CartPayload { Data = dto },
            onFailure: err => new CartPayload { Error = err.Message });
    }

    [Authorize]
    public async Task<bool> ClearCart(
        [Service] IMediator mediator,
        [Service] IHttpContextAccessor httpContext, CancellationToken ct = default)
    {
        var userId = httpContext.HttpContext!.GetUserId();
        var result = await mediator.Send(new ClearCartCommand(userId), ct);
        return result.IsSuccess;
    }
}