using NexoraBackend.Core.Exceptions;
using NexoraBackend.Core.ValueObjects;

namespace NexoraBackend.Core.Domain;

public class OrderItem : Base
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = null!;
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = null!;
    public Money LineTotal => UnitPrice.Multiply(Quantity);

    private OrderItem() { }

    public static OrderItem Create(Guid orderId, Guid productId, string productName,
        int quantity, Money unitPrice)
    {
        return new OrderItem
        {
            OrderId = orderId,
            ProductId = productId,
            ProductName = productName,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new DomainException("Quantity must be positive.");
        Quantity = newQuantity;
    }
}