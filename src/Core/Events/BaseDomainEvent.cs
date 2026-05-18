using NexoraBackend.Core.Ports;

namespace NexoraBackend.Core.Events;


//Base class for all domain events, providing common properties like EventId and OccurredAt
// Using record type for immutability(They should never change later ) and value-based equality, which is ideal for events
public abstract record BaseDomainEvent : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}