using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockFlow.Infrastructure.Data;

namespace StockFlow.Api.Tests;

public sealed class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            string baseDir = AppContext.BaseDirectory;
            string jsonPath = Path.GetFullPath(Path.Combine(baseDir, "../../../../appsettings.Test.json"));

            config.AddJsonFile(
                jsonPath,
                optional: false
            );
        });

        builder.ConfigureServices(services =>
        {
            using var scope = services.BuildServiceProvider().CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<StockFlowDbContext>();

            dbContext.Database.Migrate();
        });
    }

    public async Task ResetDatabaseAsync()
    {
        Console.WriteLine("ResetDatabaseAsync");
        using var scope = this.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<StockFlowDbContext>();

        await dbContext.Database.ExecuteSqlRawAsync("""
            TRUNCATE TABLE
                "InventoryMovements",
                "Inventories",
                "Products"
            RESTART IDENTITY CASCADE;
            """);
    }
}