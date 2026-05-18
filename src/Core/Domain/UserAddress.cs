using NexoraBackend.Core.Exceptions;
using NexoraBackend.Core.ValueObjects;

namespace NexoraBackend.Core.Domain;

/// <summary>
/// UserAddress is a child entity that wraps the Address value object.
/// Why not store Address directly in a collection?
/// EF Core's owned entity collections require a surrogate key — UserAddress
/// provides that key (Id) while embedding the Address value object columns.
/// 
/// It also carries addressbook metadata (label, isDefault) which has no place
/// on a pure Address value object. Value objects are about structural equality,
/// not identity or metadata.
/// </summary>
public class UserAddress : Base
{
    public Guid UserId { get; private set; }
    public Address Address { get; private set; } = null!;
    public string Label { get; private set; } = "Home";  // "Home", "Work", "Other"
    public bool IsDefault { get; private set; }

    private UserAddress() { }

    public static UserAddress Create(Guid userId, Address address,
        string label = "Home", bool isDefault = false)
    {
        if (string.IsNullOrWhiteSpace(label))
            throw new DomainException("Address label is required.");

        return new UserAddress
        {
            UserId = userId,
            Address = address,
            Label = label.Trim(),
            IsDefault = isDefault
        };
    }

    public void SetAsDefault() => IsDefault = true;
    public void UnsetDefault() => IsDefault = false;
    public void UpdateLabel(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
            throw new DomainException("Address label is required.");
        Label = label.Trim();
    }

}