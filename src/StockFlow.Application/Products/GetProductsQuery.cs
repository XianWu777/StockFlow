using System.ComponentModel.DataAnnotations;

namespace StockFlow.Application.Products;

public sealed record GetProductsQuery(
    [param: Range(1, int.MaxValue)]
    int Page = 1,
    [param: Range(1, 100)]
    int PageSize = 10,
    string? Search = null,
    string SortBy = "name",
    string SortOrder = "asc");