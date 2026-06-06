using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GestorAdmi.Tests.Integration.Infrastructure;

namespace GestorAdmi.Tests.Integration.Tests;

[Collection("IntegrationTests")]
public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithToken()
    {
        // Arrange
        var payload = new { Email = "admin@test.com", Password = "password123" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/login", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        body.Should().NotBeNull();
        body!.Data.Should().NotBeNull();
        body.Data!.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_InvalidPassword_Returns401()
    {
        // Arrange
        var payload = new { Email = "admin@test.com", Password = "wrongpassword" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/login", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_UnknownEmail_Returns401()
    {
        var payload = new { Email = "noexiste@test.com", Password = "wrongpassword" };

        var response = await _client.PostAsJsonAsync("/api/v1/login", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // Clases auxiliares para deserializar la respuesta
    private record LoginResponse(bool Success, LoginData? Data);
    private record LoginData(string Token, int Id, string Email);
}
