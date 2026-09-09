namespace StockFlow.Application.Inventory;

public sealed record InventoryMovementDto(
    Guid Id,
    Guid ProductId,
    InventoryMovementType Type,
    int Quantity,
    DateTime CreatedAt
);