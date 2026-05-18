using Microsoft.EntityFrameworkCore;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Domain;
using NexoraBackend.Core.Enums;

namespace NexoraBackend.Infrastructure.Persistence.Repositories;

public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context) { }

    public async Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default) =>
        await _dbSet.FirstOrDefaultAsync(p => p.OrderId == orderId, ct);

    public async Task<Payment?> GetByStripeIntentIdAsync(string paymentIntentId,
        CancellationToken ct = default) =>
        await _dbSet.FirstOrDefaultAsync(p => p.StripePaymentIntentId == paymentIntentId, ct);
}