using NexoraBackend.Application.DTOs.Inputs.Roles;
using NexoraBackend.Application.Mappings;
using NexoraBackend.Core.Domain.Ports;

namespace NexoraBackend.Application.UseCases.Roles;
public class AddRoleUseCase
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    private readonly RoleMapper _mapper;

    public AddRoleUseCase(IRoleRepository roleRepository, IUnitOfWork unitOfWork, RoleMapper mapper)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<bool> Execute(AddRoleDto input)
    {
        var role = _mapper.ToDomain(input); 

        await _roleRepository.AddAsync(role);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}