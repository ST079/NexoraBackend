using NexoraBackend.Core.ValueObjects;

namespace NexoraBackend.Core.Events;

public record PaymentCompletedEvent(Guid OrderId, Guid PaymentId, Money Amount) : BaseDomainEvent;