
namespace NexoraBackend.Application.Entities;

public class OrderEntity
{
    public Guid OrderId {get; private set; }
    public Guid OrderedBy { get; set; } = default!;
    public string Status { get; set; } = "PENDING";
    public string ShippingAddressStreet { get; set; } = default!;
    public string ShippingAddressCity { get; set; } = default!;
    public string ShippingAddressCountry { get; set; } = default!;
    public decimal TotalPrice { get; set; } = default!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public string OrderNumber { get; set; } = default!;
    public Guid? PaymentId { get; set; }

}