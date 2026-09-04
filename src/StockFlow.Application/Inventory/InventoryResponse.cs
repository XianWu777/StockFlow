public sealed record class InventoryResponse(
    Guid Id,
    Guid ProductId,
    int Quantity,
    DateTime UpdatedAt);