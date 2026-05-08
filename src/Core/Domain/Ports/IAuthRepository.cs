using NexoraBackend.Core.Domain.Entities;

namespace NexoraBackend.Core.Domain.Ports;

public interface IAuthRepository
{
    Task<string> LoginAsync(string email, string password);
    Task<User> RegisterAsync(User user);
}