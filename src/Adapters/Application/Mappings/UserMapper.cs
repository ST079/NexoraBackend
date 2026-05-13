using NexoraBackend.Application.DTOs.Inputs.Users;
using NexoraBackend.Application.DTOs.Responses.Users;
using NexoraBackend.Application.Entities;
using NexoraBackend.Core.Domain.Entities;
using NexoraBackend.Core.Domain.Factory;
using NexoraBackend.Core.Domain.ValueObjects;

namespace NexoraBackend.Application.Mappings;

public class UserMapper
{
    /*
        DTO → Domain
        Now uses UserFactory
    */
    public User ToDomain(CreateUserDto dto, List<Role> roles)
    {
        var address = new Address(
            dto.Street,
            dto.City,
            dto.Country
        );

        return UserFactory.Create(
            name: dto.Name,
            email: dto.Email,
            password: dto.Password,
            address: address,
            phoneNumber: dto.PhoneNumber ?? "",
            profileImageUrl: dto.ProfilePictureUrl,
            roles: roles
        );
    }

    public User ToDomain(RegisterDto dto, List<Role> roles)
    {
        var address = new Address(
            dto.Street ?? "",
            dto.City,
            dto.Country ?? "Nepal"
        );

        return UserFactory.Create(
            name: dto.Name,
            email: dto.Email,
            password: dto.Password,
            address: address,
            phoneNumber: dto.PhoneNumber,
            profileImageUrl: dto.ProfileImageUrl,
            roles: roles
        );
    }

    /*
        Domain → Entity
    */
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
            IsActive = domain.IsActive,

            UserRoles = domain.UserRoles.Select(ur => new UserRoleEntity
            {
                UserId = domain.Id,
                RoleId = ur.RoleId
            }).ToList()
        };
    }

    /*
        Entity → Domain
    */
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

            Address = new Address(
                entity.Street,
                entity.City,
                entity.Country
            ),
            Roles = entity.Roles
        };
    }

    /*
        Domain → Response DTO
    */
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
            IsActive = domain.IsActive,
            Roles = domain.Roles
        };
    }

}