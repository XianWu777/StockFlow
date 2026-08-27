using System.ComponentModel.DataAnnotations;

namespace StockFlow.Application.Products;

public sealed record CreateProductRequest(
    [param: Required]
    [param: MinLength(2)]
    string Name,
    string Description,
    [param: Range(0.01, 999999999)]
    decimal Price);