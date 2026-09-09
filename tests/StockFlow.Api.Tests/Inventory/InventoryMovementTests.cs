using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using StockFlow.Api.Tests;

namespace StockFlow.Application.Tests.Inventory;

public sealed class InventoryMovementTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public InventoryMovementTests(
        CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetMovements_ShouldReturnStockInAndStockOut()
    {
        await _factory.ResetDatabaseAsync();

        var authenticatedClient = await TestAuthenticationHelper.CreateAuthenticatedClientAsync(_factory);

        // Arrange
        var productId = await CreateProductAsync(authenticatedClient);

        var allProducts = await authenticatedClient.GetFromJsonAsync<IReadOnlyList<ProductResponse>>("/api/Products/GetAllAsync");

        Console.WriteLine($"allProducts : {JsonConvert.SerializeObject(allProducts)}");

        var allInventoryMovements = await authenticatedClient.GetFromJsonAsync<IReadOnlyList<InventoryMovementResponse>>("/api/inventory");

        Console.WriteLine($"allInventoryMovements : {JsonConvert.SerializeObject(allInventoryMovements)}");

        // Stock In 50
        var stockInResponse = await authenticatedClient.PostAsJsonAsync(
            $"/api/inventory/{productId}/stock-in",
            new { quantity = 50 });

        Assert.Equal(
            HttpStatusCode.OK,
            stockInResponse.StatusCode);

        var allInventoryMovements2 = await authenticatedClient.GetFromJsonAsync<IReadOnlyList<InventoryMovementResponse>>("/api/inventory");

        Console.WriteLine($"allInventoryMovements2 : {JsonConvert.SerializeObject(allInventoryMovements2)}");

        // Stock Out 20
        var stockOutResponse = await authenticatedClient.PostAsJsonAsync(
            $"/api/inventory/{productId}/stock-out",
            new { quantity = 20 });

        Assert.Equal(
            HttpStatusCode.OK,
            stockOutResponse.StatusCode);

        // Act
        var response = await authenticatedClient.GetAsync(
            $"/api/inventory/{productId}/movements");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var resultJson = await response.Content.ReadAsStringAsync();
        // .ReadFromJsonAsync<PagedResult>();

        var result = JsonConvert.DeserializeObject<PagedResult<InventoryMovementResponse>>(resultJson);

        Console.WriteLine($"result : {JsonConvert.SerializeObject(result)}");

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal(2, result.Items.Count);

        // Assert.Equal(InventoryMovementType.StockOut, result.Items[0].Type);
        // Assert.Equal(InventoryMovementType.StockIn, result.Items[1].Type);

        // Assert.Equal(50, result.Items[0].Quantity);
        // Assert.Equal(20, result.Items[1].Quantity);
    }

    [Fact]
    public async Task GetMovements_ShouldSupportPagination()
    {
        await _factory.ResetDatabaseAsync();

        var client = await TestAuthenticationHelper.CreateAuthenticatedClientAsync(_factory);

        var productId = await CreateProductAsync(client);

        for (int i = 0; i < 5; i++)
        {
            var stockInResponse = await client.PostAsJsonAsync(
                $"/api/inventory/{productId}/stock-in",
                new { quantity = 10 });

            Assert.Equal(
                HttpStatusCode.OK,
                stockInResponse.StatusCode);
        }

        for (int i = 0; i < 5; i++)
        {
            var stockOutResponse = await client.PostAsJsonAsync(
                $"/api/inventory/{productId}/stock-out",
                new { quantity = 1 });

            Assert.Equal(
                HttpStatusCode.OK,
                stockOutResponse.StatusCode);
        }

        // Act
        var response = await client.GetAsync(
            $"/api/inventory/{productId}/movements" +
            "?page=1&pageSize=3");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var resultJson = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<PagedResult<InventoryMovementResponse>>(resultJson);

        Assert.NotNull(result);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(4, result.TotalPages);
    }

    [Fact]
    public async Task GetMovements_ShouldFilterByType()
    {
        await _factory.ResetDatabaseAsync();

        var client = await TestAuthenticationHelper.CreateAuthenticatedClientAsync(_factory);

        var productId = await CreateProductAsync(client);

        // Stock In
        await client.PostAsJsonAsync(
            $"/api/inventory/{productId}/stock-in",
            new { quantity = 50 });

        // Stock Out
        await client.PostAsJsonAsync(
            $"/api/inventory/{productId}/stock-out",
            new { quantity = 10 });

        // Act
        var response = await client.GetAsync(
            $"/api/inventory/{productId}/movements" +
            "?page=1&pageSize=3&type=stockOut");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var resultJson = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<PagedResult<InventoryMovementResponse>>(resultJson);

        Assert.NotNull(result);

        Assert.Equal(1, result.TotalPages);
        Assert.Single(result.Items);
        Assert.Equal(InventoryMovementType.StockOut, result.Items[0].Type);
        Assert.Equal(10, result.Items[0].Quantity);
    }

    private async Task<Guid> CreateProductAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/Products",
            new
            {
                name = $"Integration Test {Guid.NewGuid()}",
                description = "Created by integration test inventory test",
                price = 100,
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

    private sealed record InventoryMovementResponse(
        Guid Id,
        Guid ProductId,
        InventoryMovementType Type,
        int Quantity,
        DateTime CreatedAt);

    private sealed record PagedResult<T>(
        IReadOnlyList<T> Items,
        int Page,
        int PageSize,
        int TotalCount)
    {
        public int TotalPages
        {
            get
            {
                Console.WriteLine($"TotalPages {Page} , {PageSize}");
                return (int)Math.Ceiling((double)TotalCount / PageSize);
            }
        }
    }
    private enum InventoryMovementType
    {
        StockIn,
        StockOut,
    }
}