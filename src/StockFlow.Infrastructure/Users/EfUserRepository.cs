using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Authentication;
using StockFlow.Infrastructure.Data;

namespace StockFlow.Infrastructure.Users;

public sealed class EfUserRepository : IUserRepository
{
    private readonly StockFlowDbContext _dbContext;

    public EfUserRepository(StockFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsAsync(string username, CancellationToken cancellationToken)
    {
        return _dbContext.Users.AsNoTracking().AnyAsync(user => user.UserName == username, cancellationToken);
    }

    public async Task<UserInfo?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        var userInfo = await _dbContext.Users
            .AsNoTracking()
            .Where(user => user.UserName == username)
            .Select(user => new UserInfo(
                user.Id,
                user.UserName,
                user.PasswordHash,
                user.Role
            ))
            .SingleOrDefaultAsync(cancellationToken);

        return userInfo;
    }

    public Task AddAsync(UserDraft user, CancellationToken cancellationToken)
    {
        var userEntity = new User
        {
            UserName = user.UserName,
            PasswordHash = user.PasswordHash,
            Role = user.Role,
        };

        _dbContext.Users.Add(userEntity);

        return Task.CompletedTask;
    }
}