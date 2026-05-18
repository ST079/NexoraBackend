using FluentValidation;
using MediatR;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Exceptions;

namespace NexoraBackend.Application.Commands.Auth;

public record VerifyEmailCommand(string Token) : IRequest<Result>;

public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator() =>
        RuleFor(x => x.Token).NotEmpty().MaximumLength(128);
}

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public VerifyEmailCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(VerifyEmailCommand cmd, CancellationToken ct)
    {
        // Use the dedicated index-backed method — not FindOneAsync — so EF Core
        // generates a WHERE clause it can translate and execute efficiently.
        var user = await _uow.Users.GetByEmailVerificationTokenAsync(cmd.Token, ct);

        if (user is null)
            return Result.Failure(new Error("INVALID_TOKEN", "Verification token is invalid or already used."));

        try { 
            user.VerifyEmail(); 
            }
        catch (DomainException ex)
        { 
            return Result.Failure(Error.Validation(ex.Message)); 
            }

        _uow.Users.Update(user);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}