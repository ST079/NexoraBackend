using Microsoft.EntityFrameworkCore;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Domain;
using NexoraBackend.Core.Enums;

namespace NexoraBackend.Infrastructure.Persistence.Repositories;



public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context) { }

    public async Task<Order?> GetWithItemsAsync(Guid id, CancellationToken ct = default) =>
        await _dbSet
            .Include(o => o.Items)
            .Include(o => o.User)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct = default)
    {
        return await _dbSet
            .Include(o => o.Items)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, ct);
    }

    public async Task<PagedList<Order>> GetByUserAsync(Guid userId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _dbSet
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt);
        return await PagedList<Order>.CreateAsync(query, page, pageSize, ct);
    }

    public async Task<PagedList<Order>> GetAllPagedAsync(int page, int pageSize,
        OrderStatus? status, CancellationToken ct = default)
    {
        var query = _dbSet.Include(o => o.User).AsQueryable();
        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);
        query = query.OrderByDescending(o => o.CreatedAt);
        return await PagedList<Order>.CreateAsync(query, page, pageSize, ct);
    }

    public async Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status, CancellationToken ct = default) =>
        await _dbSet.Where(o => o.Status == status).ToListAsync(ct);
}
