namespace StockFlow.Infrastructure.Inventories;

public sealed class InventoryMovement
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Type { get; set; } = null!;
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
}