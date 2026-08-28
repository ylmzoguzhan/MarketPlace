namespace MarketPlace.Payment.Contracts.Events;

public sealed record PaymentFailedEvent(
    Guid OrderId,
    string Reason,
    DateTime OccurredAtUtc);
