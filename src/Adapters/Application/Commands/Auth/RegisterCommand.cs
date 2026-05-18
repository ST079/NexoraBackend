using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Application.Interfaces.Services;
using NexoraBackend.Common.Models;
using NexoraBackend.Config;
using NexoraBackend.Core.Domain;
using NexoraBackend.Core.ValueObjects;

namespace NexoraBackend.Application.Commands.Auth;


public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password
) : IRequest<Result<AuthResponseDto>>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("Password must have an uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must have a lowercase letter.")
            .Matches(@"\d").WithMessage("Password must have a digit.")
            .Matches(@"[!@#$%^&*]").WithMessage("Password must have a special character.");
    }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly JwtSettings _jwtSettings;

    public RegisterCommandHandler(IUnitOfWork unitOfWork, IAuthService authService,
        IEmailService emailService, IMapper mapper, IOptions<JwtSettings> jwtOptions)
    {
        _unitOfWork = unitOfWork;
        _authService = authService;
        _emailService = emailService;
        _mapper = mapper;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<Result<AuthResponseDto>> Handle(RegisterCommand cmd, CancellationToken ct)
    {
        if (await _unitOfWork.Users.EmailExistsAsync(cmd.Email, ct))
            return Result<AuthResponseDto>.Failure(Error.Conflict("Email already in use."));

        var hash = _authService.HashPassword(cmd.Password);
        
        var user = User.Create(cmd.FirstName, cmd.LastName, new Email(cmd.Email), hash);

        var accessToken = _authService.GenerateAccessToken(user.Id, user.Email);
        var refreshToken = _authService.GenerateRefreshToken();
        
        user.UpdateRefreshToken(refreshToken,
            DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays));

        await _unitOfWork.Users.AddAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // Fire and forget email
        _ = _emailService.SendEmailVerificationAsync(
            user.Email.Value, user.FirstName, user.EmailVerificationToken!, ct);

        return Result<AuthResponseDto>.Success(new AuthResponseDto(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
            _mapper.Map<UserDto>(user)));
    }
}