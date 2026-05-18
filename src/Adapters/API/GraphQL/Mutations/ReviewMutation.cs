using HotChocolate.Authorization;
using MediatR;
using NexoraBackend.API.Extensions;
using NexoraBackend.API.GraphQL.Types;
using NexoraBackend.Application.Commands.Reviews;

namespace NexoraBackend.API.GraphQL.Mutations;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class ReviewMutations
{
    [Authorize]
    public async Task<ReviewPayload> SubmitReview(
        SubmitReviewInput input, [Service] IMediator mediator,
        [Service] IHttpContextAccessor httpContext, CancellationToken ct = default)
    {
        var userId = httpContext.HttpContext!.GetUserId();
        var result = await mediator.Send(
            new SubmitReviewCommand(userId, input.ProductId, input.Rating, input.Title, input.Body), ct);
        return result.Match(
            onSuccess: dto => new ReviewPayload { Data = dto },
            onFailure: err => new ReviewPayload { Error = err.Message });
    }

    [Authorize(Roles = new[] { "Admin", "SuperAdmin" })]
    public async Task<ReviewPayload> ModerateReview(
        ModerateReviewInput input, [Service] IMediator mediator, CancellationToken ct = default)
    {
        var result = await mediator.Send(
            new ModerateReviewCommand(input.ReviewId, input.Approve, input.RejectionReason), ct);
        return result.Match(
            onSuccess: dto => new ReviewPayload { Data = dto },
            onFailure: err => new ReviewPayload { Error = err.Message });
    }
}
