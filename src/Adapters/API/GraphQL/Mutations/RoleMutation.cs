using HotChocolate.Authorization;
using NexoraBackend.Application.DTOs.Inputs.Roles;
using NexoraBackend.Application.UseCases.Roles;

namespace NexoraBackend.API.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class RoleMutation
{

    [Authorize (Roles = new[] { "Admin" })] 
    public async Task<bool> AddRole(
        AddRoleDto input,
        [Service] AddRoleUseCase useCase)
    {
        return await useCase.Execute(input);
    }

    [Authorize (Roles = new[] { "Admin" })] 
    public async Task<bool> AssignRole(
        AssignRoleDto input,
        [Service] AssignRoleUseCase useCase)
    {
        return await useCase.Execute(input.UserId, input.Name);
    }

    [Authorize (Roles = new[] { "Admin" })]
    public async Task<bool> DeleteRole(Guid roleId, [Service] DeleteRoleUseCase useCase)
    {
        return await useCase.Execute(roleId);
    }
}