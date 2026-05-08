namespace NexoraBackend.Application.Entities;
public class OrderItemEntity
{
    public Guid OrderItemId { get; set; }
    public Guid OrderId { get; set; }   // FK
    public Guid ProductId { get; set; }  // FK
    public int Quantity { get; set; }
    public decimal PriceAtTime { get; set; } //snapshot of price at ordertime
}