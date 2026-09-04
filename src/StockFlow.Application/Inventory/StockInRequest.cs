using System.ComponentModel.DataAnnotations;

namespace StockFlow.Application.Inventory;

public sealed record StockInRequest(
    [param: Range(0, 999999999)]
    int Quantity);