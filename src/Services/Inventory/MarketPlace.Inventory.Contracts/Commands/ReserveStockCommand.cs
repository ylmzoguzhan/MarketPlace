namespace MarketPlace.Inventory.Contracts.Commands;

public sealed record ReserveStockCommand(
    Guid OrderId,
    Guid ProductId,
    int Quantity,
    DateTime OccurredAtUtc);
