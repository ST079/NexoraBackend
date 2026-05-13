
namespace NexoraBackend.Core.Domain.Ports;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}