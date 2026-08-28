namespace MarketPlace.Payment.Contracts.Events;

public sealed record PaymentCompletedEvent(
    Guid OrderId,
    Guid TransactionId,
    DateTime OccurredAtUtc);
