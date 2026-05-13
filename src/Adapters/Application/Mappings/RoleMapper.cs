using NexoraBackend.Application.DTOs.Inputs.Roles;
using NexoraBackend.Application.DTOs.Responses;
using NexoraBackend.Application.Entities;
using NexoraBackend.Core.Domain.Entities;

namespace NexoraBackend.Application.Mappings;

public class RoleMapper
{



    // Input DTO to Domain
    public Role ToDomain(AddRoleDto dto)
    {
        return new Role
        {
            RoleId = Guid.NewGuid(),
            Name = dto.Name,
        };
    }


    // Entity to Domain
    public Role ToDomain(RoleEntity entity)
    {
        return new Role
        {
            RoleId = entity.RoleId,
            Name = entity.Name,
        };
    }

    // Domain to Entity
    public RoleEntity ToEntity(Role role)
    {
        return new RoleEntity
        {
            RoleId = role.RoleId,
            Name = role.Name
        };
    }

    // Domain to Response DTO
    public RoleResponseDto ToResponseDto(Role role)
    {
        return new RoleResponseDto
        {
            Id = role.RoleId,
            Name = role.Name
        };
    }
}