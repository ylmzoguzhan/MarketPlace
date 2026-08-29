using MarketPlace.Identity.Api.Extensions;
using MarketPlace.Identity.Api.Features.Login;
using MarketPlace.Identity.Api.Features.Refresh;
using MarketPlace.Identity.Api.Features.Register;
using MarketPlace.Shared.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5010");

builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

app.UseSharedInfrastructure();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok("healthy"));
app.MapLoginEndpoint();
app.MapRegisterEndpoint();
app.MapRefreshEndpoint();
app.Run();
