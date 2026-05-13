using NexoraBackend.Application.DTOs.Responses.Users;

namespace NexoraBackend.Application.DTOs.Responses.Auth;

public class RegisterResponseDto
{
    public UserResponseDto RegisteredUser { get; set; } = default!;
    public AuthResponseDto Token { get; set; } = default!;
}