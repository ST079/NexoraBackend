using NexoraBackend.Core.Domain.ValueObjects;


namespace NexoraBackend.Core.Domain.Entities;

public class Order
{
    public Guid OrderId { get; private set; }

    public Guid OrderedBy { get; set; } = default!;
    public string Status { get; set; } = "PENDING";

    public List<OrderItem> OrderItems { get; set; } = default!;

    public Address ShippingAddress { get; set; } = default!;

    public decimal TotalPrice { get; set; } = default!;

    public string OrderNumber { get; set; } = default!;

    public Guid? PaymentId { get; set; }

    public Order(
        Guid orderId,
        Guid orderedBy,
        List<OrderItem> orderItems,
        Address shippingAddress,
        string orderNumber)
    {
        OrderId = orderId;
        OrderedBy = orderedBy;
        OrderItems = orderItems;
        ShippingAddress = shippingAddress;
        OrderNumber = orderNumber;
        Status = "PENDING";

        TotalPrice = orderItems.Sum(x => x.Price);
    }
}