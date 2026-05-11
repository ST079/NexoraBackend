namespace NexoraBackend.Application.DTOs.Responses.Auth;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}