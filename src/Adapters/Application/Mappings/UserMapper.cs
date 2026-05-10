using NexoraBackend.Application.DTOs.Inputs.Users;
using NexoraBackend.Application.DTOs.Responses.Users;
using NexoraBackend.Application.Entities;
using NexoraBackend.Core.Domain.Entities;

namespace NexoraBackend.Application.Mappings;

public class UserMapper
{
    // DTO → Domain : UserFactory
    
    //Domain to Entity
    public  UserEntity ToEntity(User domain)
    {
        return new UserEntity
        {
            Id = domain.Id,
            Name = domain.Name,
            Email = domain.Email,
            Password = domain.Password,
            PhoneNumber = domain.PhoneNumber,
            Street = domain.Address.Street,
            City = domain.Address.City,
            Country = domain.Address.Country
        };
    }

    // Entity → Domain
    public  User ToDomain(UserEntity entity)
    {
        return new User
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            Password = entity.Password,
            PhoneNumber = entity.PhoneNumber,
            Address =
            {
                Street = entity.Street,
                City = entity.City,
                Country = entity.Country
            }
        };
    }

    // Domain → DTO
    public  UserResponseDto ToResponseDto(User domain)
    {
        return new UserResponseDto
        {
            Id = domain.Id,
            Name = domain.Name,
            Email = domain.Email,
            PhoneNumber = domain.PhoneNumber,
            Street = domain.Address.Street,
            City = domain.Address.City,
            Country = domain.Address.Country
        };
    }
}