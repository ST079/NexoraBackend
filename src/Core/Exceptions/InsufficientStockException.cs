namespace NexoraBackend.Core.Exceptions;


public class InsufficientStockException : DomainException
{
    public InsufficientStockException(Guid productId, string productName, int requested, int available)
        : base($"Product '{productName}' (ID: {productId}) has only {available} units available, but {requested} were requested.")
    { }
}
