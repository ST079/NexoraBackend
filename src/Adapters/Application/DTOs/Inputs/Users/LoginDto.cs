namespace NexoraBackend.Application.DTOs.Inputs.Users;

public class LoginDto
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}