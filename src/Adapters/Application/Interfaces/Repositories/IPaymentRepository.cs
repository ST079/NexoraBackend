using NexoraBackend.Core.Domain;

namespace NexoraBackend.Application.Interfaces.Repositories;


public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);
    Task<Payment?> GetByStripeIntentIdAsync(string paymentIntentId, CancellationToken ct = default);
}