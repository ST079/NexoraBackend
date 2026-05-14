

using NexoraBackend.Core.Domain.ValueObjects;

namespace NexoraBackend.Core.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public Address Address { get; set; } = default!;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public List<string> Roles { get; set; } = new();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public bool IsActive { get; set; }
}
