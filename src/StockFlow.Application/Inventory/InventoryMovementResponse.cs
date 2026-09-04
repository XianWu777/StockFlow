namespace StockFlow.Application.Inventory;

public sealed record class InventoryMovementResponse(
    Guid Id,
    Guid ProductId,
    string Type,
    int Quantity,
    DateTime CreatedAt);