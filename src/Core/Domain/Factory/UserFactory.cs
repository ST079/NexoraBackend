using NexoraBackend.Core.Domain.Entities;
using NexoraBackend.Core.Domain.ValueObjects;

namespace NexoraBackend.Core.Domain.Factory;

public class UserFactory
{
    public static User Create(string name, string email, string password, Address address, List<string> roles, string phoneNumber, string? profileImageUrl)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            Password = password,
            Address = address,
            Roles = roles,
            PhoneNumber = phoneNumber,
            ProfileImageUrl = profileImageUrl
        };
    }
}