namespace NexoraBackend.Application.Entities;

public class RoleEntity
{
    public Guid RoleId { get; set; }
    public string Name { get; set; } = default!;

    public ICollection<UserRoleEntity> UserRoles { get; set; }
      = new List<UserRoleEntity>();
}