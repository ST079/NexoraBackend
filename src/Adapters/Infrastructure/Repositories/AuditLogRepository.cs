using NexoraBackend.Application.Mappings;
using NexoraBackend.Core.Domain.Entities;
using NexoraBackend.Core.Domain.Ports;
using NexoraBackend.Infrastructure.Persistence;

namespace NexoraBackend.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _context;

    private readonly AuditLogMapper _mapper ;

    public AuditLogRepository(AppDbContext context, AuditLogMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task AddAsync(AuditLog log)
    {
        await _context.AuditLogs.AddAsync(_mapper.ToEntity(log));
        await _context.SaveChangesAsync();
    }
}