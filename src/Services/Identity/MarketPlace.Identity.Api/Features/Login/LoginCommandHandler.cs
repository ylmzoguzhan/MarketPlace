using MarketPlace.Identity.Api.Data;
using MarketPlace.Identity.Api.Entities;
using MarketPlace.Identity.Api.Services;
using MarketPlace.Shared.Domain.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Identity.Api.Features.Login;

public sealed class LoginCommandHandler(IdentityDbContext db, IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await db.Users.SingleOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
        if (user is null)
        {
            return Result.Failure<LoginResponse>(Error.Failure("Identity.InvalidCredentials", "Invalid email or password."));
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Result.Failure<LoginResponse>(Error.Failure("Identity.InvalidCredentials", "Invalid email or password."));
        }

        var (accessToken, expiresAt) = jwtTokenGenerator.GenerateToken(user);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = Guid.NewGuid().ToString("N"),
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new LoginResponse(accessToken, refreshToken.Token, expiresAt));
    }
}
