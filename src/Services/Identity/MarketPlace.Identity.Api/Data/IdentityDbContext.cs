using MarketPlace.Identity.Api.Data.Configurations;
using MarketPlace.Identity.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Identity.Api.Data;

public class IdentityDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }
}
