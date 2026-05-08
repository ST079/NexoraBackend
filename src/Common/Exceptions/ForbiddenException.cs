
namespace NexoraBackend.Common.Exceptions;

public class ForbiddenException : CustomException
{
    public ForbiddenException(string message) : base(message, 403)
    {
    }
}