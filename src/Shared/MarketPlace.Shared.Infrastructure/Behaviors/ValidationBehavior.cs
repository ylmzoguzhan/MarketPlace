using FluentValidation;
using MarketPlace.Shared.Domain.Results;
using MediatR;

namespace MarketPlace.Shared.Infrastructure.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IValidationResult
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var errors = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage))
            .Distinct()
            .ToArray();

        if (errors.Length != 0)
        {
            return (TResponse)TResponse.WithErrors(errors);
        }

        return await next();
    }
}
