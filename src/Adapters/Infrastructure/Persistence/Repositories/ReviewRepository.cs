using Microsoft.EntityFrameworkCore;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Domain;
using NexoraBackend.Core.Enums;

namespace NexoraBackend.Infrastructure.Persistence.Repositories;

public class ReviewRepository : BaseRepository<Review>, IReviewRepository
{
    public ReviewRepository(AppDbContext context) : base(context) { }

    public async Task<PagedList<Review>> GetByProductAsync(Guid productId, int page, int pageSize,
        ReviewStatus? status = ReviewStatus.Approved, CancellationToken ct = default)
    {
        var query = _dbSet
            .Include(r => r.User)
            .Where(r => r.ProductId == productId);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        query = query.OrderByDescending(r => r.HelpfulVotes.Count(v => v.IsHelpful))
                     .ThenByDescending(r => r.CreatedAt);

        return await PagedList<Review>.CreateAsync(query, page, pageSize, ct);
    }

    public async Task<Review?> GetByUserAndProductAsync(Guid userId, Guid productId,
        CancellationToken ct = default) =>
        await _dbSet
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId, ct);

    public async Task<PagedList<Review>> GetPendingAsync(int page, int pageSize,
        CancellationToken ct = default)
    {
        var query = _dbSet
            .Include(r => r.User)
            .Include(r => r.Product)
            .Where(r => r.Status == ReviewStatus.Pending)
            .OrderBy(r => r.CreatedAt);

        return await PagedList<Review>.CreateAsync(query, page, pageSize, ct);
    }

    public async Task<bool> HasUserReviewedProductAsync(Guid userId, Guid productId,
        CancellationToken ct = default) =>
        await _dbSet.AnyAsync(r => r.UserId == userId && r.ProductId == productId, ct);

    public async Task<double> GetAverageRatingAsync(Guid productId, CancellationToken ct = default)
    {
        var ratings = await _dbSet
            .Where(r => r.ProductId == productId && r.Status == ReviewStatus.Approved)
            .Select(r => EF.Property<int>(r, "Rating_Value"))
            .ToListAsync(ct);

        return ratings.Count == 0 ? 0 : ratings.Average();
    }
}