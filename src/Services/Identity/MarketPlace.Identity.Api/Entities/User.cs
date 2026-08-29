using MarketPlace.Shared.Domain.Primitives;

namespace MarketPlace.Identity.Api.Entities;

public class User : Entity
{
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = "Buyer";
    public DateTime CreatedAt { get; set; }
}