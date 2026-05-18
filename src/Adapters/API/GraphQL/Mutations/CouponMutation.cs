// src/Adapters/ECommerce.API/GraphQL/Mutations/CouponMutations.cs
using HotChocolate.Authorization;
using MediatR;
using NexoraBackend.API.GraphQL.Types;
using NexoraBackend.Application.Commands.Coupons;

namespace NexoraBackend.API.GraphQL.Mutations;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class CouponMutations
{
    [Authorize(Roles = new[] { "Admin"})]
    public async Task<CouponPayload> CreateCoupon(
        CreateCouponInput input, [Service] IMediator mediator, CancellationToken ct = default)
    {
        var result = await mediator.Send(new CreateCouponCommand(
            input.Code, input.Description, input.Type, input.DiscountValue,
            input.ExpiresAt, input.MaxUsageCount, input.MaxUsagePerUser,
            input.MinimumOrderAmount, input.Currency), ct);
        return result.Match(
            onSuccess: dto => new CouponPayload { Data = dto },
            onFailure: err => new CouponPayload { Error = err.Message });
    }
}
