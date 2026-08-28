var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5000");

var app = builder.Build();

app.MapGet("/health", () => Results.Ok("healthy"));

app.Run();
