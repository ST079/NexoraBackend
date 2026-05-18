
namespace NexoraBackend.Core.Events;

public record OrderCancelledEvent(Guid OrderId, Guid ProductId, int QuantityToRelease)
    : BaseDomainEvent;