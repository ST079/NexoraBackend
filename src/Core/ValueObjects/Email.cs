using System.Text.RegularExpressions;
using NexoraBackend.Core.Exceptions;

namespace NexoraBackend.Core.ValueObjects;

//why use? => email becomes safe + validated + meaningful.
public sealed record Email
{
    //pattern of the email
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public string Value { get; }

    private Email() { } // for EF Core

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email cannot be empty.");
        if (!EmailRegex.IsMatch(value))
            throw new DomainException($"{value} is not a valid email format.");

        Value = value.ToLowerInvariant(); // Normalize to lowercase for consistency and case-insensitive comparisons
        //USER@GMAIL.COM → user@gmail.com
    }
    public override string ToString() => Value; // For easy display and debugging

    public static implicit operator string(Email email) => email.Value; // Allow implicit conversion to string for convenience
}

/*
Email email = new Email("test@gmail.com");
string s = email; // works automatically
*/