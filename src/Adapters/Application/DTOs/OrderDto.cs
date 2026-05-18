
namespace NexoraBackend.Application.DTOs;

public record OrderDto(
    Guid Id,
    string OrderNumber,
    Guid UserId,
    string UserName,
    string Status,
    decimal Subtotal,
    decimal TaxAmount,
    decimal ShippingCost,
    decimal DiscountAmount,
    decimal Total,
    string Currency,
    IReadOnlyList<OrderItemDto> Items,
    AddressDto ShippingAddress,
    DateTime CreatedAt
    );