using HotChocolate.Authorization;
using MediatR;
using NexoraBackend.API.Extensions;
using NexoraBackend.API.GraphQL.Types;
using NexoraBackend.Application.Commands.Coupons;

namespace NexoraBackend.API.GraphQL.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
public class CouponQueries
{
    /// <summary>
    /// Validates a coupon code against the current cart and returns
    /// the discount preview — does NOT commit anything.
    /// </summary>
    [Authorize]
    public async Task<AppliedCouponPayload> ValidateCoupon(
        string couponCode, [Service] IMediator mediator,
        [Service] IHttpContextAccessor httpContext,
        CancellationToken ct = default)
    {
        var userId = httpContext.HttpContext!.GetUserId();
        var result = await mediator.Send(new ApplyCouponCommand(userId, couponCode), ct);
        return result.Match(
            onSuccess: dto => new AppliedCouponPayload { Data = dto },
            onFailure: err => new AppliedCouponPayload { Error = err.Message });
    }
}