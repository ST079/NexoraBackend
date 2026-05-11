using NexoraBackend.Application.DTOs.Inputs.Users;
using NexoraBackend.Application.DTOs.Responses.Auth;
using NexoraBackend.Application.DTOs.Responses.Users;
using NexoraBackend.Application.Mappings;
using NexoraBackend.Common.Exceptions;
using NexoraBackend.Common.Helpers;
using NexoraBackend.Core.Domain.Ports;

namespace NexoraBackend.Application.UseCases.Auth;

public class RegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserMapper _mapper;

    private readonly ITokenService _jwtService;

    public RegisterUserUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork, UserMapper mapper, ITokenService jwtService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _jwtService = jwtService;
    }

    public async Task<RegisterResponseDto> Execute(RegisterDto input)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(input.Email);
        if (existingUser)
            throw new ConflictException("Email already exists");

        input.Password = BCryptPassword.HashPassword(input.Password);

        var newUser = _mapper.ToDomain(input);

        await _userRepository.CreateUserAsync(newUser);

        await _unitOfWork.SaveChangesAsync();

        var token = _jwtService.GenerateToken(newUser);

        return new RegisterResponseDto { RegisteredUser = _mapper.ToResponseDto(newUser), Token = token }
        ;

    }

}