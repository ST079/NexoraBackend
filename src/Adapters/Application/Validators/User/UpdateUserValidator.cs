using FluentValidation;
using NexoraBackend.Application.DTOs.Inputs.Users;

namespace NexoraBackend.Application.Validators.Users;


public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Name).MinimumLength(2)
                            .WithMessage("Name must be at least 2 characters.");

        RuleFor(x => x.Phone).Matches(@"^\+?[1-9]\d{9}$")
                            .When(x => !string.IsNullOrEmpty(x.Phone))
                            .WithMessage("Invalid phone number format.");
        RuleFor(x => x.Address).MaximumLength(200)
                            .WithMessage("Address cannot exceed 200 characters.");
    }
}