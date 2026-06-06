using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

/// <summary>
/// Extensión para registrar servicios de Infrastructure
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment? environment = null)
    {
        // Registrar DbContext - InMemory para Testing, SqlServer para el resto
        if (environment?.IsEnvironment("Testing") == true)
        {
            var dbName = configuration["InMemoryDatabaseName"] ?? $"GestorAdmiTestDb_{Guid.NewGuid()}";
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(dbName));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection")!,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorCodesToAdd: null);
                    });
            });
        }

        // Registrar repositorios
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        // Facturación electrónica — repositorios
        services.AddScoped<IEmisorRepository, EmisorRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IFacturaRepository, FacturaRepository>();
        services.AddScoped<IConfiguracionSRIRepository, ConfiguracionSRIRepository>();
        services.AddScoped<INotaCreditoRepository, NotaCreditoRepository>();
        services.AddScoped<INotaDebitoRepository, NotaDebitoRepository>();
        services.AddScoped<IRetencionRepository, RetencionRepository>();
        services.AddScoped<IReporteFacturacionRepository, ReporteFacturacionRepository>();

        // Facturación electrónica — servicios
        services.AddScoped<IRideService, RideService>();
        services.AddScoped<IAtsExcelService, AtsExcelService>();
        services.AddScoped<IXmlFacturaService, XmlFacturaService>();
        services.AddScoped<IXmlNotaCreditoService, XmlNotaCreditoService>();
        services.AddScoped<IXmlNotaDebitoService, XmlNotaDebitoService>();
        services.AddScoped<IXmlRetencionService, XmlRetencionService>();
        services.AddScoped<IFirmaDigitalService, FirmaDigitalService>();
        services.AddScoped<ISRIService, SRIService>();
        services.AddScoped<Application.Services.FacturaNumeracionService>();

        // Registrar servicios
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
