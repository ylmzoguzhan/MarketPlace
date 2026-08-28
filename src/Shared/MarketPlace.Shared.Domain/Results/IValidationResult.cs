namespace MarketPlace.Shared.Domain.Results;

public interface IValidationResult
{
    Error[] ValidationErrors { get; }
    static abstract IValidationResult WithErrors(Error[] errors);
}
