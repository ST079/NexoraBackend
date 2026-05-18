
namespace NexoraBackend.Core.Events;

public record UserRegisteredEvent(Guid UserId, string Email)
    : BaseDomainEvent;