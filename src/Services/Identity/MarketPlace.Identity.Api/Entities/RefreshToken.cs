using MarketPlace.Shared.Domain.Primitives;

namespace MarketPlace.Identity.Api.Entities;

public class RefreshToken : Entity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? CreatedByIp { get; set; }
    public uint Version { get; set; }
    public User User { get; set; } = null!;
}
