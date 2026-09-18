using Microsoft.Extensions.Logging;
using StockFlow.Application.Common;
using StockFlow.Application.Inventory.Exceptions;

namespace StockFlow.Application.Inventory;

public sealed class InventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IInventoryMovementRepository _inventoryMovementRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(
        IInventoryRepository inventoryRepository,
        IInventoryMovementRepository inventoryMovementRepository,
        IUnitOfWork unitOfWork,
        ILogger<InventoryService> logger)
    {
        _inventoryRepository = inventoryRepository;
        _inventoryMovementRepository = inventoryMovementRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IReadOnlyList<InventoryResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var inventorys = await _inventoryRepository.GetAllAsync(cancellationToken);
        return inventorys;
    }

    public async Task<IReadOnlyList<InventoryMovementResponse>> GetMovementsAsync(CancellationToken cancellationToken)
    {
        var inventorys = await _inventoryMovementRepository.GetAllAsync(cancellationToken);
        return inventorys;
    }

    public async Task<PagedResult<InventoryMovementResponse>> GetMovementsByProductIdAsync(Guid productId, GetInventoryMovementsQuery query, CancellationToken cancellationToken)
    {
        var inventorys = await _inventoryMovementRepository.GetByQueryAsync(productId, query, cancellationToken);
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
                _logger.LogWarning(
                    "Stock out rejected due to insufficient inventory. ProductId: {ProductId}, Quantity: {Quantity}",
                    productId,
                    request.Quantity);

                throw new InsufficientInventoryException();
            }

            var movement = new InventoryMovementDraft(productId, InventoryMovementType.StockOut, request.Quantity);
            await _inventoryMovementRepository.AddAsync(movement, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "StockOut succeeded. ProductId: {ProductId}, Quantity: {Quantity}",
                productId,
                request.Quantity);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}