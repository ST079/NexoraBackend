

using Microsoft.EntityFrameworkCore;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Core.Domain;
using NexoraBackend.Infrastructure.Persistence;
using NexoraBackend.Infrastructure.Persistence.Repositories;

namespace NexoraBackend.Infrastructure.Persistence.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        await _dbSet
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Email.Value == email.ToLowerInvariant(), ct);

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default) =>
        await _dbSet.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, ct);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        await _dbSet.AnyAsync(u => u.Email.Value == email.ToLowerInvariant(), ct);

    public async Task<User?> GetWithAddressesAsync(Guid id, CancellationToken ct = default) =>
        await _dbSet.Include(u => u.Addresses).FirstOrDefaultAsync(u => u.Id == id, ct);


    /// Token is on a non-null row for unverified users — the global soft-delete filter is fine
    /// because unverified users are live records (IsDeleted = false).
    public async Task<User?> GetByEmailVerificationTokenAsync(
        string token, CancellationToken ct = default) =>
        await _dbSet.FirstOrDefaultAsync(
            u => u.EmailVerificationToken == token, ct);

    /// PasswordResetToken is cleared after use by User.ChangePassword().
    /// Expiry is checked by the command handler — not here — to keep the repository
    /// free of business logic.
    public async Task<User?> GetByPasswordResetTokenAsync(
        string token, CancellationToken ct = default) =>
        await _dbSet.FirstOrDefaultAsync(
            u => u.PasswordResetToken == token, ct);
}