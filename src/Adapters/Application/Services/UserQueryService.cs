
using NexoraBackend.Application.DTOs.Responses.Users;
using NexoraBackend.Application.Mappings;
using NexoraBackend.Common.Exceptions;
using NexoraBackend.Core.Domain.Ports;

namespace NexoraBackend.Application.Services;

public class UserQueryService
{
    private readonly IUserRepository _userRepository;

    private readonly UserMapper _mapper;

    public UserQueryService(IUserRepository userRepository, UserMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserResponseDto>> GetUsersAsync()
    {
        var users = await _userRepository.GetUsersAsync();
        return users.Select(u => _mapper.ToResponseDto(u));
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        return _mapper.ToResponseDto(user!);
    }

    public async Task<bool> GetUserByEmail(string email)
    {
        return await _userRepository.GetUserByEmailAsync(email);
    }
}