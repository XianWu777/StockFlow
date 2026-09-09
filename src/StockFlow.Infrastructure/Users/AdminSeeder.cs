using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockFlow.Infrastructure.Data;
using StockFlow.Infrastructure.Users;

public sealed class AdminSeeder
{
    public static async Task SeedAsync(
        StockFlowDbContext dbContext,
        StockFlow.Application.Authentication.IPasswordHasher passwordHasher,
        CancellationToken cancellationToken = default)
    {
        const string username = "admin";
        const string password = "admin";
        const string role = "Admin";

        var exists = await dbContext.Users
            .AnyAsync(
                x => x.UserName == username,
                cancellationToken);

        if (exists)
        {
            Console.WriteLine("Admin user already exists");
            return;
        }
        else
        {
            Console.WriteLine("Admin user does not exist");
        }

        var user = new User
        {
            UserName = username,
            PasswordHash = passwordHasher.HasPassword(password),
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}