using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using GestorAdmi.Tests.Integration.Infrastructure;

namespace GestorAdmi.Tests.Integration.Tests;

[Collection("IntegrationTests")]
public class ClientesIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ClientesIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> GetTokenAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/login",
            new { Email = "admin@test.com", Password = "password123" });
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return body!.Data!.Token!;
    }

    [Fact]
    public async Task LeerTodo_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync("/api/v1/clientes/LeerTodo");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LeerTodo_WithAuth_Returns200WithEmptyPage()
    {
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/v1/clientes/LeerTodo?cantidad=10&pagina=1&busqueda=no-match-12345");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<PaginatedResponse>();
        body.Should().NotBeNull();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
        body.Data!.TotalCount.Should().Be(0);
        body.Data.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Crear_WithAuth_CreatesClienteAndReturnsId()
    {
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var payload = new
        {
            TipoIdentificacion = 3,
            NumeroIdentificacion = "9999999999999",
            RazonSocial = "Cliente de Prueba",
            Email = "cliente@test.com",
            Telefono = "0999999999",
            Direccion = "Av. Principal 123"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/clientes/Crear", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<LongResponse>();
        body.Should().NotBeNull();
        body!.Success.Should().BeTrue();
        body.Data.Should().BeGreaterThan(0);
    }

    private record LoginResponse(bool Success, LoginData? Data);
    private record LoginData(string Token);
    private record PaginatedResponse(bool Success, ClientePage? Data);
    private record ClientePage(int TotalCount, List<object> Items);
    private record LongResponse(bool Success, long Data);
}
