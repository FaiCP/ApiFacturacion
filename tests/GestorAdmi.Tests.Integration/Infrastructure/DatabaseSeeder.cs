using Domain.Entities;
using Infrastructure.Persistence;

namespace GestorAdmi.Tests.Integration.Infrastructure;

public static class DatabaseSeeder
{
    public static void Seed(ApplicationDbContext db)
    {
        if (db.Set<Usuario>().Any())
            return;

        db.Set<Usuario>().Add(new Usuario
        {
            Id = 1,
            Nombre = "Admin Test",
            Email = "admin@test.com",
            Password = BCrypt.Net.BCrypt.HashPassword("password123"),
            Cargo = "Administrador",
            Borrado = false
        });

        db.SaveChanges();
    }
}
