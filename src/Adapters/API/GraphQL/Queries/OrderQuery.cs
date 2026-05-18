using HotChocolate.Authorization;
using MediatR;
using NexoraBackend.API.Extensions;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Queries.Orders;
using NexoraBackend.Core.Enums;

namespace NexoraBackend.API.GraphQL.Queries;

[QueryType]
public class OrderQueries
{
    [Authorize]
    public async Task<OrderDto?> GetOrder(
        Guid id,
        [Service] IMediator mediator,
        [Service] IHttpContextAccessor httpContext,
        CancellationToken ct = default)
    {
        var userId = httpContext.HttpContext!.GetUserId();
        var result = await mediator.Send(new GetOrderQuery(id, userId), ct);
        return result.IsSuccess ? result.Value : null;
    }

    [Authorize]
    [UsePaging]
    public async Task<IEnumerable<OrderDto>> GetMyOrders(
        [Service] IMediator mediator,
        [Service] IHttpContextAccessor httpContext,
        int page = 1, int pageSize = 10,
        CancellationToken ct = default)
    {
        var userId = httpContext.HttpContext!.GetUserId();
        var result = await mediator.Send(new GetUserOrdersQuery(userId, page, pageSize), ct);
        return result.IsSuccess ? result.Value!.Items : [];
    }

    [Authorize(Roles = new[] { "Admin", "SuperAdmin" })]
    [UsePaging]
    public async Task<IEnumerable<OrderDto>> GetAllOrders(
        [Service] IMediator mediator,
        int page = 1, int pageSize = 20,
        string? status = null,
        CancellationToken ct = default)
    {
        var orderStatus = status != null ? Enum.Parse<OrderStatus>(status) : (OrderStatus?)null;
        var result = await mediator.Send(new GetAllOrdersQuery(page, pageSize, orderStatus), ct);
        return result.IsSuccess ? result.Value!.Items : [];
    }
}