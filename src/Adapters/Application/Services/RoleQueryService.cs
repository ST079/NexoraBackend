using NexoraBackend.Application.DTOs.Responses;
using NexoraBackend.Application.Mappings;
using NexoraBackend.Core.Domain.Ports;

namespace NexoraBackend.Application.Services;


public class RoleQueryService
{
    private readonly IRoleRepository _roleRepository;
    private readonly RoleMapper _mapper;

    public RoleQueryService(IRoleRepository roleRepository, RoleMapper mapper)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<RoleResponseDto?> GetByNameAsync(string name)
    {
        var role = await _roleRepository.GetByNameAsync(name);
        return _mapper.ToResponseDto(role!);
    }

    public async Task<List<RoleResponseDto>> GetAllAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        return roles.Select(r => _mapper.ToResponseDto(r)).ToList();
    }

    public async Task<RoleResponseDto?> GetByIdAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        return _mapper.ToResponseDto(role!);
    }
}