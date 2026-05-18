using NexoraBackend.Core.Enums;
using NexoraBackend.Core.Exceptions;

namespace NexoraBackend.Core.Exceptions;

public class InvalidOrderStateException : DomainException
{
    public InvalidOrderStateException(Guid orderId, OrderStatus currentStatus, string action)
        : base($"Cannot {action} order {orderId} with status '{currentStatus}'.")
    { }
}