using NexoraBackend.Common.Exceptions;
using NexoraBackend.Core.Domain.Ports;

namespace NexoraBackend.Application.UseCases.Roles;


public class DeleteRoleUseCase
{
    private readonly IRoleRepository _roleRepository;

    public DeleteRoleUseCase(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<bool> Execute(Guid roleId)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
            throw new NotFoundException($"Role with ID '{roleId}' not found.");

        await _roleRepository.DeleteAsync(roleId);
        return true;
    }
}