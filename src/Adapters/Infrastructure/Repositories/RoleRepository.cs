using NexoraBackend.Application.Mappings;
using NexoraBackend.Core.Domain.Entities;
using NexoraBackend.Core.Domain.Ports;
using NexoraBackend.Infrastructure.Persistence;

namespace NexoraBackend.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;
    private readonly RoleMapper _mapper;

    public RoleRepository(AppDbContext context, RoleMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Role> CreateRoleAsync(Role role)
    {
        var entity = _mapper.ToEntity(role);
        await _context.Roles.AddAsync(entity);
        await _context.SaveChangesAsync();
        return _mapper.ToDomain(entity);
    }


    public async Task<bool> DeleteRoleAsync(Guid id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) return false;

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Role?> GetRoleByIdAsync(Guid id)
    {
        var role = await _context.Roles.FindAsync(id);
        return role == null ? null : _mapper.ToDomain(role);
    }

    public Task<IEnumerable<Role>> GetRolesAsync()
    {
        throw new NotImplementedException();
    }
}