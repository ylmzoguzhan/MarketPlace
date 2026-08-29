using MarketPlace.Identity.Api.Data;
using MarketPlace.Identity.Api.Features.Register;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Identity.Tests;

public class RegisterCommandHandlerTests
{
    private static IdentityDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new IdentityDbContext(options);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenEmailIsUnique()
    {
        // Arrange
        using var db = CreateInMemoryDbContext();
        var handler = new RegisterCommandHandler(db);
        var command = new RegisterCommand("newuser@example.com", "Password123!", "Buyer");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);

        var savedUser = await db.Users.SingleOrDefaultAsync(u => u.Id == result.Value);
        Assert.NotNull(savedUser);
        Assert.Equal("newuser@example.com", savedUser.Email);
        Assert.True(BCrypt.Net.BCrypt.Verify("Password123!", savedUser.PasswordHash));
    }

    [Fact]
    public async Task Handle_Should_ReturnConflict_WhenEmailAlreadyExists()
    {
        // Arrange
        using var db = CreateInMemoryDbContext();
        var handler = new RegisterCommandHandler(db);

        // İlk kullanıcıyı kaydet
        var firstCommand = new RegisterCommand("existing@example.com", "Password123!", "Buyer");
        await handler.Handle(firstCommand, CancellationToken.None);

        // İkinci istek (aynı email, farklı harf büyüklüğü)
        var duplicateCommand = new RegisterCommand("EXISTING@example.com", "DifferentPassword!", "Buyer");

        // Act
        var result = await handler.Handle(duplicateCommand, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Identity.EmailInUse", result.Error.Code);
    }
}
