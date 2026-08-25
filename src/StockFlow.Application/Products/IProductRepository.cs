namespace StockFlow.Application.Products
{
    public interface IProductRepository
    {
        Task<IReadOnlyList<ProductResponse>> GetAllAsync(
            CancellationToken cancellationToken);
    }
}