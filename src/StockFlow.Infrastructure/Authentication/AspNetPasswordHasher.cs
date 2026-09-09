using StockFlow.Application.Authentication;
using Microsoft.AspNetCore.Identity;
using StockFlow.Infrastructure.Users;

namespace StockFlow.Infrastructure.Authentication;

public sealed class AspNetPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string HasPassword(string password)
    {
        return _hasher.HashPassword(
            new User(),
            password);
    }

    public bool Verify(string password, string hash)
    {
        var result =
            _hasher.VerifyHashedPassword(
                new User(),
                hash,
                password);

        return result != PasswordVerificationResult.Failed;
    }
}