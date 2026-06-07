using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db     = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<Application.Interfaces.IPasswordHasher>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            await db.Database.MigrateAsync();
            await SeedUsuariosAsync(db, hasher, logger);
            await SeedEmisoresAsync(db, logger);
            await SeedClientesAsync(db, logger);
            await SeedProductosAsync(db, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error durante el seed de datos");
        }
    }

    /// <summary>
    /// Bootstrap de usuarios admin/vendedor. Corre en cualquier ambiente (incluido Production):
    /// idempotente via AnyAsync, sin esto Production queda sin usuarios para login.
    /// </summary>
    public static async Task SeedUsuariosAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db     = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<Application.Interfaces.IPasswordHasher>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            await SeedUsuariosAsync(db, hasher, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error durante el seed de usuarios");
        }
    }

    private static async Task SeedUsuariosAsync(ApplicationDbContext db, Application.Interfaces.IPasswordHasher hasher, ILogger logger)
    {
        if (await db.Usuarios.AnyAsync()) return;

        var usuarios = new[]
        {
            new Usuario
            {
                Nombre   = "admin",
                Password = hasher.HashPassword("Admin123!"),
                Cargo    = "Administrador",
                Email    = "admin@facturacion.ec",
                Rol      = "Admin"
            },
            new Usuario
            {
                Nombre   = "vendedor1",
                Password = hasher.HashPassword("Vend123!"),
                Cargo    = "Vendedor",
                Email    = "vendedor@facturacion.ec",
                Rol      = "User"
            }
        };

        await db.Usuarios.AddRangeAsync(usuarios);
        await db.SaveChangesAsync();
        logger.LogInformation("Seed: {Count} usuarios creados", usuarios.Length);
    }

    private static async Task SeedEmisoresAsync(ApplicationDbContext db, ILogger logger)
    {
        if (await db.Set<Emisor>().AnyAsync()) return;

        var emisores = new[]
        {
            new Emisor
            {
                Ruc = "1792146830001",
                RazonSocial = "DISTRIBUIDORA ECUADOR S.A.",
                NombreComercial = "Distecua",
                Direccion = "Av. Amazonas 1234 y Eloy Alfaro",
                Telefono = "022456789",
                Email = "contabilidad@distecua.com",
                ObligadoContabilidad = true,
                Ambiente = AmbienteSRI.Pruebas,
                SerieEstablecimiento = "001",
                SeriePuntoEmision = "001"
            }
        };

        await db.Set<Emisor>().AddRangeAsync(emisores);
        await db.SaveChangesAsync();
        logger.LogInformation("Seed: {Count} emisores creados", emisores.Length);
    }

    private static async Task SeedClientesAsync(ApplicationDbContext db, ILogger logger)
    {
        if (await db.Set<Cliente>().AnyAsync()) return;

        var clientes = new[]
        {
            new Cliente
            {
                TipoIdentificacion = TipoIdentificacion.Ruc,
                NumeroIdentificacion = "1790123456001",
                RazonSocial = "CORPORACION NACIONAL C.A.",
                Email = "compras@corpnac.com",
                Telefono = "022111222",
                Direccion = "Guayaquil, Av. 9 de Octubre 567"
            },
            new Cliente
            {
                TipoIdentificacion = TipoIdentificacion.Cedula,
                NumeroIdentificacion = "1712345678",
                RazonSocial = "JUAN PEREZ LOPEZ",
                Email = "jperez@gmail.com",
                Telefono = "0991234567",
                Direccion = "Quito, Calle 10 y Amazonas"
            },
            new Cliente
            {
                TipoIdentificacion = TipoIdentificacion.ConsumidorFinal,
                NumeroIdentificacion = "9999999999999",
                RazonSocial = "CONSUMIDOR FINAL",
                Email = null,
                Telefono = null,
                Direccion = "Sin dirección"
            }
        };

        await db.Set<Cliente>().AddRangeAsync(clientes);
        await db.SaveChangesAsync();
        logger.LogInformation("Seed: {Count} clientes creados", clientes.Length);
    }

    private static async Task SeedProductosAsync(ApplicationDbContext db, ILogger logger)
    {
        if (await db.Set<Producto>().AnyAsync()) return;

        var productos = new[]
        {
            new Producto { CodigoPrincipal = "P001", Descripcion = "Monitor LED 24 pulgadas", PrecioUnitario = 189.99m, TarifaIva = TarifaIva.Quince, EsServicio = false, Activo = true },
            new Producto { CodigoPrincipal = "P002", Descripcion = "Teclado mecanico RGB", PrecioUnitario = 65.50m, TarifaIva = TarifaIva.Quince, EsServicio = false, Activo = true },
            new Producto { CodigoPrincipal = "P003", Descripcion = "Mouse inalambrico", PrecioUnitario = 25.00m, TarifaIva = TarifaIva.Quince, EsServicio = false, Activo = true },
            new Producto { CodigoPrincipal = "P004", Descripcion = "Disco SSD 512GB", PrecioUnitario = 59.99m, TarifaIva = TarifaIva.Quince, EsServicio = false, Activo = true },
            new Producto { CodigoPrincipal = "P005", Descripcion = "Memoria RAM 16GB DDR4", PrecioUnitario = 45.00m, TarifaIva = TarifaIva.Quince, EsServicio = false, Activo = true },
            new Producto { CodigoPrincipal = "P006", Descripcion = "Servicio de soporte tecnico", PrecioUnitario = 50.00m, TarifaIva = TarifaIva.Quince, EsServicio = true, Activo = true },
            new Producto { CodigoPrincipal = "P007", Descripcion = "Cable HDMI 2m", PrecioUnitario = 8.50m, TarifaIva = TarifaIva.Quince, EsServicio = false, Activo = true },
            new Producto { CodigoPrincipal = "P008", Descripcion = "Impresora multifuncional", PrecioUnitario = 149.00m, TarifaIva = TarifaIva.Quince, EsServicio = false, Activo = true },
            new Producto { CodigoPrincipal = "P009", Descripcion = "Servicio mantenimiento preventivo", PrecioUnitario = 120.00m, TarifaIva = TarifaIva.Quince, EsServicio = true, Activo = true },
            new Producto { CodigoPrincipal = "P010", Descripcion = "Servicio instalacion de software", PrecioUnitario = 35.00m, TarifaIva = TarifaIva.Quince, EsServicio = true, Activo = true }
        };

        await db.Set<Producto>().AddRangeAsync(productos);
        await db.SaveChangesAsync();
        logger.LogInformation("Seed: {Count} productos creados", productos.Length);
    }
}
