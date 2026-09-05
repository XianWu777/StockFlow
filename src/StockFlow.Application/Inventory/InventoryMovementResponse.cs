namespace StockFlow.Application.Inventory;

public sealed record class InventoryMovementResponse(
    Guid Id,
    Guid ProductId,
    InventoryMovementType Type,
    int Quantity,
    DateTime CreatedAt);