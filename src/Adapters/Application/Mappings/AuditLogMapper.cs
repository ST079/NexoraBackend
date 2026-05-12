using NexoraBackend.Application.Entities;
using NexoraBackend.Core.Domain.Entities;

namespace NexoraBackend.Application.Mappings;

public class AuditLogMapper
{
    //Domain to Entity
    public AuditLogEntity ToEntity(AuditLog domain)
    {
        return new AuditLogEntity
        {
            Id = domain.Id,
            UserId = domain.UserId,
            Action = domain.Action,
            Entity = domain.Entity,
            Details = domain.Details,
            CreatedAt = domain.CreatedAt
        };
    }

    public AuditLog ToDomain(AuditLogEntity entity)
    {
        return new AuditLog
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Action = entity.Action,
            Entity = entity.Entity,
            Details = entity.Details,
            CreatedAt = entity.CreatedAt
        };
    }
}