namespace MarketPlace.Payment.Contracts.Events;

public sealed record PaymentRefundedEvent(
    Guid OrderId,
    DateTime OccurredAtUtc);
