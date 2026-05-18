using HotChocolate.Authorization;
using MediatR;
using NexoraBackend.API.Extensions;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Queries.Users;

namespace NexoraBackend.API.GraphQL.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
public class UserQueries
{
    [Authorize]
    public async Task<UserDto?> GetMe(
        [Service] IMediator mediator,
        [Service] IHttpContextAccessor httpContext,
        CancellationToken ct = default)
    {
        var userId = httpContext.HttpContext!.GetUserId();
        var result = await mediator.Send(new GetUserQuery(userId), ct);
        return result.IsSuccess ? result.Value : null;
    }

    [Authorize(Roles = new[] { "Admin", "SuperAdmin" })]
    [UseOffsetPaging]
    public async Task<IEnumerable<UserDto>> GetAllUsers(
        [Service] IMediator mediator,
        int page = 1, int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAllUsersQuery(page, pageSize), ct);
        return result.IsSuccess ? result.Value!.Items : [];
    }
}