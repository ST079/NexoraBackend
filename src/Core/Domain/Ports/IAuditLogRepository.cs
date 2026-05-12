using NexoraBackend.Core.Domain.Entities;

namespace NexoraBackend.Core.Domain.Ports;
public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log);
}