using FluentValidation;
using NexoraBackend.Application.DTOs.Inputs.Users;

namespace NexoraBackend.Application.Validators.Users;


public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Name).MinimumLength(2)
                            .WithMessage("Name must be at least 2 characters.");

        RuleFor(x => x.PhoneNumber).Matches(@"^\+?[1-9]\d{9}$")
                            .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                            .WithMessage("Invalid phone number format.");
        RuleFor(x => x.Street).MaximumLength(50)
                            .WithMessage("Street cannot exceed 50 characters.");
        RuleFor(x => x.City).MaximumLength(50)
                            .WithMessage("City cannot exceed 50 characters.");
        RuleFor(x => x.Country).MaximumLength(50)
                            .WithMessage("Country cannot exceed 50 characters.");
    }
}