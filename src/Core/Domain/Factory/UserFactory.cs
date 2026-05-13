using NexoraBackend.Core.Domain.Entities;
using NexoraBackend.Core.Domain.ValueObjects;

namespace NexoraBackend.Core.Domain.Factory;

public class UserFactory
{
    public static User Create(
        string name,
        string email,
        string password,
        Address address,
        string phoneNumber,
        string? profileImageUrl,
        List<Role> roles
    )
    {
        // Generate user Id first because UserRole needs it
        var userId = Guid.NewGuid();

        // Create user
        var user = new User
        {
            Id = userId,
            Name = name,
            Email = email,
            Password = password,
            Address = address,
            PhoneNumber = phoneNumber,
            ProfileImageUrl = profileImageUrl,
            IsActive = true
        };

        // Create UserRole join records
        user.UserRoles = roles.Select(role => new UserRole
        {
            UserId = userId,
            RoleId = role.RoleId,
            User = user,
            Role = role
        }).ToList();

        return user;
    }
}