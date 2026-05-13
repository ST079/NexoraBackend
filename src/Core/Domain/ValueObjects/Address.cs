namespace NexoraBackend.Core.Domain.ValueObjects;

public class Address
{
    public string Country { get; set; } = "Nepal";
    public string City { get; set; } = default!;
    public string? Street { get; set; }

    public Address(string city, string? street, string country = "Nepal")
    {
        Country = country;
        City = city;
        Street = street ?? string.Empty;

    }
}