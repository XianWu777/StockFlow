using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StockFlow.Application.Inventory;
using StockFlow.Infrastructure.Data;

namespace StockFlow.Infrastructure.Inventories;

public sealed class EfInventoryRepository : IInventoryRepository
{
    private readonly StockFlowDbContext _dbContext;

    public EfInventoryRepository(StockFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<InventoryResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var inventories = await _dbContext.Inventories
            .AsNoTracking()
            .Select(inventory => new InventoryResponse(
                inventory.Id,
                inventory.ProductId,
                inventory.Quantity,
                inventory.UpdatedAt))
            .ToListAsync(cancellationToken);

        return inventories;
    }

    public async Task<IReadOnlyList<InventoryMovementResponse>> GetMovementsAsync(CancellationToken cancellationToken)
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

    public async Task StockInAsync(Guid productId, StockInRequest request, CancellationToken cancellationToken)
    {
        var inventory = await _dbContext.Inventories
        .SingleOrDefaultAsync(
            x => x.ProductId == productId,
            cancellationToken);

        if (inventory is null)
        {
            Console.WriteLine($"Inventory not found for product {productId}");
            throw new KeyNotFoundException($"Inventory not found for product {productId}");
        }

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            // 第一步：修改資料
            inventory.Quantity += request.Quantity;
            inventory.UpdatedAt = DateTime.UtcNow;
            inventory.Version++;

            var movement = new InventoryMovement
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Type = InventoryMovementType.StockIn,
                Quantity = request.Quantity,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.InventoryMovements.Add(movement);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            Console.WriteLine("StockIn ConcurrencyException");
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception)
        {
            Console.WriteLine("StockIn ROLLBACK");
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<int> StockOutAsync(Guid productId, StockOutRequest request, CancellationToken cancellationToken)
    {
        int c = await _dbContext.Inventories
            .Where(x =>
                x.ProductId == productId &&
                x.Quantity >= request.Quantity)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        x => x.Quantity,
                        x => x.Quantity - request.Quantity)
                    .SetProperty(
                        x => x.UpdatedAt,
                        x => DateTime.UtcNow
                    ),
            cancellationToken);

        Console.WriteLine($"StockOut UPDATE Version : {c}");
        return c;
        // Inventory? inventory = await _dbContext.Inventories
        // .SingleOrDefaultAsync(
        //     x => x.ProductId == productId,
        //     cancellationToken);

        // if (inventory is null)
        // {
        //     throw new KeyNotFoundException($"Inventory not found for product {productId}");
        // }

        // if (inventory.Quantity < request.Quantity)
        // {
        //     Console.WriteLine($"Not enough quantity for product {productId}");
        //     throw new Exception($"Not enough quantity for product {productId}");
        // }

        // await using var transaction =
        //     await _dbContext.Database.BeginTransactionAsync(
        //         cancellationToken);

        // try
        // {
        //     inventory.Quantity -= request.Quantity;
        //     inventory.UpdatedAt = DateTime.UtcNow;
        //     inventory.Version++;

        //     Console.WriteLine($"StockOut UPDATE Version : {inventory.Version} , Quantity : {inventory.Quantity}");

        //     var movement = new InventoryMovement
        //     {
        //         Id = Guid.NewGuid(),
        //         ProductId = productId,
        //         Type = "StockOut",
        //         Quantity = request.Quantity,
        //         CreatedAt = DateTime.UtcNow
        //     };

        //     _dbContext.InventoryMovements.Add(movement);
        //     await _dbContext.SaveChangesAsync(cancellationToken);
        //     await transaction.CommitAsync(cancellationToken);
        //     Console.WriteLine("StockOut COMMIT");
        // }
        // catch (DbUpdateConcurrencyException ex)
        // {
        //     Console.WriteLine("StockIn ConcurrencyException");
        //     await transaction.RollbackAsync(cancellationToken);
        //     throw;
        // }
        // catch
        // {
        //     Console.WriteLine("StockOut ROLLBACK");
        //     await transaction.RollbackAsync(cancellationToken);
        //     throw;
        // }
    }
}