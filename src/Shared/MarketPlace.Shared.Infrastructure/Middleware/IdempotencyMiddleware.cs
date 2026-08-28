using System.Text.Json;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;

namespace MarketPlace.Shared.Infrastructure.Middleware;

public class IdempotencyMiddleware(RequestDelegate next)
{
    private const string IdempotencyHeaderName = "Idempotency-Key";
    public async Task InvokeAsync(HttpContext context, IConnectionMultiplexer redis)
    {
        if (!HttpMethods.IsPost(context.Request.Method) && !HttpMethods.IsPut(context.Request.Method) && !HttpMethods.IsPatch(context.Request.Method))
        {
            await next(context);
            return;
        }
        if (!context.Request.Headers.TryGetValue(IdempotencyHeaderName, out var idempotencyKey) ||
            string.IsNullOrWhiteSpace(idempotencyKey))
        {
            await next(context);
            return;
        }

        var db = redis.GetDatabase();
        var cacheKey = $"idempotency:{idempotencyKey}";

        var cachedData = await db.StringGetAsync(cacheKey);
        if (cachedData.HasValue)
        {
            var response = JsonSerializer.Deserialize<IdempotentResponse>(cachedData.ToString());
            if (response is not null)
            {
                context.Response.StatusCode = response.StatusCode;
                context.Response.ContentType = response.ContentType;
                await context.Response.WriteAsync(response.Body);
                return;
            }
        }

        var originalBodyStream = context.Response.Body; // gerçek soket 
        using var responseBody = new MemoryStream(); // ram'de okunabilir 
        context.Response.Body = responseBody; // controller bunun üzerine yazacak

        try
        {
            await next(context);

            responseBody.Position = 0;
            var responseBodyText = await new StreamReader(responseBody).ReadToEndAsync();

            if (context.Response.StatusCode is >= 200 and < 300)
            {
                var idempotentResponse = new IdempotentResponse(
                    context.Response.StatusCode,
                    context.Response.ContentType ?? "application/json",
                    responseBodyText
                );

                await db.StringSetAsync(
                    cacheKey,
                    JsonSerializer.Serialize(idempotentResponse),
                    TimeSpan.FromHours(24)
                );
            }
            responseBody.Position = 0;
            await responseBody.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;

        }
    }
}

internal sealed record IdempotentResponse(int StatusCode, string ContentType, string Body);
