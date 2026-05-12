
namespace NexoraBackend.Core.Domain.Entities;

public class UserRole
{
    //User and Role Join Table for Many-to-Many relationship
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = default!;
}