using NexoraBackend.Core.Domain;

namespace NexoraBackend.Application.Interfaces.Repositories;


public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<User?> GetWithAddressesAsync(Guid id, CancellationToken ct = default);

    ///Used by VerifyEmailCommand. Token is single-use; cleared after verification.
    Task<User?> GetByEmailVerificationTokenAsync(string token, CancellationToken ct = default);

    ///Used by ResetPasswordCommand. Token expires after 1 hour
    Task<User?> GetByPasswordResetTokenAsync(string token, CancellationToken ct = default);

}
