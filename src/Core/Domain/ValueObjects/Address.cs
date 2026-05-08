namespace NexoraBackend.Core.Domain.ValueObjects;

public class Address
{
    public string Country { get; set; } = "Nepal";
    public string City { get; set; } = default!;
    public string Province { get; set; } = default!;
    public string? Street { get; set; }

    public Address(string city, string province, string? street, string country = "Nepal")
    {
        Country = country;
        Province = province;
        City = city;
        Street = street ?? string.Empty;

    }
}