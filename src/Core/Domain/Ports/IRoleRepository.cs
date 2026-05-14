using NexoraBackend.Core.Domain.Entities;

namespace NexoraBackend.Core.Domain.Ports;

public interface IRoleRepository
{
    Task<List<Role>> GetByIdsAsync(List<Guid> roleIds);
    Task<Role?> GetByNameAsync(string name);
    Task<List<Role>> GetAllAsync();
    Task<Role?> GetByIdAsync(Guid id);
    Task AddAsync(Role role);
    Task DeleteAsync(Guid id);
    Task AssignRoleAsync(Guid userId, Guid roleId, string roleName);


}