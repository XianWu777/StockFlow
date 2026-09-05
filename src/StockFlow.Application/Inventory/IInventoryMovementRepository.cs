using StockFlow.Application.Inventory;

public interface IInventoryMovementRepository
{
    Task<IReadOnlyList<InventoryMovementResponse>> GetAllAsync(
        CancellationToken cancellationToken);

    Task AddAsync(
        InventoryMovementDraft movement,
        CancellationToken cancellationToken);
}