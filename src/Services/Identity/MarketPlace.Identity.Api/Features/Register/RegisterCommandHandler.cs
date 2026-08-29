using MarketPlace.Identity.Api.Data;
using MarketPlace.Identity.Api.Entities;
using MarketPlace.Shared.Domain.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Identity.Api.Features.Register;

public sealed class RegisterCommandHandler(IdentityDbContext db) : IRequestHandler<RegisterCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // 1. Email kontrolü
        var emailExists = await db.Users.AnyAsync(u => u.Email == normalizedEmail, cancellationToken);
        if (emailExists)
        {
            return Result.Failure<Guid>(Error.Conflict("Identity.EmailInUse", "Email is already in use."));
        }

        // 2. Hash & Create
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Email = normalizedEmail,
            PasswordHash = passwordHash,
            Role = string.IsNullOrWhiteSpace(request.Role) ? "Buyer" : request.Role.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(user.Id);
    }
}
