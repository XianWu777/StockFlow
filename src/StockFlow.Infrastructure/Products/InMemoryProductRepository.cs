using StockFlow.Application.Products;

namespace StockFlow.Infrastructure.Products;

public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly List<ProductResponse> _products = [
        new (
            Guid.NewGuid(),
            "Mechanical Keyboard",
            "A mechanical keyboard",
            2990m,
            true
        ),
        new (
            Guid.NewGuid(),
            "Gaming Mouse",
            "A gaming mouse",
            1590m,
            true
        )
    ];

    public Task<IReadOnlyList<ProductResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        IReadOnlyList<ProductResponse> p = new List<ProductResponse> {
            new (
                Guid.NewGuid(),
                "Mechanical Keyboard",
                "A mechanical keyboard",
                2990m,
                true
            ),
            new (
                Guid.NewGuid(),
                "Gaming Mouse",
                "A gaming mouse",
                1590m,
                true
            )
        };
        // IReadOnlyList<ProductResponse> products = _products;
        return Task.FromResult(p);
    }

    public Task<ProductResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var p = new ProductResponse(
            Guid.NewGuid(),
            "Mechanical Keyboard",
            "A mechanical keyboard",
            2990m,
            true
        );

        return Task.FromResult(p);
    }

    public Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        ProductResponse product = new(
            Guid.NewGuid(),
            request.Name,
            request.Description,
            request.Price,
            true
        );

        return Task.FromResult(product);
    }

    public Task<ProductResponse?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var p = new ProductResponse(
            Guid.NewGuid(),
            request.Name,
            request.Description,
            request.Price,
            true
        );

        return Task.FromResult(p);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }
}