using StockFlow.Application.Inventory.Exceptions;

namespace StockFlow.Application.Inventory;

public sealed class InventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IInventoryMovementRepository _inventoryMovementRepository;
    private readonly IUnitOfWork _unitOfWork;

    public InventoryService(
        IInventoryRepository inventoryRepository,
        IInventoryMovementRepository inventoryMovementRepository,
        IUnitOfWork unitOfWork)
    {
        _inventoryRepository = inventoryRepository;
        _inventoryMovementRepository = inventoryMovementRepository;
        _unitOfWork = unitOfWork;
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
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            int affectedRows =
                await _inventoryRepository.StockOutAsync(
                    productId,
                    request,
                    cancellationToken);

            if (affectedRows == 0)
            {
                Console.WriteLine($"Insufficient inventory for product: {productId}");
                throw new InsufficientInventoryException();
            }

            var movement = new InventoryMovementDraft(productId, InventoryMovementType.Out, request.Quantity);
            await _inventoryMovementRepository.AddAsync(movement, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}