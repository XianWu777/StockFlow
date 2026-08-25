using Newtonsoft.Json;

namespace StockFlow.Application.Products;

public sealed class ProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        IReadOnlyList<ProductResponse> products = await _productRepository.GetAllAsync(cancellationToken);
        return products;
    }
}
