using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Inventory;
using StockFlow.Infrastructure.Data;

namespace StockFlow.Infrastructure.Inventories;

public sealed class InventoryMovementRepository : IInventoryMovementRepository
{
    private readonly StockFlowDbContext _dbContext;

    public InventoryMovementRepository(StockFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(
        InventoryMovementDraft movement,
        CancellationToken cancellationToken)
    {
        var entity = new InventoryMovement
        {
            ProductId = movement.ProductId,
            Quantity = movement.Quantity,
            Type = movement.Type,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Add(entity);

        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryMovementResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var movements = await _dbContext.InventoryMovements
            .AsNoTracking()
            .Select(movement => new InventoryMovementResponse(
                movement.Id,
                movement.ProductId,
                movement.Type,
                movement.Quantity,
                movement.CreatedAt))
            .ToListAsync(cancellationToken);

        return movements;
    }
}