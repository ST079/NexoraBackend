namespace NexoraBackend.Core.Events;

// Fired when admin approves a review — triggers product average-rating recalculation
public record ReviewApprovedEvent(Guid ReviewId, Guid ProductId)
    : BaseDomainEvent;