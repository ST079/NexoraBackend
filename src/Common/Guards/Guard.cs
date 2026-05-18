namespace NexoraBackend.Common.Guards;

public static class Guard
{
    public static T NotNull<T>(T? value, string paramName) where T : class =>
        value ?? throw new ArgumentNullException(paramName);

    public static string NotNullOrWhiteSpace(string? value, string paramName) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException($"{paramName} cannot be null or whitespace.", paramName)
            : value;

    public static int Positive(int value, string paramName) =>
        value > 0 ? value : throw new ArgumentException($"{paramName} must be positive.", paramName);

    public static decimal NonNegative(decimal value, string paramName) =>
        value >= 0 ? value : throw new ArgumentException($"{paramName} cannot be negative.", paramName);
}