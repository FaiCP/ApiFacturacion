using Domain.Entities;
using Infrastructure.Persistence;

namespace GestorAdmi.Tests.Integration.Infrastructure;

/// <summary>
/// Siembra datos mínimos para pruebas de integración.
/// </summary>
public static class DatabaseSeeder
{
    public static void Seed(ApplicationDbContext db)
    {
        if (db.Set<Usuario>().Any())
            return;

        // Usuario de prueba con contraseña "password123" hasheada con BCrypt
        db.Set<Usuario>().Add(new Usuario
        {
            Id = 1,
            Nombre = "Admin Test",
            Email = "admin@test.com",
            Password = BCrypt.Net.BCrypt.HashPassword("password123"),
            Cargo = "Administrador",
            Borrado = false
        });

        // Hardware de prueba
        db.Set<Hardware>().AddRange(
            new Hardware
            {
                Id = 1,
                IdEquipo = "EQ-001",
                NombreDispositivo = "Laptop",
                Marca = "Dell",
                Modelo = "XPS 15",
                CodigoCne = "CNE-001",
                Estado = "Activo",
                Ubicacion = "Oficina 1",
                Borrado = false
            },
            new Hardware
            {
                Id = 2,
                IdEquipo = "EQ-002",
                NombreDispositivo = "Monitor",
                Marca = "LG",
                Modelo = "27UK850",
                CodigoCne = "CNE-002",
                Estado = "Activo",
                Ubicacion = "Oficina 2",
                Borrado = false
            }
        );

        db.SaveChanges();
    }
}
