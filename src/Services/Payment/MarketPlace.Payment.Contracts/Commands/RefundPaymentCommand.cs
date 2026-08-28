namespace MarketPlace.Payment.Contracts.Commands;

public sealed record RefundPaymentCommand(
    Guid OrderId,
    DateTime OccurredAtUtc);
