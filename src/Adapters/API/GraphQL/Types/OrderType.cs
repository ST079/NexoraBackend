using NexoraBackend.Application.DTOs;

namespace NexoraBackend.API.GraphQL.Types;

public class OrderType : ObjectType<OrderDto>
{
    protected override void Configure(IObjectTypeDescriptor<OrderDto> descriptor)
    {
        descriptor.Description("A customer order.");
        descriptor.Field(o => o.Id).Description("Unique order identifier.");
        descriptor.Field(o => o.OrderNumber).Description("Human-readable order reference.");
        descriptor.Field(o => o.Status).Description("Current order status.");
        descriptor.Field(o => o.Total).Description("Total amount charged.");
    }
}