using NexoraBackend.Common.Models;
using NexoraBackend.Core.Domain;
using NexoraBackend.Core.Enums;

namespace NexoraBackend.Application.Interfaces.Repositories;


public interface IReviewRepository : IRepository<Review>
{
    Task<PagedList<Review>> GetByProductAsync(Guid productId, int page, int pageSize,
        ReviewStatus? status = ReviewStatus.Approved, CancellationToken ct = default);
    Task<Review?> GetByUserAndProductAsync(Guid userId, Guid productId, CancellationToken ct = default);
    Task<PagedList<Review>> GetPendingAsync(int page, int pageSize, CancellationToken ct = default);
    Task<bool> HasUserReviewedProductAsync(Guid userId, Guid productId, CancellationToken ct = default);
    Task<double> GetAverageRatingAsync(Guid productId, CancellationToken ct = default);
}