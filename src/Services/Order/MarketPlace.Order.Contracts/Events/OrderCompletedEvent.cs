namespace MarketPlace.Order.Contracts.Events;

public sealed record OrderCompletedEvent(
    Guid OrderId,
    DateTime OccurredAtUtc);
