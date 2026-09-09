using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using StockFlow.Api.Tests;
using StockFlow.Application.Products;

namespace StockFlow.Api.Test.Inventory;

public class InventoryApiTests : IClassFixture<CustomWebApplicationFactory>
{
    // private HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public InventoryApiTests(
        CustomWebApplicationFactory factory)
    {
        // _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task StockOut_WhenInventoryIsAvailable_ShouldReturnNoContent()
    {
        // await _factory.ResetDatabaseAsync();
        var _client = await TestAuthenticationHelper.CreateAuthenticatedClientAsync(_factory);

        // 1. 建立 Product
        var createRequest = new
        {
            name = $"Integration Test {Guid.NewGuid()}",
            description = "Created by integration test inventory test",
            price = 99.99m,
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Products",
            createRequest);

        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode);

        if (createResponse.StatusCode == HttpStatusCode.Created)
        {
            var allProducts = await _client.GetFromJsonAsync<IReadOnlyList<ProductResponse>>("/api/Products/GetAllAsync");

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

            var problem =
                await thirdStockOutResponse.Content
                    .ReadFromJsonAsync<ProblemDetailsResponse>();

            // Console.WriteLine($"problem : {JsonConvert.SerializeObject(problem)}");
            Assert.NotNull(problem);
            Assert.Equal("Insufficient Inventory", problem.Title);
            Assert.Equal("INSUFFICIENT_INVENTORY", problem.Code);
            Assert.False(string.IsNullOrEmpty(problem.TraceId));
        }
        else
        {
            Console.WriteLine($"CreateProduct_ShouldReturnCreated Success 2");
        }
    }

    private sealed record ProblemDetailsResponse(
        string? Title,
        string? Detail,
        string? Code,
        string? TraceId);
}

// Status = StatusCodes.Status409Conflict,
//                 Title = "Insufficient Inventory",
//                 Detail = "The inventory is insufficient for the requested operation.",
//                 Extensions =
//                     {
//                         ["code"] = "INSUFFICIENT_INVENTORY",
//                         ["traceId"] = httpContext.TraceIdentifier,
//                     }