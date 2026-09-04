using System.ComponentModel.DataAnnotations;

namespace StockFlow.Application.Inventory;

public sealed record StockOutRequest(
    [param: Range(0, int.MaxValue)]
    int Quantity);