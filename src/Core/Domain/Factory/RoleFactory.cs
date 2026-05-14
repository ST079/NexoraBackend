using NexoraBackend.Core.Domain.Entities;

namespace NexoraBackend.Core.Domain.Factory;

public class RoleFactory
{
    public static Role Create(string roleName)
    {
        return new Role
        {
            RoleId = Guid.NewGuid(),
            Name = roleName
        };
    }

    public static List<Role> DefaultRoles()
    {
        return new List<Role>
        {
            new Role
            {
                RoleId = Guid.NewGuid(),
                Name = "User"
            }
        };
    }
}