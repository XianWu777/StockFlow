using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StockFlow.Api.Tests;
using StockFlow.Application.Inventory;
using StockFlow.Application.Products;
using StockFlow.Infrastructure.Data;

namespace StockFlow.Api.Tests.Inventory;

public class InventoryConcurrencyTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public InventoryConcurrencyTests(
        CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task StockOut_ConcurrentRequests_ShouldNotOversell()
    {
        // await _factory.ResetDatabaseAsync();
        var authenticatedClient = await TestAuthenticationHelper.CreateAuthenticatedClientAsync(_factory);

        var createProductResponse = await authenticatedClient.PostAsJsonAsync(
            "/api/Products",
            new
            {
                name = "Concurrency Test Product",
                description = "Test",
                price = 100
            });

        var product = await createProductResponse.Content
            .ReadFromJsonAsync<ProductResponse>();

        Console.WriteLine($"product : {product} , createProductResponse status : {createProductResponse.StatusCode}");

        Assert.Equal(
            HttpStatusCode.Created,
            createProductResponse.StatusCode);

        Assert.NotNull(product);

        var productId = product!.id;

        Console.WriteLine($"productId : {productId}");

        var stockInRequest = await authenticatedClient.PostAsJsonAsync(
            "/api/inventory/" + productId + "/stock-in",
            new { quantity = 50 });

        Console.WriteLine($"stockInRequest : {stockInRequest.StatusCode}");

        Assert.Equal(
            HttpStatusCode.OK,
            stockInRequest.StatusCode);

        int temp = 0;
        using var requestClient = await TestAuthenticationHelper.CreateAuthenticatedClientAsync(_factory);

        var tasks = Enumerable
            .Range(0, 10)
            .Select(async _ =>
            {
                var response = await requestClient.PostAsJsonAsync(
                    $"/api/inventory/{productId}/stock-out",
                    new { quantity = 1 });

                return response;
            });

        var responses = await Task.WhenAll(tasks);
        Console.WriteLine($"temp : {temp}");

        var statusCounts =
            responses
                .GroupBy(x => x.StatusCode)
                .ToDictionary(
                    x => x.Key,
                    x => x.Count());

        foreach (var response in responses)
        {
            if ((int)response.StatusCode >= 500)
            {
                var body =
                    await response.Content.ReadAsStringAsync();

                Console.WriteLine(
                    $"Status: {(int)response.StatusCode}");

                Console.WriteLine($"Body: {body}");
            }
        }

        var successCount = responses.Count(x => x.StatusCode == HttpStatusCode.OK);
        var insufficientStorageCount = responses.Count(x => x.StatusCode == HttpStatusCode.Conflict);

        Console.WriteLine($"successCount : {successCount} , insufficientStorageCount : {insufficientStorageCount}");

        // Assert.Equal(50, successCount);
        // Assert.Equal(450, insufficientStorageCount);

        Assert.All(
            responses,
            response =>
                Assert.Contains(
                    response.StatusCode,
                    new[]
                    {
                        HttpStatusCode.OK,
                        HttpStatusCode.Conflict
                    }
                ));
    }

    [Fact]
    public async Task ConcurrentStockOut_ShouldKeepInventoryConsistent()
    {
        await _factory.ResetDatabaseAsync();

        var authenticatedClient = await TestAuthenticationHelper.CreateAuthenticatedClientAsync(_factory);

        var productId = await CreateProductAsync(authenticatedClient);

        // Initial inventory = 50
        var stockInResponse = await authenticatedClient.PostAsJsonAsync(
            $"/api/inventory/{productId}/stock-in",
            new { quantity = 50 });

        Assert.Equal(
            HttpStatusCode.OK,
            stockInResponse.StatusCode);

        const int requestCount = 100;
        const int stockOutQuantity = 1;

        var tasks = Enumerable.Range(0, requestCount)
            .Select(_ =>
                authenticatedClient.PostAsJsonAsync(
                    $"/api/inventory/{productId}/stock-out",
                    new { quantity = stockOutQuantity }));

        var responses = await Task.WhenAll(tasks);

        var successCount = responses.Count(x => x.StatusCode == HttpStatusCode.OK);

        var conflictCount = responses.Count(x => x.StatusCode == HttpStatusCode.Conflict);

        var serverErrorCount = responses.Count(x => x.StatusCode >= HttpStatusCode.InternalServerError);

        Assert.Equal(50, successCount);
        Assert.Equal(50, conflictCount);
        Assert.Equal(0, serverErrorCount);

        // Verify database state.
        await using var scope = _factory.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<StockFlowDbContext>();

        var inventory = await dbContext.Inventories.SingleAsync(x => x.ProductId == productId);

        var movements = await dbContext.InventoryMovements
            .Where(x => x.ProductId == productId)
            .ToListAsync();

        Console.WriteLine($"movements : {JsonConvert.SerializeObject(movements)}");

        var stockInTotal = movements.Where(x => x.Type == InventoryMovementType.StockIn).Sum(x => x.Quantity);

        var stockOutTotal = movements.Where(x => x.Type == InventoryMovementType.StockOut).Sum(x => x.Quantity);

        // Inventory = StockIn - StockOut
        Assert.Equal(stockInTotal - stockOutTotal, inventory.Quantity);

        Assert.Equal(0, inventory.Quantity);

        Assert.Equal(50, stockInTotal);

        Assert.Equal(50, stockOutTotal);

        Assert.Equal(51, movements.Count);
    }

    private static async Task<Guid> CreateProductAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/Products",
            new
            {
                name = $"Concurrency Test Product",
                description = "Integration test",
                price = 100
            });

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var product = await response.Content
            .ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(product);

        return product!.id;
    }

    private sealed record ProductResponse(
        Guid id,
        string Name,
        string Description,
        decimal Price);
}