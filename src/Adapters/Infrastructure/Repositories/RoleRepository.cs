using Microsoft.EntityFrameworkCore;
using NexoraBackend.Core.Domain.Entities;
using NexoraBackend.Core.Domain.Ports;
using NexoraBackend.Application.Entities;
using NexoraBackend.Infrastructure.Persistence;
using NexoraBackend.Application.Mappings;
using NexoraBackend.Common.Enums;
using NexoraBackend.Common.Exceptions;

namespace NexoraBackend.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _dbContext;
    private readonly RoleMapper _mapper;

    public RoleRepository(AppDbContext dbContext, RoleMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    // Get multiple roles by IDs
    public async Task<List<Role>> GetByIdsAsync(List<Guid> roleIds)
    {
        var roles = await _dbContext.Roles
            .Where(r => roleIds.Contains(r.RoleId))
            .ToListAsync();

        return roles.Select(r => _mapper.ToDomain(r)).ToList();
    }

    // Get role by name
    public async Task<Role?> GetByNameAsync(string roleName)
    {
        var role = await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Name == roleName);

        if (role == null)
            throw new NotFoundException($"Role with name '{roleName}' not found.");

        return _mapper.ToDomain(role);
    }

    // Get all roles
    public async Task<List<Role>> GetAllAsync()
    {
        return await _dbContext.Roles
            .Select(r => _mapper.ToDomain(r))
            .ToListAsync();
    }

    // Get role by ID
    public async Task<Role?> GetByIdAsync(Guid id)
    {
        var role = await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.RoleId == id);

        if (role == null)
            throw new NotFoundException($"Role with ID '{id}' not found.");

        return _mapper.ToDomain(role);
    }

    // Create new role
    public async Task AddAsync(Role role)
    {
        if (!Enum.IsDefined(typeof(RoleType), role.Name))
        {
            throw new ConflictException($"Invalid role type provided: {role.Name}");
        }


        var existingRole = await _dbContext.Roles
            .FirstOrDefaultAsync(r => string.Equals(r.Name.ToLower(), role.Name.ToLower()));

        if (existingRole != null)
            throw new ConflictException($"Role with name '{role.Name}' already exists.");


        var entity = new RoleEntity
        {
            RoleId = role.RoleId == Guid.Empty ? Guid.NewGuid() : role.RoleId,
            Name = role.Name
        };

        await _dbContext.Roles.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleId == id);
        if (entity != null)
        {
            _dbContext.Roles.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task AssignRoleAsync(Guid userId, Guid roleId, string roleName)
    {
        var user = await _dbContext.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new NotFoundException("User not found");

        var alreadyExists = user.UserRoles
            .Any(x => x.RoleId == roleId);

        if (alreadyExists)
            throw new ConflictException("Role already assigned");

        user.UserRoles.Add(new UserRoleEntity
        {
            UserId = userId,
            RoleId = roleId
        });

        var existInRolesList = user.Roles.Any(r => r.Equals(roleName, StringComparison.OrdinalIgnoreCase));

        if (!existInRolesList)
        {
            user.Roles.Add(roleName);
        }
    }
}
