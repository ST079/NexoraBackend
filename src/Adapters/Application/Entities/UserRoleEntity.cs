
namespace NexoraBackend.Application.Entities;

public class UserRoleEntity
{
    //User and Role Join Table for Many-to-Many relationship
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = default!;
    public Guid RoleId { get; set; }
    public RoleEntity Role { get; set; } = default!;

    
}