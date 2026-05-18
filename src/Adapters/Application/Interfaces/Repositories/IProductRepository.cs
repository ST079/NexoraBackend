
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Domain;

namespace NexoraBackend.Application.Interfaces.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<PagedList<Product>> GetPagedAsync(int page, int pageSize,
    string? search = null, Guid? categoryId = null, decimal? minPrice = null
    , decimal? maxPrice = null, bool? isActive = true, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetFeaturedAsync(int count, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);

    Task<Product?> GetReviewsAsync(Guid id, CancellationToken cancellationToken = default);
}