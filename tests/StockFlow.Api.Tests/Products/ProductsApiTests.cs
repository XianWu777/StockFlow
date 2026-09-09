using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using StockFlow.Application.Authentication;
using StockFlow.Application.Products;
using Xunit;

namespace StockFlow.Api.Tests.Products;

public class ProductsApiTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductsApiTests(
        WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ShouldReturnSuccess()
    {
        // Act
        var response =
            await _client.GetAsync("/api/Products/GetAllAsync");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnCreated()
    {
        // Arrange
        var request = new
        {
            name = $"Integration Test {Guid.NewGuid()}",
            description = "Created by integration test product test",
            price = 99.99m,
        };

        using var content = JsonContent.Create(request);

        // Act
        var response = await _client.PostAsync(
            "/api/Products",
            content);

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_ThenGetProduct_ShouldReturnProduct()
    {
        var token = await LoginAsync("admin", "admin");

        // Arrange
        var productName =
            $"Integration Test {Guid.NewGuid()}";

        var request = new
        {
            name = productName,
            description = "Created by integration test product test",
            price = 99.99m,
        };

        using var content = JsonContent.Create(request);

        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        // Act
        var createResponse = await _client.PostAsync(
            "/api/Products",
            content);

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode);

        var createdProduct =
            await createResponse.Content
                .ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(createdProduct);

        var getResponse = await _client.GetAsync(
            $"/api/products/{createdProduct!.id}");

        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode);

        var product =
            await getResponse.Content
                .ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(product);
        Assert.Equal(productName, product!.Name);
    }

    private async Task<string> LoginAsync(string username, string password)
    {
        var loginRequest = new
        {
            username = username,
            password = password
        };

        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            loginRequest);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content
            .ReadFromJsonAsync<LoginResponse>();

        return responseContent!.AccessToken;
    }
}