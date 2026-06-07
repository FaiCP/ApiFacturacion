using Domain.Entities;
using Domain.Enums;
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

    // Facturación electrónica — documentos principales
    public DbSet<Emisor> Emisores { get; set; } = null!;
    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Producto> Productos { get; set; } = null!;
    public DbSet<Factura> Facturas { get; set; } = null!;
    public DbSet<DetalleFactura> DetallesFactura { get; set; } = null!;
    public DbSet<ConfiguracionSRI> ConfiguracionesSRI { get; set; } = null!;

    // Documentos adicionales
    public DbSet<NotaCredito> NotasCredito { get; set; } = null!;
    public DbSet<DetalleNotaCredito> DetallesNotaCredito { get; set; } = null!;
    public DbSet<NotaDebito> NotasDebito { get; set; } = null!;
    public DbSet<MotivoNotaDebito> MotivosNotaDebito { get; set; } = null!;
    public DbSet<Retencion> Retenciones { get; set; } = null!;
    public DbSet<DetalleRetencion> DetallesRetencion { get; set; } = null!;

    public DbSet<Usuario> Usuarios { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── FACTURACIÓN ELECTRÓNICA ──────────────────────────────────────

        modelBuilder.Entity<Emisor>(e =>
        {
            e.ToTable("emisores");
            e.Property(x => x.Ruc).HasMaxLength(13).IsRequired();
            e.Property(x => x.RazonSocial).HasMaxLength(300).IsRequired();
            e.Property(x => x.NombreComercial).HasMaxLength(300);
            e.Property(x => x.Direccion).HasMaxLength(500).IsRequired();
            e.Property(x => x.Telefono).HasMaxLength(20);
            e.Property(x => x.Email).HasMaxLength(100);
            e.Property(x => x.Ambiente).HasConversion<int>();
            e.Property(x => x.SerieEstablecimiento).HasMaxLength(3).IsRequired();
            e.Property(x => x.SeriePuntoEmision).HasMaxLength(3).IsRequired();
            e.Property(x => x.LogoBase64).HasColumnType("text");
            e.HasIndex(x => x.Ruc).IsUnique().HasDatabaseName("IX_Emisor_Ruc");
        });

        modelBuilder.Entity<Cliente>(e =>
        {
            e.ToTable("clientes");
            e.Property(x => x.TipoIdentificacion).HasConversion<int>();
            e.Property(x => x.NumeroIdentificacion).HasMaxLength(13).IsRequired();
            e.Property(x => x.RazonSocial).HasMaxLength(300).IsRequired();
            e.Property(x => x.Email).HasMaxLength(100);
            e.Property(x => x.Telefono).HasMaxLength(20);
            e.Property(x => x.Direccion).HasMaxLength(500);
            e.Property(x => x.Borrado).HasDefaultValue(false);
            e.HasIndex(x => x.NumeroIdentificacion).HasDatabaseName("IX_Cliente_Identificacion");
        });

        modelBuilder.Entity<Producto>(e =>
        {
            e.ToTable("productos");
            e.Property(x => x.CodigoPrincipal).HasMaxLength(50).IsRequired();
            e.Property(x => x.CodigoAuxiliar).HasMaxLength(50);
            e.Property(x => x.Descripcion).HasMaxLength(500).IsRequired();
            e.Property(x => x.PrecioUnitario).HasColumnType("decimal(18,4)");
            e.Property(x => x.TarifaIva).HasConversion<int>();
            e.Property(x => x.Borrado).HasDefaultValue(false);
            e.HasIndex(x => x.CodigoPrincipal).IsUnique().HasDatabaseName("IX_Producto_Codigo");
        });

        modelBuilder.Entity<Factura>(e =>
        {
            e.ToTable("facturas");
            e.Property(x => x.ClaveAcceso).HasMaxLength(49);
            e.Property(x => x.NumeroAutorizacion).HasMaxLength(49);
            e.Property(x => x.Estado).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.MotivoRechazo).HasColumnType("text");
            e.Property(x => x.XmlFirmado).HasColumnType("text");
            e.Property(x => x.Serie).HasMaxLength(7).IsRequired();
            e.Property(x => x.Secuencial).HasMaxLength(9).IsRequired();
            e.Property(x => x.TotalSinImpuestos).HasColumnType("decimal(18,2)");
            e.Property(x => x.TotalDescuento).HasColumnType("decimal(18,2)");
            e.Property(x => x.TotalIva).HasColumnType("decimal(18,2)");
            e.Property(x => x.ImporteTotal).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Emisor).WithMany(em => em.Facturas).HasForeignKey(x => x.EmisorId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Cliente).WithMany(c => c.Facturas).HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => x.ClaveAcceso).IsUnique().HasDatabaseName("IX_Factura_ClaveAcceso");
            e.HasIndex(x => x.Estado).HasDatabaseName("IX_Factura_Estado");
            e.HasIndex(x => new { x.EmisorId, x.Serie, x.Secuencial }).IsUnique().HasDatabaseName("IX_Factura_Secuencial_Unico");
        });

        modelBuilder.Entity<DetalleFactura>(e =>
        {
            e.ToTable("detalle_facturas");
            e.Property(x => x.CodigoPrincipal).HasMaxLength(50).IsRequired();
            e.Property(x => x.Descripcion).HasMaxLength(500).IsRequired();
            e.Property(x => x.Cantidad).HasColumnType("decimal(18,4)");
            e.Property(x => x.PrecioUnitario).HasColumnType("decimal(18,4)");
            e.Property(x => x.Descuento).HasColumnType("decimal(18,2)");
            e.Property(x => x.SubtotalSinImpuesto).HasColumnType("decimal(18,2)");
            e.Property(x => x.TarifaIva).HasConversion<int>();
            e.Property(x => x.ValorIva).HasColumnType("decimal(18,2)");
            e.Property(x => x.PrecioTotalSinImpuesto).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Factura).WithMany(f => f.Detalles).HasForeignKey(x => x.FacturaId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Producto).WithMany(p => p.Detalles).HasForeignKey(x => x.ProductoId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ConfiguracionSRI>(e =>
        {
            e.ToTable("configuracion_sri");
            e.Property(x => x.CertificadoBase64).HasColumnType("text");
            e.Property(x => x.PasswordCertificado).HasMaxLength(500);
            e.Property(x => x.Ambiente).HasConversion<int>();
            e.HasOne(x => x.Emisor).WithMany().HasForeignKey(x => x.EmisorId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => x.EmisorId).HasDatabaseName("IX_ConfiguracionSRI_EmisorId");
        });

        // ── DOCUMENTOS ADICIONALES ───────────────────────────────────────

        modelBuilder.Entity<NotaCredito>(e =>
        {
            e.ToTable("notas_credito");
            e.Property(x => x.ClaveAcceso).HasMaxLength(49);
            e.Property(x => x.NumeroAutorizacion).HasMaxLength(49);
            e.Property(x => x.Estado).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.XmlFirmado).HasColumnType("text");
            e.Property(x => x.MotivoRechazo).HasColumnType("text");
            e.Property(x => x.Serie).HasMaxLength(7).IsRequired();
            e.Property(x => x.Secuencial).HasMaxLength(9).IsRequired();
            e.Property(x => x.Motivo).HasMaxLength(500).IsRequired();
            e.Property(x => x.NumDocModificado).HasMaxLength(20).IsRequired();
            e.Property(x => x.TotalSinImpuestos).HasColumnType("decimal(18,2)");
            e.Property(x => x.TotalIva).HasColumnType("decimal(18,2)");
            e.Property(x => x.ValorModificacion).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Factura).WithMany().HasForeignKey(x => x.FacturaId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Emisor).WithMany().HasForeignKey(x => x.EmisorId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => x.ClaveAcceso).IsUnique().HasDatabaseName("IX_NotaCredito_ClaveAcceso");
            e.HasIndex(x => new { x.EmisorId, x.Serie, x.Secuencial }).IsUnique().HasDatabaseName("IX_NotaCredito_Secuencial_Unico");
        });

        modelBuilder.Entity<DetalleNotaCredito>(e =>
        {
            e.ToTable("detalle_notas_credito");
            e.Property(x => x.CodigoPrincipal).HasMaxLength(50).IsRequired();
            e.Property(x => x.Descripcion).HasMaxLength(500).IsRequired();
            e.Property(x => x.Cantidad).HasColumnType("decimal(18,4)");
            e.Property(x => x.PrecioUnitario).HasColumnType("decimal(18,4)");
            e.Property(x => x.Descuento).HasColumnType("decimal(18,2)");
            e.Property(x => x.SubtotalSinImpuesto).HasColumnType("decimal(18,2)");
            e.Property(x => x.TarifaIva).HasConversion<int>();
            e.Property(x => x.ValorIva).HasColumnType("decimal(18,2)");
            e.Property(x => x.PrecioTotalSinImpuesto).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.NotaCredito).WithMany(n => n.Detalles).HasForeignKey(x => x.NotaCreditoId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<NotaDebito>(e =>
        {
            e.ToTable("notas_debito");
            e.Property(x => x.ClaveAcceso).HasMaxLength(49);
            e.Property(x => x.NumeroAutorizacion).HasMaxLength(49);
            e.Property(x => x.Estado).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.XmlFirmado).HasColumnType("text");
            e.Property(x => x.MotivoRechazo).HasColumnType("text");
            e.Property(x => x.Serie).HasMaxLength(7).IsRequired();
            e.Property(x => x.Secuencial).HasMaxLength(9).IsRequired();
            e.Property(x => x.NumDocModificado).HasMaxLength(20).IsRequired();
            e.Property(x => x.TotalSinImpuestos).HasColumnType("decimal(18,2)");
            e.Property(x => x.TotalIva).HasColumnType("decimal(18,2)");
            e.Property(x => x.ValorTotal).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Factura).WithMany().HasForeignKey(x => x.FacturaId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Emisor).WithMany().HasForeignKey(x => x.EmisorId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Cliente).WithMany().HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => x.ClaveAcceso).IsUnique().HasDatabaseName("IX_NotaDebito_ClaveAcceso");
            e.HasIndex(x => new { x.EmisorId, x.Serie, x.Secuencial }).IsUnique().HasDatabaseName("IX_NotaDebito_Secuencial_Unico");
        });

        modelBuilder.Entity<MotivoNotaDebito>(e =>
        {
            e.ToTable("motivos_nota_debito");
            e.Property(x => x.Razon).HasMaxLength(500).IsRequired();
            e.Property(x => x.Valor).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.NotaDebito).WithMany(n => n.Motivos).HasForeignKey(x => x.NotaDebitoId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Retencion>(e =>
        {
            e.ToTable("retenciones");
            e.Property(x => x.ClaveAcceso).HasMaxLength(49);
            e.Property(x => x.NumeroAutorizacion).HasMaxLength(49);
            e.Property(x => x.Estado).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.XmlFirmado).HasColumnType("text");
            e.Property(x => x.MotivoRechazo).HasColumnType("text");
            e.Property(x => x.Serie).HasMaxLength(7).IsRequired();
            e.Property(x => x.Secuencial).HasMaxLength(9).IsRequired();
            e.Property(x => x.PeriodoFiscal).HasMaxLength(7).IsRequired();
            e.HasOne(x => x.Factura).WithMany().HasForeignKey(x => x.FacturaId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Emisor).WithMany().HasForeignKey(x => x.EmisorId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.SujetoRetenido).WithMany().HasForeignKey(x => x.SujetoRetenidoId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => x.ClaveAcceso).IsUnique().HasDatabaseName("IX_Retencion_ClaveAcceso");
            e.HasIndex(x => new { x.EmisorId, x.Serie, x.Secuencial }).IsUnique().HasDatabaseName("IX_Retencion_Secuencial_Unico");
        });

        modelBuilder.Entity<DetalleRetencion>(e =>
        {
            e.ToTable("detalle_retenciones");
            e.Property(x => x.TipoImpuesto).HasConversion<int>();
            e.Property(x => x.CodigoRetencion).HasMaxLength(10).IsRequired();
            e.Property(x => x.BaseImponible).HasColumnType("decimal(18,2)");
            e.Property(x => x.PorcentajeRetener).HasColumnType("decimal(5,2)");
            e.Property(x => x.ValorRetenido).HasColumnType("decimal(18,2)");
            e.Property(x => x.CodDocSustento).HasMaxLength(2);
            e.Property(x => x.NumDocSustento).HasMaxLength(20);
            e.HasOne(x => x.Retencion).WithMany(r => r.Detalles).HasForeignKey(x => x.RetencionId).OnDelete(DeleteBehavior.Cascade);
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
            entity.Property(e => e.Rol).HasColumnName("rol").HasMaxLength(50).HasDefaultValue("User");
            entity.Ignore(e => e.Borrado);
            entity.Ignore(e => e.CreatedAt);
            entity.Ignore(e => e.UpdatedAt);

            entity.HasIndex(e => e.Nombre).HasDatabaseName("IX_Usuario_Nombre").IsUnique();
            entity.HasIndex(e => e.Email).HasDatabaseName("IX_Usuario_Email");
        });
    }
}
