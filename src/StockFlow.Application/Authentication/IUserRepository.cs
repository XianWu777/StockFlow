namespace StockFlow.Application.Authentication;

public interface IUserRepository
{
    Task<bool> ExistsAsync(
        string username,
        CancellationToken cancellationToken
    );

    Task<UserInfo?> GetByUsernameAsync(
        string username,
        CancellationToken cancellationToken
    );

    Task AddAsync(
        UserDraft user,
        CancellationToken cancellationToken
    );
}