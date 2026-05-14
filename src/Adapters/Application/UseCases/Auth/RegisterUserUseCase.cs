using NexoraBackend.Application.DTOs.Inputs.Users;
using NexoraBackend.Application.DTOs.Responses.Auth;
using NexoraBackend.Application.DTOs.Responses.Users;
using NexoraBackend.Application.Mappings;
using NexoraBackend.Common.Exceptions;
using NexoraBackend.Common.Helpers;
using NexoraBackend.Core.Domain.Entities;
using NexoraBackend.Core.Domain.Factory;
using NexoraBackend.Core.Domain.Ports;

namespace NexoraBackend.Application.UseCases.Auth;

public class RegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserMapper _mapper;

    private readonly IRefreshTokenRepository _refreshTokenRepository;

    private readonly IRoleRepository _roleRepository;
    private readonly ITokenService _jwtService;

    public RegisterUserUseCase(IRoleRepository roleRepository, IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, UserMapper mapper, ITokenService jwtService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _jwtService = jwtService;
        _refreshTokenRepository = refreshTokenRepository;
        _roleRepository = roleRepository;
    }

    public async Task<RegisterResponseDto> Execute(RegisterDto input)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(input.Email);
        if (existingUser)
            throw new ConflictException("Email already exists");

        var hashedPassword = BCryptPassword.HashPassword(input.Password);

        var defaultRole = await _roleRepository.GetByNameAsync("User");

        if (defaultRole == null)
            throw new Exception("Default role not found");

        var roles = new List<Role> { defaultRole };

        var newUser = _mapper.ToDomain(input, roles);

        newUser.Password = hashedPassword;

        await _userRepository.CreateUserAsync(newUser);

        var accessToken = _jwtService.GenerateAccessToken(newUser);
        var refreshToken = _jwtService.GenerateRefreshToken();

        await _refreshTokenRepository.CreateAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            UserId = newUser.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(2),
            IsRevoked = false
        });

        await _unitOfWork.SaveChangesAsync();

        return new RegisterResponseDto
        {
            RegisteredUser = _mapper.ToResponseDto(newUser),
            Token = new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }
        };
    }

}