using MarketPlace.Shared.Domain.Results;
using MediatR;

namespace MarketPlace.Identity.Api.Features.Login;

public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;
