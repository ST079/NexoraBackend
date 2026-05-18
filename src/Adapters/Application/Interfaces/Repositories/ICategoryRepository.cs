

using NexoraBackend.Core.Domain;

namespace NexoraBackend.Application.Interfaces.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<IReadOnlyList<Category>> GetRootCategoriesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Category>> GetWithChildrenAsync(Guid parentId, CancellationToken ct = default);
    Task<Category?> GetWithProductsAsync(Guid id, CancellationToken ct = default);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken ct = default);
}