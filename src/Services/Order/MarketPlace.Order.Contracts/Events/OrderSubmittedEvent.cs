namespace MarketPlace.Order.Contracts.Events;

public sealed record OrderSubmittedEvent(
    Guid OrderId,
    Guid ProductId,
    int Quantity,
    decimal TotalAmount,
    DateTime OccurredAtUtc);
