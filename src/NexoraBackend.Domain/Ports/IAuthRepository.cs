using NexoraBackend.Domain.Entities;

namespace NexoraBackend.Domain.Ports;

public interface IAuthRepository
{
    Task<string> LoginAsync(string email, string password);
    Task<User> RegisterAsync(User user);
}