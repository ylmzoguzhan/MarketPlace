using MarketPlace.Identity.Api.Data;
using MarketPlace.Identity.Api.Entities;
using MarketPlace.Identity.Api.Features.Login;
using MarketPlace.Identity.Api.Services;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace MarketPlace.Identity.Tests;

public class LoginCommandHandlerTests
{
    private static IdentityDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new IdentityDbContext(options);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenCredentialsAreValid()
    {
        // Arrange
        using var db = CreateInMemoryDbContext();
        var jwtGenerator = Substitute.For<IJwtTokenGenerator>();

        var user = new User
        {
            Email = "loginuser@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("SecretPassword123!"),
            Role = "Buyer",
            CreatedAt = DateTime.UtcNow
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        jwtGenerator.GenerateToken(Arg.Any<User>())
            .Returns(("mock.access.token", DateTime.UtcNow.AddMinutes(15)));

        var handler = new LoginCommandHandler(db, jwtGenerator);
        var command = new LoginCommand("loginuser@example.com", "SecretPassword123!");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("mock.access.token", result.Value.AccessToken);
        Assert.False(string.IsNullOrWhiteSpace(result.Value.RefreshToken));

        // DB'ye RefreshToken kaydedilmiş mi?
        var savedRefreshToken = await db.RefreshTokens.SingleOrDefaultAsync(t => t.UserId == user.Id);
        Assert.NotNull(savedRefreshToken);
        Assert.Equal(result.Value.RefreshToken, savedRefreshToken.Token);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenPasswordIsIncorrect()
    {
        // Arrange
        using var db = CreateInMemoryDbContext();
        var jwtGenerator = Substitute.For<IJwtTokenGenerator>();

        var user = new User
        {
            Email = "user@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword!"),
            Role = "Buyer",
            CreatedAt = DateTime.UtcNow
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var handler = new LoginCommandHandler(db, jwtGenerator);
        var command = new LoginCommand("user@example.com", "WrongPassword!");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Identity.InvalidCredentials", result.Error.Code);
    }
}
