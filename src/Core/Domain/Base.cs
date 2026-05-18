using NexoraBackend.Core.Ports;

namespace NexoraBackend.Core.Domain;

//abstract class means you cannot create a instance/object of base.
//you must inherit it
public abstract class Base
{
    //protected set; means only the class itself and its derived classes can set the value of Id, 
    //but it can be read by anyone.
    public Guid Id { get; protected set; } = Guid.NewGuid();
    private readonly List<IDomainEvent> _domainEvents = [];

    // stores the domain events like userCreated, orderPlaced etc.......
    //events means something happened inside the domain
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly(); // readonly access to prevent outside code form changing them, that protects integrity.

    //this entity triggered an important event
    protected void AddDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(domainEvent);

    //after events are processed, to avoid duplicate handling, we clear the events
    public void ClearDomainEvents() => _domainEvents.Clear();
}



/*
We use base entity class to provide common properties and 
methods for all entities in our domain. 

Instead of writing the same logic in every entity like:
User
Order
Product
RefreshToken
we create one Base Entity and let all entities inherit from it.

This includes a unique identifier (Id) and 
a collection of domain events that can be raised by the entity. 
The base class also provides methods to add and clear domain events, 
which allows us to implement the Domain Events pattern effectively across our domain model.
*/