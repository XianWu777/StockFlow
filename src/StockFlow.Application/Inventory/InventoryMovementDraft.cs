namespace StockFlow.Application.Inventory;

public sealed record InventoryMovementDraft(
    Guid ProductId,
    InventoryMovementType Type,
    int Quantity
);