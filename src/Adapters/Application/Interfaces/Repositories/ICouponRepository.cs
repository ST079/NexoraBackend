
using NexoraBackend.Core.Domain;

namespace NexoraBackend.Application.Interfaces.Repositories;

public interface ICouponRepository : IRepository<Coupon>
{
    Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
    /// Returns the number of times a specific user has used a given coupon.
    /// Used to enforce per-user usage limits.
    Task<int> GetUserUsageCountAsync(string couponCode, Guid userId, CancellationToken ct = default);
}