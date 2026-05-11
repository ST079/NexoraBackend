using NexoraBackend.Application.DTOs.Responses.Users;

namespace NexoraBackend.Application.DTOs.Responses.Auth;

public class RegisterResponseDto
{
    public UserResponseDto RegisteredUser { get; set; } = default!;
    public string Token { get; set; } = default!;
}