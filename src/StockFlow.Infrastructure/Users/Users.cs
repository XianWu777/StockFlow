namespace StockFlow.Infrastructure.Users;

public sealed class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}