using NexoraBackend.Core.Domain.Entities;

namespace NexoraBackend.Core.Domain.Ports;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetRolesAsync();
    Task<Role?> GetRoleByIdAsync(Guid id);
    Task<Role> CreateRoleAsync(Role role);
    Task<bool> DeleteRoleAsync(Guid id);
}