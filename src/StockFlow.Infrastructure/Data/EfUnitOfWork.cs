namespace StockFlow.Infrastructure.Data;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly StockFlowDbContext _dbContext;

    public EfUnitOfWork(StockFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        if (_dbContext.Database.CurrentTransaction is not null)
        {
            await _dbContext.Database.CommitTransactionAsync(cancellationToken);
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        if(_dbContext.Database.CurrentTransaction is not null)
        {
            await _dbContext.Database.RollbackTransactionAsync(cancellationToken);
        }
    }
}