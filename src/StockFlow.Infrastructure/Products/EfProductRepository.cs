using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StockFlow.Application.Common;
using StockFlow.Application.Products;
using StockFlow.Infrastructure.Data;
using StockFlow.Infrastructure.Entity;

namespace StockFlow.Infrastructure.Products;

public class EfProductRepository : IProductRepository
{
    private readonly StockFlowDbContext _dbContext;

    public EfProductRepository(StockFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<ProductResponse>> GetAllAsync(GetProductsQuery query, CancellationToken cancellationToken)
    {
        Console.WriteLine("Try to GetAllAsync");

        var products = _dbContext.Products
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            products = products.Where(product =>
                product.Name.Contains(query.Search));
        }

        var tatalCount = await products.CountAsync(cancellationToken);
        string sortBy = query.SortBy.ToLowerInvariant();
        string sortOrder = query.SortOrder.ToLowerInvariant();

        switch (sortBy)
        {
            default:
            case "name":
                if (sortOrder == "desc")
                    products = products.OrderByDescending(product => product.Name).ThenBy(product => product.Id);
                else
                    products = products.OrderBy(product => product.Name).ThenBy(product => product.Id);
                break;
            case "description":
                if (sortOrder == "desc")
                    products = products.OrderByDescending(product => product.Description).ThenBy(product => product.Id);
                else
                    products = products.OrderBy(product => product.Description).ThenBy(product => product.Id);
                break;
            case "price":
                if (sortOrder == "desc")
                    products = products.OrderByDescending(product => product.Price).ThenBy(product => product.Id);
                else
                    products = products.OrderBy(product => product.Price).ThenBy(product => product.Id);
                break;
            case "isActive":
                if (sortOrder == "desc")
                    products = products.OrderByDescending(product => product.IsActive).ThenBy(product => product.Id);
                else
                    products = products.OrderBy(product => product.IsActive).ThenBy(product => product.Id);
                break;
            case "createdAt":
                if (sortOrder == "desc")
                    products = products.OrderByDescending(product => product.CreatedAt).ThenBy(product => product.Id);
                else
                    products = products.OrderBy(product => product.CreatedAt).ThenBy(product => product.Id);
                break;
        }

        // order by query
        // sort by query
        var items = await products
            // .OrderBy(product => product.Name)
            // .ThenBy(product => product.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(product => new ProductResponse(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.IsActive))
            .ToListAsync(cancellationToken);

        Console.WriteLine($"Try to GetAllAsync Products: {JsonConvert.SerializeObject(products.ToList())}");

        return new PagedResult<ProductResponse>(
            items,
            query.Page,
            query.PageSize,
            tatalCount);
    }

    public async Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        // Console.WriteLine("Try to GetByIdAsync");

        // var product = await _dbContext.Products
        //     .AsNoTracking()
        //     .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        // Console.WriteLine($"Try to GetByIdAsync Product: {JsonConvert.SerializeObject(product)}");

        // return product == null ? null : new ProductResponse(
        //     product.Id,
        //     product.Name,
        //     product.Description,
        //     product.Price,
        //     product.IsActive);
        var product = await _dbContext.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductResponse(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.IsActive)).FirstOrDefaultAsync(cancellationToken);

        return product;
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        Console.WriteLine("Try to CreateAsync");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        Console.WriteLine($"Try to CreateAsync Product: {JsonConvert.SerializeObject(product)}");

        return new ProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.IsActive);
    }

    public async Task<ProductResponse?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        Console.WriteLine("Try to UpdateAsync");

        var product = await _dbContext.Products
            .FirstOrDefaultAsync
                (product => product.Id == id,
                cancellationToken);

        if (product == null)
        {
            return null;
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.IsActive);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        Console.WriteLine("Try to DeleteAsync");

        var product = await _dbContext.Products
            .FirstOrDefaultAsync
                (product => product.Id == id,
                cancellationToken);

        if (product == null)
        {
            return false;
        }

        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}