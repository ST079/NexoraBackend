using HotChocolate.Authorization;
using MediatR;
using NexoraBackend.API.GraphQL.Types;
using NexoraBackend.Application.Commands.Auth;

namespace NexoraBackend.API.GraphQL.Mutations; 

[ExtendObjectType(OperationTypeNames.Mutation)]
public class AuthMutations
{
    public async Task<AuthPayload> Register(
        RegisterInput input,
        [Service] IMediator mediator,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(
            new RegisterCommand(input.FirstName, input.LastName, input.Email, input.Password), ct);

        return result.Match(
            onSuccess: dto => new AuthPayload { Data = dto },
            onFailure: err => new AuthPayload { Error = err.Message });
    }

    public async Task<AuthPayload> Login(
        LoginInput input,
        [Service] IMediator mediator,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new LoginCommand(input.Email, input.Password), ct);
        return result.Match(
            onSuccess: dto => new AuthPayload { Data = dto },
            onFailure: err => new AuthPayload { Error = err.Message });
    }

    [Authorize]
    public async Task<RefreshPayload> RefreshToken(
        string refreshToken,
        [Service] IMediator mediator,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new RefreshTokenCommand(refreshToken), ct);
        return result.Match(
            onSuccess: dto => new RefreshPayload { Data = dto },
            onFailure: err => new RefreshPayload { Error = err.Message });
    }

    public async Task<BooleanPayload> VerifyEmail(
    string token, [Service] IMediator mediator, CancellationToken ct = default)
{
    var result = await mediator.Send(new VerifyEmailCommand(token), ct);
    return new BooleanPayload { Data = result.IsSuccess, Error = result.Error?.Message };
}
}