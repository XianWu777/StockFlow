namespace StockFlow.Infrastructure.Entity;

public sealed record Inventory
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime UpdatedAt { get; set; }
    public required Product Product { get; set; }
}