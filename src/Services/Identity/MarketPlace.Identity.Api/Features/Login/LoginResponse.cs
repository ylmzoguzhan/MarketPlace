namespace MarketPlace.Identity.Api.Features.Login;

public record LoginResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);
