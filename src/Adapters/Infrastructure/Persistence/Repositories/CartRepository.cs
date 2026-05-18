using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Core.Domain;

namespace NexoraBackend.Infrastructure.Persistence.Repositories;

public class CartRepository : BaseRepository<Cart>, ICartRepository
{
    public CartRepository(AppDbContext context) : base(context) { }

    public async Task<Cart?> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        await _dbSet
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);

    /// <summary>
    /// Loads cart items together with the current product data so the
    /// application layer can detect stale prices or out-of-stock items.
    /// </summary>
    public async Task<Cart?> GetByUserWithProductsAsync(Guid userId, CancellationToken ct = default) =>
        await _dbSet
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);
}
