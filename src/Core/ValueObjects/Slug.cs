using System.Text.RegularExpressions;
using NexoraBackend.Core.Exceptions;

namespace NexoraBackend.Core.ValueObjects;

public sealed record Slug
{
    private static readonly Regex SlugRegex = new(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.Compiled);
    public string Value { get; }

    private Slug() { } // for EF Core

    public Slug(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Slug cannot be empty.");
        if (!SlugRegex.IsMatch(value))
            throw new DomainException($"{value} is not a valid slug format. Slugs can only contain lowercase letters, numbers, and hyphens.");

        Value = value;
    }

    public static Slug Create(string input)
    {
        var slug = input.ToLowerInvariant() // Convert to lowercase
                        .Trim() // Remove leading and trailing whitespace
                        .Replace(" ", "-") // Replace spaces with hyphens
                        .Replace("_", "-"); // Replace underscores with hyphens

        slug = Regex.Replace(slug, @"[^a-z0-9\-]", ""); // Remove any characters that are not lowercase letters, numbers, or hyphens
        slug = Regex.Replace(slug, @"-{2,}", "-"); // Replace multiple hyphens with a single one

        return new Slug(slug);
    }

    public override string ToString() => Value; // For easy display and debugging
}