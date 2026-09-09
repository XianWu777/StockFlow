
namespace StockFlow.Application.Inventory;

public interface IInventoryRepository
{
    Task<IReadOnlyList<InventoryResponse>> GetAllAsync(
        CancellationToken cancellationToken);

    Task StockInAsync(
        Guid productId,
        StockInRequest request,
        CancellationToken cancellationToken);

    Task<int> StockOutAsync(
        Guid productId,
        StockOutRequest request,
        CancellationToken cancellationToken);
}