using System.ComponentModel.DataAnnotations;

namespace StockFlow.Application.Inventory;

public sealed record GetInventoryMovementsQuery(
    [Range(1, int.MaxValue)]
    int Page =1,
    int PageSize = 10,
    InventoryMovementType? Type = null,
    DateTime? From = null,
    DateTime? To = null,
    string SortOrder = "desc"
);