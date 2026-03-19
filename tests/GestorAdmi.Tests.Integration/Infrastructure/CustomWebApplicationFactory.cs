using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestorAdmi.Tests.Integration.Infrastructure;

/// <summary>
/// Factory que reemplaza SqlServer por un DbContext InMemory para pruebas de integración.
/// Cada instancia usa una base de datos InMemory con nombre único para evitar colisiones
/// cuando varias clases de test corren en paralelo.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    // Nombre único por instancia de factory para evitar conflictos de DbContext en tests paralelos
    private readonly string _dbName = $"GestorAdmiTestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // Cargar configuración de prueba (sobreescribe appsettings.json del API)
        builder.ConfigureAppConfiguration(config =>
        {
            // Archivo con valores de test (copiado al directorio de salida del proyecto de prueba)
            config.AddJsonFile(
                Path.Combine(AppContext.BaseDirectory, "appsettings.Testing.json"),
                optional: true,
                reloadOnChange: false);

            // Garantía adicional vía InMemory (tiene la mayor precedencia)
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"]             = "test-secret-key-for-integration-tests-32chars!!",
                ["Jwt:Issuer"]          = "GestorAdmi",
                ["Jwt:Audience"]        = "GestorAdmiClient",
                ["Jwt:ExpirationHours"] = "1",
                ["ConnectionStrings:DefaultConnection"] = "InMemory"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Quitar el registro real de ApplicationDbContext (SqlServer)
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Registrar DbContext con base de datos en memoria (nombre único por instancia)
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            // Crear y sembrar la base de datos en memoria
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
            DatabaseSeeder.Seed(db);
        });
    }
}
