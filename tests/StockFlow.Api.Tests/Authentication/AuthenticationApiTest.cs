using System.Net;
using System.Net.Http.Json;
using StockFlow.Api.Tests;
using StockFlow.Application.Authentication;

namespace StockFlow.Api.Tests.Authentication;

public class AuthenticationApiTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthenticationApiTest(
        CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateProduct_WithoutToken_ShouldReturn401()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/Products",
            new
            {
                name = "Test Product",
                description = "Test",
                price = 100
            });

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithUserToken_ShouldReturn403()
    {
        // await _factory.ResetDatabaseAsync();

        var username = "admin";
        var password = "admin";

        var accessToken = await LoginAsync(username, password);

        _client.DefaultRequestHeaders.Authorization = new("Bearer", accessToken);

        var response = await _client.PostAsJsonAsync(
            "/api/Products",
            new
            {
                name = "Test Product",
                description = "Test",
                price = 100
            });

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_AsAdmin_ShouldReturn201()
    {
        // await _factory.ResetDatabaseAsync();

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                username = "admin",
                password = "admin"
            });

        Assert.Equal(
            HttpStatusCode.OK,
            loginResponse.StatusCode);

        var login =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(login);

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken);

        var response = await _client.PostAsJsonAsync(
            "/api/Products",
            new
            {
                name = "Admin Product",
                description = "Created by admin",
                price = 100
            });

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
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