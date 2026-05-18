using NexoraBackend.Core.Domain;

namespace NexoraBackend.Application.Interfaces.Repositories;

/// Cart repository keeps the cart loading strategy encapsulated.
/// GetByUserAsync always eager-loads items — a cart without its items is useless.
public interface ICartRepository : IRepository<Cart>
{
    //Returns the cart with all items, or null if the user has no cart yet.
    Task<Cart?> GetByUserAsync(Guid userId, CancellationToken ct = default);

    //Returns the cart with items including refreshed product prices.
    Task<Cart?> GetByUserWithProductsAsync(Guid userId, CancellationToken ct = default);
}