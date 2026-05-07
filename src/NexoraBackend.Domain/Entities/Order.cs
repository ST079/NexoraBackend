using NexoraBackend.Domain.ValueObjets;

namespace NexoraBackend.Domain.Entities;

public class Order
{
    public Guid OrderId = new Guid();

    public Guid OrderedBy { get; set; } = default!;
    public List<string> Status { get; set; } = ["PENDING"];

    public IEnumerable<Product> OrderItems { get; set; } = default!;

    public Address ShippingAddress { get; set; } = default!;

    public decimal TotalPrice { get; set; } = default!;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public string OrderNumber { get; set; } = default!;

    public Guid? PaymentId { get; set; }

}