namespace NexoraBackend.Application.DTOs.Inputs.Users;

public class CreateUserDto
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Street { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? ProfilePictureUrl { get; set; }
}