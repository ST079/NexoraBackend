using NexoraBackend.Application.DTOs.Responses.Users;

namespace NexoraBackend.Application.DTOs.Responses.Auth;

public class LoginResponseDto
{
    public  AuthResponseDto Token { get; set; } = default!;
    public UserResponseDto LoggedInUser { get; set; } = default!;
}