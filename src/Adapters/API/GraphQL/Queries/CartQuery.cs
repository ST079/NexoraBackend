using HotChocolate.Authorization;
using MediatR;
using NexoraBackend.API.Extensions;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Queries.Cart;

namespace NexoraBackend.API.GraphQL.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
public class CartQueries
{
    [Authorize]
    public async Task<CartDto?> GetMyCart(
        [Service] IMediator mediator,
        [Service] IHttpContextAccessor httpContext,
        CancellationToken ct = default)
    {
        var userId = httpContext.HttpContext!.GetUserId();
        var result = await mediator.Send(new GetCartQuery(userId), ct);
        return result.IsSuccess ? result.Value : null;
    }
}