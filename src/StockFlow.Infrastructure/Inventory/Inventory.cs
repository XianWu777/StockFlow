using System.ComponentModel.DataAnnotations;
using StockFlow.Infrastructure.Products;

namespace StockFlow.Infrastructure.Inventories;

public sealed record Inventory
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Product Product { get; set; } = null!;

    // [Timestamp] // EF Core 會自動將此欄位視為樂觀鎖版本號
    public int Version { get; set; }
}