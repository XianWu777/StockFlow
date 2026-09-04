namespace StockFlow.Application.Inventory.Exceptions;

public sealed class InsufficientInventoryException : Exception
{
    public InsufficientInventoryException()
        : base("Insufficient inventory.")
    {
    }
}