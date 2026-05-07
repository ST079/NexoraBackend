namespace NexoraBackend.Domain.Entities;

public class Payment
{
    public string? TransactionId { get; set; }
    public string Method { get; set; } = default!;
    public string status { get; set; } = default!;
    public decimal amount { get; set; } = default!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}