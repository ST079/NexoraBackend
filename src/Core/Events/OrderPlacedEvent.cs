
using NexoraBackend.Core.ValueObjects;

namespace NexoraBackend.Core.Events;

public record OrderPlacedEvent(Guid OrderId, Guid UserId, Money TotalAmount) : BaseDomainEvent;
