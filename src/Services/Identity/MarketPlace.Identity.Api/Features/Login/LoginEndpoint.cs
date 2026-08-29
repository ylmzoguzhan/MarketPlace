using MarketPlace.Shared.Domain.Results;
using MediatR;

namespace MarketPlace.Identity.Api.Features.Login;

public static class LoginEndpoint
{
    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/identity/login", async (LoginCommand command, ISender sender, CancellationToken ct) =>
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

                if (result.Error.Code == "Identity.InvalidCredentials")
                {
                    return Results.Unauthorized();
                }

                return Results.BadRequest(new { error = result.Error.Message });
            }

            return Results.Ok(result.Value);
        })
        .WithName("Login")
        .WithTags("Identity");
    }
}
