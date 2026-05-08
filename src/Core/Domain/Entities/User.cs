

using NexoraBackend.Core.Domain.ValueObjects;

namespace NexoraBackend.Core.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public Address Address { get; set; } = default!;
    public List<string> Roles { get; set; } = new();
    public string PhoneNumber { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }

    
}
