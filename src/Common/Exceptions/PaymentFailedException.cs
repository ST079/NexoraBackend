
namespace NexoraBackend.Common.Exceptions;

public class PaymentFailedException : CustomException
{
    public PaymentFailedException(string message) : base(message, 402)
    {
    }
}