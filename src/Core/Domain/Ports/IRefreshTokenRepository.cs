using NexoraBackend.Core.Domain.Entities;

namespace NexoraBackend.Core.Domain.Ports;

public interface IRefreshTokenRepository
{
    Task CreateAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<bool> RevokeAsync(string token);
}