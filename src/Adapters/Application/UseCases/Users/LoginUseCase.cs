

using FluentValidation;
using NexoraBackend.Core.Domain.Ports;
using NexoraBackend.Application.DTOs.Inputs.Users;
using NexoraBackend.Application.DTOs.Responses.Users;
using NexoraBackend.Application.DTOs.Responses;


namespace NexoraBackend.Application.UseCases.Users;

public class LoginUseCase
{
    private readonly IValidator<LoginDto> _validator;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _jwtService;

    public LoginUseCase(IValidator<LoginDto> validator, IUserRepository userRepository, ITokenService jwtService)
    {
        _validator = validator;
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<LoginResponseDto> Execute(LoginDto input)
    {
        var result = await _validator.ValidateAsync(input);

        if (!result.IsValid)
        {
            throw new Exception(
                string.Join(", ", result.Errors.Select(e => e.ErrorMessage))
            );
        }

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