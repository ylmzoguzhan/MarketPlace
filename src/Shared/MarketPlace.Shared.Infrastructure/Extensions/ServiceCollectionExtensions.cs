using MarketPlace.Shared.Infrastructure.Behaviors;
using MarketPlace.Shared.Infrastructure.Middleware;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace MarketPlace.Shared.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        var redisConnection = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnection));


        return services;
    }
    public static IApplicationBuilder UseSharedInfrastructure(this IApplicationBuilder app)
    {
        app.UseExceptionHandler();
        app.UseMiddleware<IdempotencyMiddleware>();
        return app;
    }
}
