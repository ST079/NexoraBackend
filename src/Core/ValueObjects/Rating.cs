using NexoraBackend.Core.Exceptions;

namespace NexoraBackend.Core.ValueObjects;

/// Rating enforces the 1–5 star constraint at the type level.
/// Using a value object instead of int means you can never accidentally
/// store a rating of 0 or 99 — the compiler and constructor prevent it.

public sealed record Rating
{
    public int Value { get; }

    private Rating() { } // EF Core

    public Rating(int value)
    {
        if (value is < 1 or > 5)  // is just like if(value < 1 || value > 5)
            throw new DomainException("Rating must be between 1 and 5.");
        Value = value;
    }

    public static Rating One => new(1);
    public static Rating Five => new(5);

    public override string ToString() => $"{Value}/5";
    public static implicit operator int(Rating rating) => rating.Value;
}