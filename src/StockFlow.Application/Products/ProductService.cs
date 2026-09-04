using Newtonsoft.Json;
using StockFlow.Application.Common;

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
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return products;
    }

    public async Task<PagedResult<ProductResponse>> GetAllQueryAsync(
        GetProductsQuery query,
        CancellationToken cancellationToken)
    {
        PagedResult<ProductResponse> products = await _productRepository.GetAllQueryAsync(query, cancellationToken);
        return products;
    }

    public async Task<ProductResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        ProductResponse? product = await _productRepository.GetByIdAsync(id, cancellationToken);
        return product;
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        ProductResponse product = await _productRepository.CreateAsync(request, cancellationToken);
        return product;
    }

    public async Task<ProductResponse?> UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        ProductResponse product = await _productRepository.UpdateAsync(id, request, cancellationToken);
        return product;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        bool deleted = await _productRepository.DeleteAsync(id, cancellationToken);
        return deleted;
    }
}
