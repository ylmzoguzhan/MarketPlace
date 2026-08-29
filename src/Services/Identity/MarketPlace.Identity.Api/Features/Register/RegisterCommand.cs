using MarketPlace.Shared.Domain.Results;
using MediatR;

namespace MarketPlace.Identity.Api.Features.Register;

public record RegisterCommand(string Email, string Password, string? Role) : IRequest<Result<Guid>>;
