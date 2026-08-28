namespace MarketPlace.Inventory.Contracts.Events;

public sealed record StockReservationFailedEvent(
    Guid OrderId,
    string Reason,
    DateTime OccurredAtUtc);
