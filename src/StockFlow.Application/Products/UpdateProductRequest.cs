public sealed record UpdateProductRequest(
    string Name,
    string Description,
    decimal Price,
    bool IsActive);