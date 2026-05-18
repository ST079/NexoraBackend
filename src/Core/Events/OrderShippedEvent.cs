
namespace NexoraBackend.Core.Events;

public record OrderShippedEvent(Guid OrderId, Guid UserId, string TrackingNumber) : BaseDomainEvent;