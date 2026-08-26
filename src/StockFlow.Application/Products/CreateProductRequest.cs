using System.ComponentModel.DataAnnotations;

namespace StockFlow.Application.Products;

public sealed record CreateProductRequest(
    [property: Required]
    [property: MinLength(2)]
    string Name,
    string Description,
    [property: Range(0.01, 999999999)]
    decimal Price);