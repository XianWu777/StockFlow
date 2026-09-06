using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using StockFlow.Application.Products;

namespace StockFlow.Api.Test.Inventory;

public class InventoryApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public InventoryApiTests(
        WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task StockOut_WhenInventoryIsAvailable_ShouldReturnNoContent()
    {
        // 1. 建立 Product
        var createRequest = new
        {
            name = $"Integration Test {Guid.NewGuid()}",
            description = "Created by integration test",
            price = 99.99m,
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Products",
            createRequest);

        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode);

        var product = await createResponse.Content
            .ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(product);

        Console.WriteLine($"Created product {JsonConvert.SerializeObject(product)}");

        // 2. 建立 StockIn 10
        var stockInRequest = new { quantity = 10 };

        var stockInResponse = await _client.PostAsJsonAsync(
            $"/api/inventory/{product!.id}/stock-in",
            stockInRequest);

        Assert.Equal(
            HttpStatusCode.OK,
            stockInResponse.StatusCode);

        // 3. 建立 StockOut 5
        var firstStockOutRequest = new { quantity = 5 };

        var firstStockOutResponse = await _client.PostAsJsonAsync(
            $"/api/inventory/{product!.id}/stock-out",
            firstStockOutRequest);

        Assert.Equal(
            HttpStatusCode.OK,
            firstStockOutResponse.StatusCode);

        // 4. 建立 StockOut 5
        var secondStockOutRequest = new { quantity = 5 };

        var secondStockOutResponse = await _client.PostAsJsonAsync(
            $"/api/inventory/{product!.id}/stock-out",
            secondStockOutRequest);

        Assert.Equal(
            HttpStatusCode.OK,
            secondStockOutResponse.StatusCode);

        // 5. 建立 StockOut 1
        var thirdStockOutRequest = new { quantity = 1 };

        var thirdStockOutResponse = await _client.PostAsJsonAsync(
            $"/api/inventory/{product!.id}/stock-out",
            thirdStockOutRequest);

        Assert.Equal(
            HttpStatusCode.Conflict,
            thirdStockOutResponse.StatusCode);
    }
}