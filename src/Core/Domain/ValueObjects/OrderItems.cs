namespace NexoraBackend.Core.Domain.ValueObjects;
public class OrderItem
{
    public Guid ProductId { get; }
    public int Quantity { get; }
    public decimal Price { get; }
}