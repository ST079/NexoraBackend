using Microsoft.EntityFrameworkCore;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Domain;

namespace NexoraBackend.Infrastructure.Persistence.Repositories;


public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }

    public async Task<Product?> GetBySlugAsync(string slug, CancellationToken ct = default) =>
        await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => EF.Property<string>(p, "Slug_Value") == slug, ct);

    public async Task<PagedList<Product>> GetPagedAsync(
        int page, int pageSize, string? search, Guid? categoryId,
        decimal? minPrice, decimal? maxPrice, bool? isActive, CancellationToken ct = default)
    {
        var query = _dbSet.Include(p => p.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (minPrice.HasValue)
            query = query.Where(p => EF.Property<decimal>(p, "Price_Amount") >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => EF.Property<decimal>(p, "Price_Amount") <= maxPrice.Value);

        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);

        query = query.OrderByDescending(p => p.CreatedAt);

        return await PagedList<Product>.CreateAsync(query, page, pageSize, ct);
    }

    public async Task<IReadOnlyList<Product>> GetFeaturedAsync(int count, CancellationToken ct = default) =>
        await _dbSet
            .Include(p => p.Category)
            .Where(p => p.IsFeatured && p.IsActive)
            .Take(count)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default) =>
        await _dbSet
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .ToListAsync(ct);

    public async Task<Product?> GetWithReviewsAsync(Guid id, CancellationToken ct = default) =>
        await _dbSet
            .Include(p => p.Reviews)
            .ThenInclude(r => r.User)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<Product?> GetReviewsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}