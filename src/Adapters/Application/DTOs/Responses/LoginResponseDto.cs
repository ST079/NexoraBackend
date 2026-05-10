using NexoraBackend.Application.DTOs.Responses.Users;

namespace NexoraBackend.Application.DTOs.Responses;

public class LoginResponseDto
{
    public string Token { get; set; } = default!;
    public UserResponseDto LoggedInUser { get; set; } = default!;
}