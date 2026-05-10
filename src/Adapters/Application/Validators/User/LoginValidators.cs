using FluentValidation;
using NexoraBackend.Application.DTOs.Inputs.Users;

namespace NexoraBackend.Application.Validators.Users;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}