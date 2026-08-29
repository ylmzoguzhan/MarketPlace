using MarketPlace.Identity.Api.Features.Login;
using MarketPlace.Shared.Domain.Results;
using MediatR;

namespace MarketPlace.Identity.Api.Features.Refresh;

public record RefreshCommand(string RefreshToken) : IRequest<Result<LoginResponse>>;
