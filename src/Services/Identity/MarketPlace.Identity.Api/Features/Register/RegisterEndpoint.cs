using MarketPlace.Shared.Domain.Results;
using MediatR;

namespace MarketPlace.Identity.Api.Features.Register;

public static class RegisterEndpoint
{
    public static void MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/identity/register", async (RegisterCommand command, ISender sender, CancellationToken ct) =>
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

                if (result.Error.Code == "Identity.EmailInUse")
                {
                    return Results.Conflict(new { error = result.Error.Message });
                }

                return Results.BadRequest(new { error = result.Error.Message });
            }

            return Results.Created($"/api/v1/identity/users/{result.Value}", new { id = result.Value });
        })
        .WithName("Register")
        .WithTags("Identity");
    }
}
