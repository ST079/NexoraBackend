using NexoraBackend.Application.Entities;
using NexoraBackend.Core.Domain.Entities;

namespace NexoraBackend.Application.Mappings;

public class RoleMapper
{
    public Role ToDomain(RoleEntity entity)
    {
        return new Role
        {
            RoleId = entity.RoleId,
            Name = entity.Name,
        };
    }

    public RoleEntity ToEntity(Role role)
    {
        return new RoleEntity
        {
            RoleId = role.RoleId,
            Name = role.Name
        };
    }
}