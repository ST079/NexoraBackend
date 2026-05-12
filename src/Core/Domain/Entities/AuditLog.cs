namespace NexoraBackend.Core.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? UserId { get; set; } 

    public string Action { get; set; } = default!; 

    public string? Entity { get; set; } 

    public string? Details { get; set; } 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? IpAddress { get; set; }
}