namespace StockFlow.Application.Products;

public sealed record class ProductResponse(
    Guid id,
    string Name,
    string Description,
    decimal Price,
    bool IsActive);