using NexoraBackend.Core.Domain.Ports;
using NexoraBackend.Application.DTOs.Inputs.Users;
using NexoraBackend.Application.DTOs.Responses.Users;
using NexoraBackend.Application.DTOs.Responses.Auth;


namespace NexoraBackend.Application.UseCases.Auth;

public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _jwtService;

    public LoginUseCase(IUserRepository userRepository, ITokenService jwtService)
    {

        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<LoginResponseDto> Execute(LoginDto input)
    {
        var user = await _userRepository.LoginAsync(input.Email, input.Password);

        if (user == null)
        {
            throw new Exception("Invalid email or password.");
        }

        var token = _jwtService.GenerateToken(user);

        return new LoginResponseDto
        {
            LoggedInUser = new UserResponseDto
            {
                Name = user.Name,
                Email = user.Email
            },
            Token = token
        };
    }
}