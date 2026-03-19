using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

/// <summary>
/// DbContext principal de la aplicación
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<Hardware> Hardware { get; set; } = null!;
    public DbSet<CaracteristicaComputadora> CaracteristicasComputadora { get; set; } = null!;
    public DbSet<Departamento> Departamentos { get; set; } = null!;
    public DbSet<Custodio> Custodios { get; set; } = null!;
    public DbSet<GestionActivo> GestionActivos { get; set; } = null!;
    public DbSet<Kit> Kits { get; set; } = null!;
    public DbSet<Persona> Personas { get; set; } = null!;
    public DbSet<Suministro> Suministros { get; set; } = null!;
    public DbSet<ControlActivo> ControlesActivos { get; set; } = null!;
    public DbSet<HistorialCustodio> HistorialCustodios { get; set; } = null!;
    public DbSet<Usuario> Usuarios { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Hardware
        modelBuilder.Entity<Hardware>(entity =>
        {
            entity.ToTable("gestion_hardware");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdEquipo).HasColumnName("id_equipo").HasMaxLength(255);
            entity.Property(e => e.Marca).HasColumnName("marca").HasMaxLength(255);
            entity.Property(e => e.Modelo).HasColumnName("modelo").HasMaxLength(255);
            entity.Property(e => e.FechaAdquisicion).HasColumnName("fecha_adquisicion");
            entity.Property(e => e.Estado).HasColumnName("estado").HasMaxLength(255);
            entity.Property(e => e.Ubicacion).HasColumnName("ubicacion").HasMaxLength(255);
            entity.Property(e => e.CodigoCne).HasColumnName("codigo_cne").HasMaxLength(255);
            entity.Property(e => e.NombreDispositivo).HasColumnName("nombre_dispositivo").HasMaxLength(255);
            entity.Property(e => e.IdSuministro).HasColumnName("id_suministro");
            entity.Property(e => e.Observacion).HasColumnName("observacion").HasColumnType("text");
            entity.Property(e => e.Valor).HasColumnName("valor").HasColumnType("decimal(18,2)");
            entity.Ignore(e => e.Borrado);
            entity.Ignore(e => e.CreatedAt);
            entity.Ignore(e => e.UpdatedAt);

            entity.HasIndex(e => e.IdEquipo).HasDatabaseName("IX_Hardware_IdEquipo");
            entity.HasIndex(e => e.Estado).HasDatabaseName("IX_Hardware_Estado");
        });

        // Configuración de CaracteristicaComputadora
        modelBuilder.Entity<CaracteristicaComputadora>(entity =>
        {
            entity.ToTable("caracteristicas_computadora");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdEquipo).HasColumnName("id_equipo").HasMaxLength(255);
            entity.Property(e => e.Ram).HasColumnName("ram").HasMaxLength(255);
            entity.Property(e => e.Rom).HasColumnName("rom").HasMaxLength(255);
            entity.Property(e => e.Procesador).HasColumnName("procesador").HasMaxLength(255);
            entity.Ignore(e => e.HardwareId);
            entity.Ignore(e => e.Borrado);
            entity.Ignore(e => e.CreatedAt);
            entity.Ignore(e => e.UpdatedAt);

            entity.HasOne(e => e.Hardware)
                .WithMany(h => h.Caracteristicas)
                .HasForeignKey(e => e.IdEquipo)
                .HasPrincipalKey(h => h.IdEquipo)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de Departamento
        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.ToTable("departamentos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(255).IsRequired();
            entity.Ignore(e => e.Borrado);
            entity.Ignore(e => e.CreatedAt);
            entity.Ignore(e => e.UpdatedAt);
        });

        // Configuración de Custodio
        modelBuilder.Entity<Custodio>(entity =>
        {
            entity.ToTable("Custodios");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(255);
            entity.Property(e => e.Cargo).HasColumnName("cargo").HasMaxLength(255);
            entity.Property(e => e.Cedula).HasColumnName("cedula").HasMaxLength(255);
            entity.Property(e => e.IdDepartamento).HasColumnName("id_departamento");
            entity.Ignore(e => e.Borrado);
            entity.Ignore(e => e.CreatedAt);
            entity.Ignore(e => e.UpdatedAt);

            entity.HasOne(e => e.Departamento)
                .WithMany(d => d.Custodios)
                .HasForeignKey(e => e.IdDepartamento)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.Cedula).HasDatabaseName("IX_Custodio_Cedula");
        });

        // Configuración de GestionActivo
        modelBuilder.Entity<GestionActivo>(entity =>
        {
            entity.ToTable("gestion_activos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdEquipo).HasColumnName("id_equipo").HasMaxLength(255);
            entity.Property(e => e.IdCustodio).HasColumnName("id_custodio");
            entity.Property(e => e.FechaAsignacion).HasColumnName("fecha_asignacion");
            entity.Property(e => e.FechaDevolucion).HasColumnName("fecha_devolucion");
            entity.Ignore(e => e.Borrado);
            entity.Ignore(e => e.CreatedAt);
            entity.Ignore(e => e.UpdatedAt);

            entity.HasOne(e => e.Custodio)
                .WithMany(c => c.GestionActivos)
                .HasForeignKey(e => e.IdCustodio)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Hardware)
                .WithMany(h => h.GestionActivos)
                .HasForeignKey(e => e.IdEquipo)
                .HasPrincipalKey(h => h.IdEquipo)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de Kit
        modelBuilder.Entity<Kit>(entity =>
        {
            entity.ToTable("Kits");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Insumo).HasColumnName("INSUMO").HasMaxLength(255);
            entity.Property(e => e.Cantidad).HasColumnName("CANTIDAD").HasMaxLength(255);
            entity.Property(e => e.Estado).HasColumnName("ESTADO").HasMaxLength(255);
            entity.Property(e => e.Marca).HasColumnName("MARCA").HasMaxLength(255);
            entity.Property(e => e.Serie).HasColumnName("Serie").HasMaxLength(255);
            entity.Property(e => e.Modelo).HasColumnName("MODELO").HasMaxLength(255);
            entity.Property(e => e.Observacion).HasColumnName("OBSERVACION").HasMaxLength(255);
            entity.Ignore(e => e.Borrado);
            entity.Ignore(e => e.CreatedAt);
            entity.Ignore(e => e.UpdatedAt);
        });

        // Configuración de Persona
        modelBuilder.Entity<Persona>(entity =>
        {
            entity.ToTable("Personal");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(255);
            entity.Property(e => e.Cedula).HasColumnName("cedula").HasMaxLength(255);
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.Cargo).HasColumnName("cargo").HasMaxLength(255);
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);
            entity.Property(e => e.TempPass).HasColumnName("tempPass").HasMaxLength(255);
            entity.Ignore(e => e.Borrado);
            entity.Ignore(e => e.CreatedAt);
            entity.Ignore(e => e.UpdatedAt);
        });

        // Configuración de Suministro
        modelBuilder.Entity<Suministro>(entity =>
        {
            entity.ToTable("suministros_remanufacturados");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdEquipo).HasColumnName("id_equipo").HasMaxLength(255);
            entity.Property(e => e.TipoSuministro).HasColumnName("tipo_suministro").HasMaxLength(255);
            entity.Property(e => e.FechaRetiro).HasColumnName("fecha_retiro");
            entity.Property(e => e.IdEquipoAsignado).HasColumnName("id_equipoAsignado").HasMaxLength(255);
            entity.Ignore(e => e.HardwareId);
            entity.Ignore(e => e.Borrado);
            entity.Ignore(e => e.CreatedAt);
            entity.Ignore(e => e.UpdatedAt);

            entity.HasOne(e => e.Hardware)
                .WithMany(h => h.Suministros)
                .HasForeignKey(e => e.IdEquipo)
                .HasPrincipalKey(h => h.IdEquipo)
                .IsRequired(false);
        });

        // Configuración de ControlActivo
        modelBuilder.Entity<ControlActivo>(entity =>
        {
            entity.ToTable("control_activos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdEquipo).HasColumnName("id_equipo").HasMaxLength(255);
            entity.Property(e => e.FechaAuditoria).HasColumnName("fecha_auditoria");
            entity.Property(e => e.DetallesAuditoria).HasColumnName("detalles_auditoria").HasColumnType("text");
            entity.Property(e => e.Custodio).HasColumnName("custodio").HasMaxLength(255);
            entity.Ignore(e => e.HardwareId);
            entity.Ignore(e => e.Borrado);
            entity.Ignore(e => e.CreatedAt);
            entity.Ignore(e => e.UpdatedAt);

            entity.HasOne(e => e.Hardware)
                .WithMany(h => h.ControlesActivos)
                .HasForeignKey(e => e.IdEquipo)
                .HasPrincipalKey(h => h.IdEquipo)
                .IsRequired(false);
        });

        // Configuración de HistorialCustodio
        modelBuilder.Entity<HistorialCustodio>(entity =>
        {
            entity.ToTable("historial_custodios");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TimestampEvento).HasColumnName("timestamp_evento");
            entity.Property(e => e.Custodio).HasColumnName("custodio").HasMaxLength(255);
            entity.Property(e => e.IdDepartamento).HasColumnName("id_departamento");
            entity.Ignore(e => e.Borrado);
            entity.Ignore(e => e.CreatedAt);
            entity.Ignore(e => e.UpdatedAt);
        });

        // Configuración de Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Password).HasColumnName("Pass").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Cargo).HasColumnName("cargo").HasMaxLength(50);
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(50);
            entity.Ignore(e => e.Borrado);
            entity.Ignore(e => e.CreatedAt);
            entity.Ignore(e => e.UpdatedAt);

            entity.HasIndex(e => e.Nombre).HasDatabaseName("IX_Usuario_Nombre").IsUnique();
            entity.HasIndex(e => e.Email).HasDatabaseName("IX_Usuario_Email");
        });
    }
}
