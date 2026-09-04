public interface IInventoryMovementRepository
{
    Task<IReadOnlyList<InventoryMovementResponse>> GetAllAsync(
        CancellationToken cancellationToken);
}