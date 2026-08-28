namespace MarketPlace.Inventory.Contracts.Commands;

public sealed record ConfirmReservationCommand(
    Guid OrderId,
    DateTime OccurredAtUtc);
