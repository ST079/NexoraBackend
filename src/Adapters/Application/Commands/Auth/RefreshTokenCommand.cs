using MediatR;
using AutoMapper;
using Microsoft.Extensions.Options;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Application.Interfaces.Services;
using NexoraBackend.Common.Models;
using NexoraBackend.Config;

namespace NexoraBackend.Application.Commands.Auth;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponseDto>>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;
    private readonly JwtSettings _jwtSettings;

    public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, IAuthService authService,
        IMapper mapper, IOptions<JwtSettings> jwtOptions)
    {
        _unitOfWork = unitOfWork; _authService = authService;
        _mapper = mapper; _jwtSettings = jwtOptions.Value;
    }

    public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand cmd, CancellationToken ct)
    {
        var user = await _unitOfWork.Users.GetByRefreshTokenAsync(cmd.RefreshToken, ct);

        if (user is null || user.RefreshTokenExpiry < DateTime.UtcNow)
            return Result<AuthResponseDto>.Failure(
                new Error("INVALID_REFRESH_TOKEN", "Refresh token is invalid or expired."));

        var newAccessToken  = _authService.GenerateAccessToken(user.Id, user.Email);
        var newRefreshToken = _authService.GenerateRefreshToken();

        // Rotate refresh token on every use — old token is immediately invalidated
        user.UpdateRefreshToken(newRefreshToken,
            DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays));

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<AuthResponseDto>.Success(new AuthResponseDto(
            newAccessToken, newRefreshToken,
            DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
            _mapper.Map<UserDto>(user)));
    }
}