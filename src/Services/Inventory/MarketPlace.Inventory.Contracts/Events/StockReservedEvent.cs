namespace MarketPlace.Inventory.Contracts.Events;

public sealed record StockReservedEvent(
    Guid OrderId,
    DateTime OccurredAtUtc);
