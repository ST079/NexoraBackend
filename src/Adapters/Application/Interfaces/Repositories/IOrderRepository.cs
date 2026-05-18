using NexoraBackend.Common.Models;
using NexoraBackend.Core.Domain;
using NexoraBackend.Core.Enums;

namespace NexoraBackend.Application.Interfaces.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetWithItemsAsync(Guid id, CancellationToken ct = default);
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct = default);
    Task<PagedList<Order>> GetByUserAsync(Guid userId, int page, int pageSize, CancellationToken ct = default);
    Task<PagedList<Order>> GetAllPagedAsync(int page, int pageSize,
        OrderStatus? status = null, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status, CancellationToken ct = default);
}