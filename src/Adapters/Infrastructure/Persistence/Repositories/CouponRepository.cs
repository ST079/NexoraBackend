using Microsoft.EntityFrameworkCore;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Domain;
using NexoraBackend.Core.Enums;

namespace NexoraBackend.Infrastructure.Persistence.Repositories;

public class CouponRepository : BaseRepository<Coupon>, ICouponRepository
{
    public CouponRepository(AppDbContext context) : base(context) { }

    public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct = default) =>
        await _dbSet.FirstOrDefaultAsync(c => c.Code == code.ToUpperInvariant(), ct);

    public async Task<bool> CodeExistsAsync(string code, CancellationToken ct = default) =>
        await _dbSet.AnyAsync(c => c.Code == code.ToUpperInvariant(), ct);

    /// <summary>
    /// Counts how many orders a user completed using a specific coupon code.
    /// We look at the Order.CouponCode field (set during PlaceOrder).
    /// </summary>
    public async Task<int> GetUserUsageCountAsync(string couponCode, Guid userId,
        CancellationToken ct = default) =>
        await _context.Set<Order>().CountAsync(
            o => o.UserId == userId
                 && o.CouponCode == couponCode.ToUpperInvariant()
                 && o.Status != OrderStatus.Cancelled, ct);
}