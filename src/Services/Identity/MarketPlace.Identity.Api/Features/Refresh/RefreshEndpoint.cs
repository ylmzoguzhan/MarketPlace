using MarketPlace.Identity.Api.Features.Login;
using MarketPlace.Shared.Domain.Results;
using MediatR;

namespace MarketPlace.Identity.Api.Features.Refresh;

public static class RefreshEndpoint
{
    public static void MapRefreshEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/identity/refresh", async (RefreshCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);

            if (result.IsFailure)
            {
                if (result is IValidationResult validationResult && validationResult.ValidationErrors.Length > 0)
                {
                    return Results.BadRequest(new
                    {
                        title = result.Error.Message,
                        errors = validationResult.ValidationErrors.Select(e => new { property = e.Code, message = e.Message })
                    });
                }

                return Results.Unauthorized();
            }

            return Results.Ok(result.Value);
        })
        .WithName("RefreshToken")
        .WithTags("Identity");
    }
}
