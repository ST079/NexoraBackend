
namespace NexoraBackend.Core.Events;

public record LowStockEvent(Guid ProductId, string ProductName, int CurrentStock) 
    : BaseDomainEvent;