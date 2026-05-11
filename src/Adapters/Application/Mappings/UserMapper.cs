using NexoraBackend.Application.DTOs.Inputs.Users;
using NexoraBackend.Application.DTOs.Responses.Users;
using NexoraBackend.Application.Entities;
using NexoraBackend.Core.Domain.Entities;
using NexoraBackend.Core.Domain.ValueObjects;

namespace NexoraBackend.Application.Mappings;

public class UserMapper
{
    // DTO → Domain : UserFactory
    public User ToDomain(RegisterDto dto)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            Password = dto.Password,
            PhoneNumber = dto.PhoneNumber,
            Address = new Address
            (
                dto.Street,
                dto.City,
                dto.Country
            ),
            Roles = new List<string> { "User" },
            IsActive = true
        };
    }

    //Domain to Entity
    public UserEntity ToEntity(User domain)
    {
        return new UserEntity
        {
            Id = domain.Id,
            Name = domain.Name,
            Email = domain.Email,
            Password = domain.Password,
            PhoneNumber = domain.PhoneNumber,
            Street = domain.Address.Street ?? "",
            City = domain.Address.City,
            Country = domain.Address.Country,
            IsActive = domain.IsActive
        };
    }

    // Entity → Domain
    public User ToDomain(UserEntity entity)
    {
        return new User
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            Password = entity.Password,
            PhoneNumber = entity.PhoneNumber,
            IsActive = entity.IsActive,
            Address = new Address
            (
                entity.Street,
                entity.City,
                entity.Country
            ),
        };
    }

    // Domain → DTO
    public UserResponseDto ToResponseDto(User domain)
    {
        return new UserResponseDto
        {
            Id = domain.Id,
            Name = domain.Name,
            Email = domain.Email,
            PhoneNumber = domain.PhoneNumber,
            Street = domain.Address?.Street ?? "",
            City = domain.Address?.City ?? "",
            Country = domain.Address?.Country ?? "",
            IsActive = domain.IsActive
        };
    }
}