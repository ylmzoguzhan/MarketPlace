using System.Diagnostics.CodeAnalysis;

namespace MarketPlace.Shared.Domain.Results;

/// <summary>
/// Represents the outcome of an operation without returning a payload.
/// </summary>
public class Result : IValidationResult
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new ArgumentException("A successful result cannot contain an error.", nameof(error));
        }

        if (!isSuccess && error == Error.None)
        {
            throw new ArgumentException("A failed result must contain an error.", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

    public static Result<TValue> Create<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    public virtual Error[] ValidationErrors => [];
    public static IValidationResult WithErrors(Error[] errors) => ValidationResult.WithErrors(errors);
}

/// <summary>
/// Represents the outcome of an operation with a strongly-typed payload on success.
/// </summary>
/// <typeparam name="TValue">The payload type.</typeparam>
public class Result<TValue> : Result, IValidationResult
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result cannot be accessed.");

    public static new IValidationResult WithErrors(Error[] errors) => ValidationResult<TValue>.WithErrors(errors);
}
