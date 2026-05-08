using NexoraBackend.Core.Domain.Entities;

namespace NexoraBackend.Core.Domain.Ports;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetOrdersAsync();
    Task<Order?> GetOrderByIdAsync(Guid id);
    Task<IEnumerable<Order>> GetOrderByUserIdAsync(Guid id);
    Task<Order> CreateOrderAsync(Order order);
    Task<bool> CancelOrderAsync(Guid id, User user);
    Task<bool> UpdateOrderStatusAsync(Guid id, string newStatus);
    Task<bool> DeleteOrderAsync(Guid id);
    Task<bool> ConfirmOrderAsync(Guid id);

}