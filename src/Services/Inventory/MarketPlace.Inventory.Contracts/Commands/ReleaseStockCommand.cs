namespace MarketPlace.Inventory.Contracts.Commands;

public sealed record ReleaseStockCommand(
    Guid OrderId,
    DateTime OccurredAtUtc);
