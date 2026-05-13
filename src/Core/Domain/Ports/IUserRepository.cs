using NexoraBackend.Core.Domain.Entities;

namespace NexoraBackend.Core.Domain.Ports;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsersAsync();
    Task<User?> GetUserByIdAsync(Guid id);
    Task<bool> GetUserByEmailAsync(string email);
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(Guid id);
    Task<User> LoginAsync(string email, string password);
}