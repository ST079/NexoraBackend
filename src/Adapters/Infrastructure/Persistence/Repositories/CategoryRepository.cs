using Microsoft.EntityFrameworkCore;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Domain;
using NexoraBackend.Core.Enums;

namespace NexoraBackend.Infrastructure.Persistence.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }

    public async Task<Category?> GetBySlugAsync(string slug, CancellationToken ct = default) =>
        await _dbSet
            .Include(c => c.Children)
            .Include(c => c.Parent)
            .FirstOrDefaultAsync(c => EF.Property<string>(c, "Slug_Value") == slug, ct);

    public async Task<IReadOnlyList<Category>> GetRootCategoriesAsync(CancellationToken ct = default) =>
        await _dbSet
            .Where(c => c.ParentId == null && c.IsActive)
            .Include(c => c.Children.Where(ch => ch.IsActive))
            .OrderBy(c => c.SortOrder)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Category>> GetWithChildrenAsync(
        Guid parentId, CancellationToken ct = default) =>
        await _dbSet
            .Where(c => c.ParentId == parentId && c.IsActive)
            .Include(c => c.Children)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(ct);

    public async Task<Category?> GetWithProductsAsync(Guid id, CancellationToken ct = default) =>
        await _dbSet
            .Include(c => c.Products.Where(p => p.IsActive))
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null,
        CancellationToken ct = default) =>
        await _dbSet.AnyAsync(c =>
            EF.Property<string>(c, "Slug_Value") == slug
            && (excludeId == null || c.Id != excludeId.Value), ct);
}