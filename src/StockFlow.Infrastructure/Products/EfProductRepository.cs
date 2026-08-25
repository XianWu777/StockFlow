using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StockFlow.Application.Products;
using StockFlow.Infrastructure.Data;

namespace StockFlow.Infrastructure.Products;

public class EfProductRepository : IProductRepository
{
    private readonly StockFlowDbContext _dbContext;

    public EfProductRepository(StockFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Try to GetAllAsync");

        var products = await _dbContext.Products
            .AsNoTracking()
            .Select(p => new ProductResponse(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.IsActive))
            .ToListAsync(cancellationToken);

        Console.WriteLine($"Try to GetAllAsync Products: {JsonConvert.SerializeObject(products)}");

        return products;
    }
}