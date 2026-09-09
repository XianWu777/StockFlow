using System.Net.Http.Json;
using StockFlow.Api.Tests;

public static class TestAuthenticationHelper
{
    public static async Task<HttpClient> CreateAuthenticatedClientAsync(
        CustomWebApplicationFactory factory)
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                username = "admin",
                password = "admin"
            });

        response.EnsureSuccessStatusCode();

        var login =
            await response.Content
                .ReadFromJsonAsync<LoginResponse>();

        ArgumentNullException.ThrowIfNull(login);

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken
            );

        return client;
    }

    private sealed record LoginResponse(
        string AccessToken
    );
}