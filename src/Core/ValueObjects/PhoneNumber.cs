namespace NexoraBackend.Core.ValueObjects;

using System.Text.RegularExpressions;
using NexoraBackend.Core.Exceptions;

/// <summary>
/// Represents an E.164-format international phone number.
/// Why a value object? Phone numbers have format rules and canonicalization
/// logic that doesn't belong scattered across the codebase.
/// </summary>
public sealed record PhoneNumber
{
    // E.164: +[country code][number], 7–15 digits total
    private static readonly Regex PhoneRegex = new(
        @"^\+[1-9]\d{6,14}$",
        RegexOptions.Compiled);

    public string Value { get; }

    private PhoneNumber() { } // EF Core

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Phone number cannot be empty.");

        // Strip spaces and dashes before validating
        //+977 981-234-5678
        //(+977) 9812345678
        //to +9779812345678 for validation and storage.
        var normalized = value.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

        if (!PhoneRegex.IsMatch(normalized))
            throw new DomainException($"'{value}' is not a valid E.164 phone number (e.g. +14155552671).");

        Value = normalized;
    }

    public override string ToString() => Value;
    public static implicit operator string(PhoneNumber phone) => phone.Value;
    /*
    Allows: string phone = user.PhoneNumber;
    Instead of: user.PhoneNumber.Value
    */
}