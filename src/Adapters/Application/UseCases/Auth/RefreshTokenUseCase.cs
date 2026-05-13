using System.Security.Claims;
using NexoraBackend.Application.DTOs.Responses.Auth;
using NexoraBackend.Common.Exceptions;
using NexoraBackend.Core.Domain.Entities;
using NexoraBackend.Core.Domain.Ports;

namespace NexoraBackend.Application.UseCases.Auth;

public class RefreshTokenUseCase
{
    public readonly IRefreshTokenRepository _refreshTokenRepository;
    public readonly ITokenService _tokenService;
    public readonly IUserRepository _userRepository;
    public readonly IUnitOfWork _unitOfWork;

    public RefreshTokenUseCase(IUnitOfWork unitOfWork, IRefreshTokenRepository refreshTokenRepository, ITokenService tokenService, IUserRepository userRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponseDto> Execute(string accessToken, string refreshToken)
    {
        //validate from DB
        var storedRefreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (storedRefreshToken == null)
            throw new UnauthorizedAccessException("Invalid refresh Token");

        if (storedRefreshToken.IsRevoked)
            throw new UnauthorizedAccessException("Refresh Token Revoked");

        if (storedRefreshToken.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh Token Expired");


        //extra user
        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);

        if (principal == null)
            throw new UnauthorizedAccessException("Invalid access token.");

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            throw new UnauthorizedAccessException("Invalid token claims");

        var user = await _userRepository.GetUserByIdAsync(Guid.Parse(userId));

        if (user == null)
            throw new NotFoundException("User Not Found");


        // Generate New Token 
        var newAccessToken = _tokenService.GenerateAccessToken(user);

        storedRefreshToken.IsRevoked = true;
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        await _refreshTokenRepository.CreateAsync(new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(2),
            IsRevoked = false
        });

        await _unitOfWork.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };

    }
}
