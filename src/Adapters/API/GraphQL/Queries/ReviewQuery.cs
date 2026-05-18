using HotChocolate.Authorization;
using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Queries.Reviews;

namespace NexoraBackend.API.GraphQL.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
public class ReviewQueries
{
    [UseOffsetPaging]
    public async Task<IEnumerable<ReviewSummaryDto>> GetProductReviews(
        Guid productId, [Service] IMediator mediator,
        int page = 1, int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetProductReviewsQuery(productId, page, pageSize), ct);
        return result.IsSuccess ? result.Value!.Items : [];
    }

    [Authorize(Roles = new[] { "Admin", "SuperAdmin" })]
    [UseOffsetPaging]
    public async Task<IEnumerable<ReviewDto>> GetPendingReviews(
        [Service] IMediator mediator,
        int page = 1, int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetPendingReviewsQuery(page, pageSize), ct);
        return result.IsSuccess ? result.Value!.Items : [];
    }
}