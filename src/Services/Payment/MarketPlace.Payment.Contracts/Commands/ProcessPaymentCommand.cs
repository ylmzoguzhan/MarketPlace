namespace MarketPlace.Payment.Contracts.Commands;

public sealed record ProcessPaymentCommand(
    Guid OrderId,
    decimal Amount,
    string Currency,
    DateTime OccurredAtUtc);
