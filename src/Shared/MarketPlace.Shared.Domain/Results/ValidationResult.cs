namespace MarketPlace.Shared.Domain.Results;

public class ValidationResult : Result, IValidationResult
{
    public override Error[] ValidationErrors { get; }
    private ValidationResult(Error[] errors) : base(false, Error.Validation("ValidationError", "A validation problem occurred."))
    {
        ValidationErrors = errors;
    }
    public static new IValidationResult WithErrors(Error[] errors) => new ValidationResult(errors);
}

public sealed class ValidationResult<TValue> : Result<TValue>, IValidationResult
{
    public override Error[] ValidationErrors { get; }
    private ValidationResult(Error[] errors) : base(default, false, Error.Validation("ValidationError", "A validation problem occurred."))
    {
        ValidationErrors = errors;
    }
    public static new IValidationResult WithErrors(Error[] errors) => new ValidationResult<TValue>(errors);

}