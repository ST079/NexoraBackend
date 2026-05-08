
namespace NexoraBackend.Common.Exceptions;

public class InsufficientStockException : CustomException
{
    public InsufficientStockException(string message) : base(message, 400)
    {
    }
}