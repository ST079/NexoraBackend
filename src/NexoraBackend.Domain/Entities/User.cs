

using NexoraBackend.Domain.ValueObjets;

namespace NexoraBackend.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public Address Address { get; set; } = default!;
    public List<string> Roles { get; set; } = new();
    public string PhoneNumber { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public DateTime CreatedAt {get; private set;} = DateTime.UtcNow;
    public bool IsActive { get; set; }
}
