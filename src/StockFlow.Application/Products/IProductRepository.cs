using StockFlow.Application.Common;

namespace StockFlow.Application.Products
{
    public interface IProductRepository
    {
        Task<PagedResult<ProductResponse>> GetAllAsync(
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
}