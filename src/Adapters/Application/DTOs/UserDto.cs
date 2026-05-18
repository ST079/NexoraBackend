

namespace NexoraBackend.Application.DTOs;

public record UserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    bool IsEmailVerified,
    DateTime CreatedAt
    );