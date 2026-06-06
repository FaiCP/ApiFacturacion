using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_PG : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TipoIdentificacion = table.Column<int>(type: "integer", nullable: false),
                    NumeroIdentificacion = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    RazonSocial = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Direccion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Borrado = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "configuracion_sri",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CertificadoBase64 = table.Column<string>(type: "text", nullable: true),
                    PasswordCertificado = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Ambiente = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaVencimientoCert = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Borrado = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configuracion_sri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "emisores",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ruc = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    RazonSocial = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    NombreComercial = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Direccion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ObligadoContabilidad = table.Column<bool>(type: "boolean", nullable: false),
                    Ambiente = table.Column<int>(type: "integer", nullable: false),
                    SerieEstablecimiento = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    SeriePuntoEmision = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    LogoBase64 = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Borrado = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emisores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "productos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoPrincipal = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CodigoAuxiliar = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TarifaIva = table.Column<int>(type: "integer", nullable: false),
                    EsServicio = table.Column<bool>(type: "boolean", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Borrado = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Pass = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    cargo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    rol = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "User")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "facturas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClaveAcceso = table.Column<string>(type: "character varying(49)", maxLength: 49, nullable: true),
                    NumeroAutorizacion = table.Column<string>(type: "character varying(49)", maxLength: 49, nullable: true),
                    FechaAutorizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MotivoRechazo = table.Column<string>(type: "text", nullable: true),
                    XmlFirmado = table.Column<string>(type: "text", nullable: true),
                    FechaEmision = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Serie = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    Secuencial = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    TotalSinImpuestos = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalDescuento = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalIva = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ImporteTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EmisorId = table.Column<long>(type: "bigint", nullable: false),
                    ClienteId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Borrado = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facturas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_facturas_clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_facturas_emisores_EmisorId",
                        column: x => x.EmisorId,
                        principalTable: "emisores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "detalle_facturas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FacturaId = table.Column<long>(type: "bigint", nullable: false),
                    ProductoId = table.Column<long>(type: "bigint", nullable: true),
                    CodigoPrincipal = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Descuento = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SubtotalSinImpuesto = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TarifaIva = table.Column<int>(type: "integer", nullable: false),
                    ValorIva = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PrecioTotalSinImpuesto = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Borrado = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detalle_facturas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_detalle_facturas_facturas_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "facturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_detalle_facturas_productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "notas_credito",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClaveAcceso = table.Column<string>(type: "character varying(49)", maxLength: 49, nullable: true),
                    NumeroAutorizacion = table.Column<string>(type: "character varying(49)", maxLength: 49, nullable: true),
                    FechaAutorizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MotivoRechazo = table.Column<string>(type: "text", nullable: true),
                    XmlFirmado = table.Column<string>(type: "text", nullable: true),
                    FechaEmision = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Serie = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    Secuencial = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    Motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FacturaId = table.Column<long>(type: "bigint", nullable: false),
                    NumDocModificado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FechaEmisionDocSustento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TotalSinImpuestos = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalIva = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ValorModificacion = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EmisorId = table.Column<long>(type: "bigint", nullable: false),
                    ClienteId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Borrado = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notas_credito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notas_credito_clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_notas_credito_emisores_EmisorId",
                        column: x => x.EmisorId,
                        principalTable: "emisores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_notas_credito_facturas_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "facturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notas_debito",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClaveAcceso = table.Column<string>(type: "character varying(49)", maxLength: 49, nullable: true),
                    NumeroAutorizacion = table.Column<string>(type: "character varying(49)", maxLength: 49, nullable: true),
                    FechaAutorizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MotivoRechazo = table.Column<string>(type: "text", nullable: true),
                    XmlFirmado = table.Column<string>(type: "text", nullable: true),
                    FechaEmision = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Serie = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    Secuencial = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    FacturaId = table.Column<long>(type: "bigint", nullable: false),
                    NumDocModificado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FechaEmisionDocSustento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TotalSinImpuestos = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalIva = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EmisorId = table.Column<long>(type: "bigint", nullable: false),
                    ClienteId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Borrado = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notas_debito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notas_debito_clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_notas_debito_emisores_EmisorId",
                        column: x => x.EmisorId,
                        principalTable: "emisores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_notas_debito_facturas_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "facturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "retenciones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClaveAcceso = table.Column<string>(type: "character varying(49)", maxLength: 49, nullable: true),
                    NumeroAutorizacion = table.Column<string>(type: "character varying(49)", maxLength: 49, nullable: true),
                    FechaAutorizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MotivoRechazo = table.Column<string>(type: "text", nullable: true),
                    XmlFirmado = table.Column<string>(type: "text", nullable: true),
                    FechaEmision = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Serie = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    Secuencial = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    PeriodoFiscal = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    FacturaId = table.Column<long>(type: "bigint", nullable: false),
                    EmisorId = table.Column<long>(type: "bigint", nullable: false),
                    SujetoRetenidoId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Borrado = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_retenciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_retenciones_clientes_SujetoRetenidoId",
                        column: x => x.SujetoRetenidoId,
                        principalTable: "clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_retenciones_emisores_EmisorId",
                        column: x => x.EmisorId,
                        principalTable: "emisores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_retenciones_facturas_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "facturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "detalle_notas_credito",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NotaCreditoId = table.Column<long>(type: "bigint", nullable: false),
                    CodigoPrincipal = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Descuento = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SubtotalSinImpuesto = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TarifaIva = table.Column<int>(type: "integer", nullable: false),
                    ValorIva = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PrecioTotalSinImpuesto = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Borrado = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detalle_notas_credito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_detalle_notas_credito_notas_credito_NotaCreditoId",
                        column: x => x.NotaCreditoId,
                        principalTable: "notas_credito",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "motivos_nota_debito",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NotaDebitoId = table.Column<long>(type: "bigint", nullable: false),
                    Razon = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Borrado = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_motivos_nota_debito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_motivos_nota_debito_notas_debito_NotaDebitoId",
                        column: x => x.NotaDebitoId,
                        principalTable: "notas_debito",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "detalle_retenciones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RetencionId = table.Column<long>(type: "bigint", nullable: false),
                    TipoImpuesto = table.Column<int>(type: "integer", nullable: false),
                    CodigoRetencion = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    BaseImponible = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PorcentajeRetener = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    ValorRetenido = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CodDocSustento = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    NumDocSustento = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FechaEmisionDocSustento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Borrado = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detalle_retenciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_detalle_retenciones_retenciones_RetencionId",
                        column: x => x.RetencionId,
                        principalTable: "retenciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Identificacion",
                table: "clientes",
                column: "NumeroIdentificacion");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_facturas_FacturaId",
                table: "detalle_facturas",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_facturas_ProductoId",
                table: "detalle_facturas",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_notas_credito_NotaCreditoId",
                table: "detalle_notas_credito",
                column: "NotaCreditoId");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_retenciones_RetencionId",
                table: "detalle_retenciones",
                column: "RetencionId");

            migrationBuilder.CreateIndex(
                name: "IX_Emisor_Ruc",
                table: "emisores",
                column: "Ruc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Factura_ClaveAcceso",
                table: "facturas",
                column: "ClaveAcceso");

            migrationBuilder.CreateIndex(
                name: "IX_Factura_Estado",
                table: "facturas",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_facturas_ClienteId",
                table: "facturas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_facturas_EmisorId",
                table: "facturas",
                column: "EmisorId");

            migrationBuilder.CreateIndex(
                name: "IX_motivos_nota_debito_NotaDebitoId",
                table: "motivos_nota_debito",
                column: "NotaDebitoId");

            migrationBuilder.CreateIndex(
                name: "IX_notas_credito_ClienteId",
                table: "notas_credito",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_notas_credito_EmisorId",
                table: "notas_credito",
                column: "EmisorId");

            migrationBuilder.CreateIndex(
                name: "IX_notas_credito_FacturaId",
                table: "notas_credito",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_notas_debito_ClienteId",
                table: "notas_debito",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_notas_debito_EmisorId",
                table: "notas_debito",
                column: "EmisorId");

            migrationBuilder.CreateIndex(
                name: "IX_notas_debito_FacturaId",
                table: "notas_debito",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_Codigo",
                table: "productos",
                column: "CodigoPrincipal",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_retenciones_EmisorId",
                table: "retenciones",
                column: "EmisorId");

            migrationBuilder.CreateIndex(
                name: "IX_retenciones_FacturaId",
                table: "retenciones",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_retenciones_SujetoRetenidoId",
                table: "retenciones",
                column: "SujetoRetenidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Email",
                table: "Usuarios",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Nombre",
                table: "Usuarios",
                column: "nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "configuracion_sri");

            migrationBuilder.DropTable(
                name: "detalle_facturas");

            migrationBuilder.DropTable(
                name: "detalle_notas_credito");

            migrationBuilder.DropTable(
                name: "detalle_retenciones");

            migrationBuilder.DropTable(
                name: "motivos_nota_debito");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "productos");

            migrationBuilder.DropTable(
                name: "notas_credito");

            migrationBuilder.DropTable(
                name: "retenciones");

            migrationBuilder.DropTable(
                name: "notas_debito");

            migrationBuilder.DropTable(
                name: "facturas");

            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropTable(
                name: "emisores");
        }
    }
}
