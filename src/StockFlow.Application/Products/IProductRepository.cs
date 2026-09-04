using StockFlow.Application.Common;

namespace StockFlow.Application.Products;

public interface IProductRepository
{
    Task<IReadOnlyList<ProductResponse>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<PagedResult<ProductResponse>> GetAllQueryAsync(
        GetProductsQuery query,
        CancellationToken cancellationToken);

    Task<ProductResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken);

    Task<ProductResponse?> UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);
}