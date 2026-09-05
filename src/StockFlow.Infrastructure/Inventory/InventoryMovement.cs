using StockFlow.Application.Inventory;

namespace StockFlow.Infrastructure.Inventories;

public sealed class InventoryMovement
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public InventoryMovementType Type { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
}