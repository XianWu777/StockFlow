using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StockFlow.Application.Common;
using StockFlow.Application.Inventory;
using StockFlow.Infrastructure.Data;

namespace StockFlow.Infrastructure.Inventories;

public sealed class EfInventoryMovementRepository : IInventoryMovementRepository
{
    private readonly StockFlowDbContext _dbContext;

    public EfInventoryMovementRepository(StockFlowDbContext dbContext)
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

    public async Task<PagedResult<InventoryMovementResponse>> GetByQueryAsync(
        Guid productId,
        GetInventoryMovementsQuery query,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"GetByQueryAsync {JsonConvert.SerializeObject(query)}");
        int page = query.Page;
        int pageSize = query.PageSize;
        var dbQuery = _dbContext.InventoryMovements
            .AsNoTracking()
            .Where(x => x.ProductId == productId);

        if (query.Type.HasValue)
        {
            dbQuery = dbQuery.Where(x => x.Type == query.Type);
        }

        if (query.From.HasValue)
        {
            dbQuery = dbQuery.Where(x => x.CreatedAt.ToLocalTime() >= query.From);
        }

        if (query.To.HasValue)
        {
            dbQuery = dbQuery.Where(x => x.CreatedAt.ToLocalTime() <= query.To);
        }

        int totalCount = await _dbContext.InventoryMovements.CountAsync(cancellationToken);

        if (query.SortOrder == "desc")
        {
            dbQuery = dbQuery.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id);
        }
        else
        {
            dbQuery = dbQuery.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id);
        }

        var movements = await dbQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(movement => new InventoryMovementResponse(
                movement.Id,
                movement.ProductId,
                movement.Type,
                movement.Quantity,
                movement.CreatedAt))
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<InventoryMovementResponse>(
            movements,
            page,
            pageSize,
            totalCount);

        Console.WriteLine($"pagedResult : {JsonConvert.SerializeObject(pagedResult)}");

        return pagedResult;
    }
}