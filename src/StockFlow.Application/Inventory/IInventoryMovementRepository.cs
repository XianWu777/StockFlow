using StockFlow.Application.Common;
using StockFlow.Application.Inventory;

public interface IInventoryMovementRepository
{
    Task<IReadOnlyList<InventoryMovementResponse>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<PagedResult<InventoryMovementResponse>> GetByQueryAsync(
        Guid productId,
        GetInventoryMovementsQuery query,
        CancellationToken cancellationToken);

    Task AddAsync(
        InventoryMovementDraft movement,
        CancellationToken cancellationToken);
}