using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using GestorAdmi.Tests.Integration.Infrastructure;

namespace GestorAdmi.Tests.Integration.Tests;

public class HardwareIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HardwareIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> GetTokenAsync()
    {
        var response = await _client.PostAsJsonAsync("/login",
            new { Email = "admin@test.com", Password = "password123" });
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return body!.Data!.Token;
    }

    [Fact]
    public async Task LeerTodo_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync("/api/Hardware/LeerTodo");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LeerTodo_WithAuth_Returns200WithItems()
    {
        // Arrange
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/Hardware/LeerTodo?cantidad=10&pagina=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<PaginatedResponse>();
        body.Should().NotBeNull();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
        body.Data!.TotalCount.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task LeerTodo_SearchByMarca_FiltersResults()
    {
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/Hardware/LeerTodo?busqueda=Dell&pagina=1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PaginatedResponse>();
        body!.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Crear_WithAuth_CreatesHardwareAndReturnsId()
    {
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var payload = new
        {
            Ubicacion = "Bodega",
            Descripcion = "Teclado mecánico",
            NombreDispositivo = "Teclado",
            Marca = "Logitech",
            Modelo = "K100",
            CodigoCne = "CNE-999",
            IdEquipo = "EQ-999",
            Estado = "Activo",
            Ram = "",
            Rom = "",
            Procesador = "",
            Valor = 80.0
        };

        var response = await _client.PostAsJsonAsync("/api/Hardware/Crear", payload);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<LongResponse>();
        body!.Data.Should().BeGreaterThan(0);
    }

    // Helpers de deserialización
    private record LoginResponse(bool Success, LoginData? Data);
    private record LoginData(string Token);
    private record PaginatedResponse(bool Success, HardwarePage? Data);
    private record HardwarePage(int TotalCount, List<object> Items);
    private record LongResponse(bool Success, long Data);
}
