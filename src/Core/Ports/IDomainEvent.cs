
using MediatR;
namespace NexoraBackend.Core.Ports;

//“Something important happened inside the business domain” is Domain Event
public interface IDomainEvent : INotification //INotification is marker interface from MediatR, 
// which allows us to publish these events and have handlers react to them asynchronously
{

    //Not Directly using INotification because 
    // we want to enforce that all domain events have these properties, 
    // which can be useful for logging, auditing, or other cross-cutting concerns
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}
