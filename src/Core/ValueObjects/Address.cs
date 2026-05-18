using NexoraBackend.Core.Exceptions;

namespace NexoraBackend.Core.ValueObjects;

public sealed record Address
{
    public string Line1 { get; }
    public string? Line2 { get; }
    public string City { get; }
    public string State { get; }
    public string PostalCode { get; }
    public string Country { get; }  // ISO 3166-1 alpha-2, e.g. "US"

    private Address() { } // EF Core

    public Address(string line1, string? line2, string city,
        string state, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(line1))
            throw new DomainException("Address line 1 is required.");
        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City is required.");
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new DomainException("Postal code is required.");
        if (string.IsNullOrWhiteSpace(country) || country.Length != 2)
            throw new DomainException("Country must be a 2-letter ISO 3166-1 alpha-2 code.");

        Line1 = line1.Trim();
        Line2 = line2?.Trim();
        City = city.Trim();
        State = state.Trim();
        PostalCode = postalCode.Trim().ToUpperInvariant();
        Country = country.Trim().ToUpperInvariant();
    }

    public override string ToString() =>
        string.IsNullOrWhiteSpace(Line2)
            ? $"{Line1}, {City}, {State} {PostalCode}, {Country}"
            : $"{Line1}, {Line2}, {City}, {State} {PostalCode}, {Country}";
}