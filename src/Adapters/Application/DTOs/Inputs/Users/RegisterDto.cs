namespace NexoraBackend.Application.DTOs.Inputs.Users;

public class RegisterDto
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string? Street { get; set; }
    public string City { get; set; } = default!;
    public string? Country { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
}