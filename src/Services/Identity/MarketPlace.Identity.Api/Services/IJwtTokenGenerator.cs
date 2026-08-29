using MarketPlace.Identity.Api.Entities;

namespace MarketPlace.Identity.Api.Services;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}
