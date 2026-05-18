

using System.Security.Principal;
using NexoraBackend.Core.Enums;
using NexoraBackend.Core.Events;
using NexoraBackend.Core.Exceptions;
using NexoraBackend.Core.ValueObjects;

namespace NexoraBackend.Core.Domain;

public class User : Auditable // gets createdAt, UpdatedAt, DeletedAt, IsDeleted, DomainEvents, etc from AuditTable
{

    //private cause changes must happen through methods
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;


    public UserRole Role { get; private set; } = UserRole.Customer;

    public bool IsEmailVerified { get; private set; }
    public string? EmailVerificationToken { get; private set; }

    //used for jwt login + refresh token system
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiry { get; private set; }

    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiry { get; private set; }


    // UserAddress is a child entity that *wraps* the Address value object.
    // We can't store Address (a value object) in a collection directly because
    // EF Core owned-entity collections require a surrogate key — UserAddress provides that
    private readonly List<UserAddress> _addresses = [];
    public IReadOnlyCollection<UserAddress> Addresses => _addresses.AsReadOnly();

    private readonly List<Order> _orders = [];
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

    private User() { } // for EF Core

    public static User Create(string firstName, string lastName, Email email, string passwordHash)
    {
        var user = new User
        {
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = new Email(email),
            PasswordHash = passwordHash,
            EmailVerificationToken = Guid.NewGuid().ToString("N") // Generate a unique token for email verification
        };

        //raises the domain events
        user.AddDomainEvent(new UserRegisteredEvent(user.Id, email)); // Raise a domain event when a new user is created
        return user;
    }

    public void VerifyEmail()
    {
        if (IsEmailVerified) throw new DomainException("Email is already verified.");
        IsEmailVerified = true;
        EmailVerificationToken = null; // Clear the token after verification
    }

    public void UpdateRefreshToken(string token, DateTime expiry)
    {
        RefreshToken = token;
        RefreshTokenExpiry = expiry;
    }

    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiry = null;
    }

    public void SetPasswordResetToken(string token)
    {
        PasswordResetToken = token;
        PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        PasswordResetToken = null; // Clear reset token after password change
        PasswordResetTokenExpiry = null;
        RevokeRefreshToken(); // Invalidate existing refresh tokens after password change
    }

    public void ChangeRole(UserRole newRole)
    {
        if (Role == newRole) throw new DomainException($"User already has the role '{newRole}'.");
        Role = newRole;
    }

    public void AddAddress(UserAddress address) => _addresses.Add(address);

    public void RemoveAddress(Guid addressId)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId);
        if (address == null) throw new DomainException("Address not found.");
        _addresses.Remove(address);
    }

    public string FullName => $"{FirstName} {LastName}";

}
