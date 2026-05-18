using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Application.Interfaces.Services;
using NexoraBackend.Common.Models;
using NexoraBackend.Config;


namespace NexoraBackend.Application.Commands.Auth;

public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponseDto>>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;
    private readonly JwtSettings _jwtSettings;

    public LoginCommandHandler(IUnitOfWork unitOfWork, IAuthService authService,
        IMapper mapper, IOptions<JwtSettings> jwtOptions)
    {
        _unitOfWork = unitOfWork;
        _authService = authService;
        _mapper = mapper;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<Result<AuthResponseDto>> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(cmd.Email, ct);
        if (user is null || !_authService.VerifyPassword(cmd.Password, user.PasswordHash))
            return Result<AuthResponseDto>.Failure(Error.Unauthorized());

        if (!user.IsEmailVerified)
            return Result<AuthResponseDto>.Failure(
                new Error("EMAIL_NOT_VERIFIED", "Please verify your email before logging in."));

        var accessToken = _authService.GenerateAccessToken(user.Id, user.Email);
        var refreshToken = _authService.GenerateRefreshToken();
        user.UpdateRefreshToken(refreshToken,
            DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays));

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<AuthResponseDto>.Success(new AuthResponseDto(
            accessToken, refreshToken,
            DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
            _mapper.Map<UserDto>(user)));
    }
}
