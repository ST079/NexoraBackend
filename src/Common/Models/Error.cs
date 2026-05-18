namespace NexoraBackend.Common.Models;

public record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    // Common error codes
    public static Error NotFound(string entity, object id) =>
        new("NOT_FOUND", $"{entity} with ID '{id}' was not found.");

    public static Error Unauthorized() =>
        new("UNAUTHORIZED", "You are not authorized to perform this action.");

    public static Error Validation(string message) =>
        new("VALIDATION_ERROR", message);

    public static Error Conflict(string message) =>
        new("CONFLICT", message);

    public static Error Internal(string message) =>
        new("INTERNAL_ERROR", message);
}