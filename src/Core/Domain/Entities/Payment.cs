
namespace NexoraBackend.Core.Domain.Entities;

public class Payment
{
    public string? TransactionId { get; set; }
    public Guid PaymentId { get; private set; }
    public string Method { get; set; } = default!;
    public string Status { get; set; } = default!;
    public decimal Amount { get; set; } = default!;
    // public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}