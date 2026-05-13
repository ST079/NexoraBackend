using NexoraBackend.Common.Exceptions;
using NexoraBackend.Core.Domain.Ports;

namespace NexoraBackend.Application.UseCases.Auth;

public class LogoutUseCase
{
    public readonly IUnitOfWork _unitOfWork;
    public readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutUseCase(IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<bool> Execute(string refreshToken)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        if(token == null)
        throw new NotFoundException("Refresh Token Not Found,");

        if(token.IsRevoked)
        throw new Exception("Already LoggedOut");

        await _refreshTokenRepository.RevokeAsync(token.Token);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}