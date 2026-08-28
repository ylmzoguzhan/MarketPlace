using MarketPlace.Shared.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5020");

builder.Services.AddSharedInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSharedInfrastructure();

app.MapGet("/health", () => Results.Ok("healthy"));

app.Run();
