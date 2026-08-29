using MarketPlace.Identity.Api.Data;
using MarketPlace.Identity.Api.Entities;
using MarketPlace.Identity.Api.Features.Refresh;
using MarketPlace.Identity.Api.Services;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace MarketPlace.Identity.Tests;

public class RefreshCommandHandlerTests
{
    private static IdentityDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new IdentityDbContext(options);
    }

    [Fact]
    public async Task Handle_Should_RotateToken_WhenRefreshTokenIsValid()
    {
        // Arrange
        using var db = CreateInMemoryDbContext();
        var jwtGenerator = Substitute.For<IJwtTokenGenerator>();

        var user = new User { Email = "user@test.com", PasswordHash = "hash", Role = "Buyer" };
        db.Users.Add(user);

        var oldToken = new RefreshToken
        {
            UserId = user.Id,
            Token = "valid_old_token",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            User = user
        };
        db.RefreshTokens.Add(oldToken);
        await db.SaveChangesAsync();

        jwtGenerator.GenerateToken(Arg.Any<User>())
            .Returns(("new.jwt.token", DateTime.UtcNow.AddMinutes(15)));

        var handler = new RefreshCommandHandler(jwtGenerator, db);
        var command = new RefreshCommand("valid_old_token");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual("valid_old_token", result.Value.RefreshToken);

        // Eski token revoke edilmiş ve ReplacedByToken atanmış mı?
        var updatedOldToken = await db.RefreshTokens.SingleAsync(t => t.Token == "valid_old_token");
        Assert.NotNull(updatedOldToken.RevokedAt);
        Assert.Equal(result.Value.RefreshToken, updatedOldToken.ReplacedByToken);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessWithSameToken_WhenRevokedWithinGracePeriod()
    {
        // Arrange (20 saniye önce iptal edilmiş ve yerine newToken üretilmiş bir token simüle ediyoruz)
        using var db = CreateInMemoryDbContext();
        var jwtGenerator = Substitute.For<IJwtTokenGenerator>();

        var user = new User { Email = "user@test.com", PasswordHash = "hash", Role = "Buyer" };
        db.Users.Add(user);

        var newToken = new RefreshToken
        {
            UserId = user.Id,
            Token = "already_generated_new_token",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            User = user
        };
        var oldToken = new RefreshToken
        {
            UserId = user.Id,
            Token = "parallel_request_old_token",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            RevokedAt = DateTime.UtcNow.AddSeconds(-20), // 20 sn önce revoke edilmiş (Grace Period içinde)
            ReplacedByToken = "already_generated_new_token",
            User = user
        };
        db.RefreshTokens.AddRange(newToken, oldToken);
        await db.SaveChangesAsync();

        jwtGenerator.GenerateToken(Arg.Any<User>())
            .Returns(("grace.jwt.token", DateTime.UtcNow.AddMinutes(15)));

        var handler = new RefreshCommandHandler(jwtGenerator, db);
        var command = new RefreshCommand("parallel_request_old_token");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("already_generated_new_token", result.Value.RefreshToken);
    }

    [Fact]
    public async Task Handle_Should_RevokeAllSessions_WhenTokenIsReusedBeyondGracePeriod()
    {
        // Arrange (70 saniye önce iptal edilmiş eski token - Olası Hacker Replay Saldırısı!)
        using var db = CreateInMemoryDbContext();
        var jwtGenerator = Substitute.For<IJwtTokenGenerator>();

        var user = new User { Email = "user@test.com", PasswordHash = "hash", Role = "Buyer" };
        db.Users.Add(user);

        // Kullanıcının başka bir cihazdaki aktif oturumu
        var activeOtherSession = new RefreshToken
        {
            UserId = user.Id,
            Token = "active_phone_token",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            User = user
        };
        // 70 saniye önce çalınmış ve süresi geçmiş token
        var stolenOldToken = new RefreshToken
        {
            UserId = user.Id,
            Token = "stolen_old_token",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            RevokedAt = DateTime.UtcNow.AddSeconds(-70), // Grace period (60sn) DIŞINDA!
            ReplacedByToken = "some_token",
            User = user
        };
        db.RefreshTokens.AddRange(activeOtherSession, stolenOldToken);
        await db.SaveChangesAsync();

        var handler = new RefreshCommandHandler(jwtGenerator, db);
        var command = new RefreshCommand("stolen_old_token");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Identity.TokenReuseDetected", result.Error.Code);

        // Güvenlik Doğrulaması: Kullanıcının diğer aktif oturumu da veritabanında İPTAL EDİLDİ Mİ?
        var phoneTokenInDb = await db.RefreshTokens.SingleAsync(t => t.Token == "active_phone_token");
        Assert.NotNull(phoneTokenInDb.RevokedAt); // Oturum patlatıldı!
    }
}
