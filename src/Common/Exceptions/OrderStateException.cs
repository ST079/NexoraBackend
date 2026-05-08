
namespace NexoraBackend.Common.Exceptions;

public class OrderStateException : CustomException
{
    public OrderStateException(string message) : base(message, 400)
    {
    }
}