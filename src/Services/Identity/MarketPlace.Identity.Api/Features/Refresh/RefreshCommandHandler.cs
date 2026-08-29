using MarketPlace.Identity.Api.Data;
using MarketPlace.Identity.Api.Entities;
using MarketPlace.Identity.Api.Features.Login;
using MarketPlace.Identity.Api.Services;
using MarketPlace.Shared.Domain.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace MarketPlace.Identity.Api.Features.Refresh;

public class RefreshCommandHandler(IJwtTokenGenerator jwtTokenGenerator, IdentityDbContext dbContext) : IRequestHandler<RefreshCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var token = await dbContext.RefreshTokens.Include(x => x.User).SingleOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken);
        if (token == null)
            return Result.Failure<LoginResponse>(Error.Failure("Identity.TokenNotFound", "Invalid refresh token."));
        if (token.ExpiresAt < DateTime.UtcNow)
            return Result.Failure<LoginResponse>(Error.Failure("Identity.TokenExpired", "Refresh token has expired."));
        if (token.RevokedAt is not null)
        {
            if (token.RevokedAt >= DateTime.UtcNow.AddSeconds(-60) && token.ReplacedByToken is not null)
            {
                var existingNewToken = await dbContext.RefreshTokens.SingleAsync(t => t.Token == token.ReplacedByToken, cancellationToken);
                var (accToken, expAt) = jwtTokenGenerator.GenerateToken(token.User);
                return Result.Success(new LoginResponse(accToken, existingNewToken.Token, expAt));
            }
            var activeTokens = await dbContext.RefreshTokens
               .Where(t => t.UserId == token.UserId && t.RevokedAt == null)
               .ToListAsync(cancellationToken);
            foreach (var activeToken in activeTokens)
                activeToken.RevokedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);
            return Result.Failure<LoginResponse>(Error.Failure("Identity.TokenReuseDetected", "Token reuse detected. All sessions revoked."));
        }
        var newRefreshToken = new RefreshToken
        {
            UserId = token.UserId,
            Token = Guid.NewGuid().ToString("N"),
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        token.RevokedAt = DateTime.UtcNow;
        token.ReplacedByToken = newRefreshToken.Token;
        dbContext.RefreshTokens.Add(newRefreshToken);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            var current = await dbContext.RefreshTokens
                .AsNoTracking()
                .SingleAsync(t => t.Token == request.RefreshToken, cancellationToken);
            if (current.RevokedAt >= DateTime.UtcNow.AddSeconds(-60) && current.ReplacedByToken is not null)
            {
                var existing = await dbContext.RefreshTokens
                    .SingleAsync(t => t.Token == current.ReplacedByToken, cancellationToken);
                var (concurrencyAccessToken, concurrencyExpiresAt) = jwtTokenGenerator.GenerateToken(token.User);
                return Result.Success(new LoginResponse(concurrencyAccessToken, existing.Token, concurrencyExpiresAt));
            }
            throw;
        }
        var (accessToken, expiresAt) = jwtTokenGenerator.GenerateToken(token.User);
        return Result.Success(new LoginResponse(accessToken, newRefreshToken.Token, expiresAt));
    }
}
