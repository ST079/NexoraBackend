namespace NexoraBackend.Core.Events;

public record ReviewSubmittedEvent(Guid ReviewId, Guid ProductId, Guid UserId)
    : BaseDomainEvent;