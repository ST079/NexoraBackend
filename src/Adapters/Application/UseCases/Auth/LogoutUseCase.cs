using NexoraBackend.Common.Exceptions;
using NexoraBackend.Core.Domain.Entities;
using NexoraBackend.Core.Domain.Ports;

namespace NexoraBackend.Application.UseCases.Auth;

public class LogoutUseCase
{
    public readonly IUnitOfWork _unitOfWork;
    public readonly IRefreshTokenRepository _refreshTokenRepository;
    public readonly IAuditLogRepository _auditLogRepository;
    public LogoutUseCase(IRefreshTokenRepository refreshTokenRepository, IAuditLogRepository auditLogRepository, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _refreshTokenRepository = refreshTokenRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<bool> Execute(string refreshToken)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(Uri.UnescapeDataString(refreshToken));
        if (token == null)
            throw new NotFoundException("Refresh Token Not Found,");

        if (token.IsRevoked)
            throw new CustomException("Already LoggedOut");

        await _refreshTokenRepository.RevokeAsync(token.Token);
        await _unitOfWork.SaveChangesAsync();
        await _auditLogRepository.AddAsync(new AuditLog
        {
            UserId = token.UserId,
            Action = "LOGOUT",
            Details = "User logged out"
        });
        return true;
    }
}