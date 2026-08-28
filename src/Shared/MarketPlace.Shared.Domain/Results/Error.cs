namespace MarketPlace.Shared.Domain.Results;

/// <summary>
/// Represents a domain/application error with an explicit error code and human-readable message.
/// </summary>
/// <param name="Code">Unique machine-readable error code.</param>
/// <param name="Message">Human-readable description of the error.</param>
public sealed record Error(string Code, string Message)
{
    /// <summary>
    /// Represents no error (success sentinel).
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty);

    /// <summary>
    /// Represents a null value error when a non-null argument was required.
    /// </summary>
    public static readonly Error NullValue = new("Error.NullValue", "The specified value is null.");

    /// <summary>
    /// Creates a custom not-found error.
    /// </summary>
    public static Error NotFound(string code, string message) => new(code, message);

    /// <summary>
    /// Creates a custom validation error.
    /// </summary>
    public static Error Validation(string code, string message) => new(code, message);

    /// <summary>
    /// Creates a custom conflict error.
    /// </summary>
    public static Error Conflict(string code, string message) => new(code, message);

    /// <summary>
    /// Creates a custom failure error.
    /// </summary>
    public static Error Failure(string code, string message) => new(code, message);
}
