using StockFlow.Application.Inventory.Exceptions;

namespace StockFlow.Application.Inventory;

public sealed class InventoryService
{
    private readonly IInventoryRepository _inventoryRepository;

    public InventoryService(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<IReadOnlyList<InventoryResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var inventorys = await _inventoryRepository.GetAllAsync(cancellationToken);
        return inventorys;
    }

    public async Task<IReadOnlyList<InventoryMovementResponse>> GetMovementsAsync(CancellationToken cancellationToken)
    {
        var inventorys = await _inventoryRepository.GetMovementsAsync(cancellationToken);
        return inventorys;
    }

    public async Task StockInAsync(
        Guid productId,
        StockInRequest request,
        CancellationToken cancellationToken)
    {
        await _inventoryRepository.StockInAsync(
            productId,
            request,
            cancellationToken);
    }

    public async Task StockOutAsync(
        Guid productId,
        StockOutRequest request,
        CancellationToken cancellationToken)
    {
        int affectedRows =
            await _inventoryRepository.StockOutAsync(
                productId,
                request,
                cancellationToken);

        if (affectedRows == 0)
        {
            throw new InsufficientInventoryException();
        }
    }
}