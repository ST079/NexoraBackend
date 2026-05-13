using NexoraBackend.Common.Exceptions;
using NexoraBackend.Core.Domain.Ports;

namespace NexoraBackend.Application.UseCases.Roles;

public class AssignRoleUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IRoleRepository _roleRepository;

    private readonly IUserRepository _userRepository;

    public AssignRoleUseCase(IUnitOfWork unitOfWork, IRoleRepository roleRepository, IUserRepository userRepository)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Execute(Guid userId, string roleName)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user == null)
            throw new NotFoundException($"User with ID '{userId}' not found.");

        var role = await _roleRepository.GetByNameAsync(roleName);

        if (role == null)
            throw new NotFoundException($"Role with name '{roleName}' not found.");

        await _roleRepository.AssignRoleAsync(
            userId,
            role.RoleId,
            role.Name
        );

        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}