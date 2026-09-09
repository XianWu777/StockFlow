using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using StockFlow.Api.Tests;
using StockFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Inventory;

public sealed class InventoryConsistencyTests :
    IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public InventoryConsistencyTests(
        CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Inventory_ShouldMatchSuccessfulMovements()
    {
        await _factory.ResetDatabaseAsync();

        var authenticatedClient = await TestAuthenticationHelper.CreateAuthenticatedClientAsync(_factory);

        var productId = await CreateProductAsync(authenticatedClient);

        // Stock In 100
        await StockInAsync(authenticatedClient, productId, 100);

        // Stock Out 30
        await StockOutAsync(authenticatedClient, productId, 30);

        // Stock In 20
        await StockInAsync(authenticatedClient, productId, 20);

        // Stock Out 40
        await StockOutAsync(authenticatedClient, productId, 40);

        // Expected
        // 100 - 30 + 20 - 40 = 50
        await using var scope = _factory.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<StockFlowDbContext>();

        var inventory = await dbContext.Inventories
            .SingleAsync(x => x.ProductId == productId);

        var movements = await dbContext.InventoryMovements
            .Where(x => x.ProductId == productId)
            .ToListAsync();

        var stockInTotal = movements.Where(x => x.Type == InventoryMovementType.StockIn).Sum(x => x.Quantity);

        var stockOutTotal = movements.Where(x => x.Type == InventoryMovementType.StockOut).Sum(x => x.Quantity);

        var calculatedQuantity = stockInTotal - stockOutTotal;

        Assert.Equal(50, inventory.Quantity);

        Assert.Equal(calculatedQuantity, inventory.Quantity);

        Assert.Equal(120, stockInTotal);
        Assert.Equal(70, stockOutTotal);

        Assert.Equal(4, movements.Count);
    }

    private static async Task<Guid> CreateProductAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/Products",
            new
            {
                name = $"Consistency Test Product",
                description = "Integration test",
                price = 100
            }
        );

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var product = await response.Content
            .ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(product);

        return product!.id;
    }

    private static async Task StockInAsync(
        HttpClient client,
        Guid productId,
        int quantity)
    {
        var response = await client.PostAsJsonAsync(
            $"/api/inventory/{productId}/stock-in",
            new { quantity = quantity });

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    private static async Task StockOutAsync(
        HttpClient client,
        Guid productId,
        int quantity)
    {
        var response = await client.PostAsJsonAsync(
            $"/api/inventory/{productId}/stock-out",
            new { quantity = quantity });

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    private sealed record ProductResponse(
        Guid id
    );
}