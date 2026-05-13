using HotChocolate.Authorization;
using NexoraBackend.Application.DTOs.Responses;

using NexoraBackend.Application.Services;

namespace NexoraBackend.API.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]

[Authorize(Roles = new[] { "Admin" })]
public class RoleQuery
{   
    public async Task<RoleResponseDto?> GetRoleByNameAsync(string name, [Service] RoleQueryService roleService)
    {
        return await roleService.GetByNameAsync(name);
    }

    public async Task<List<RoleResponseDto>> GetRolesAsync([Service] RoleQueryService roleService)
    {
        return await roleService.GetAllAsync();
    }

    public async Task<RoleResponseDto?> GetRoleByIdAsync(Guid id, [Service] RoleQueryService roleService)
    {
        return await roleService.GetByIdAsync(id);
    }
}