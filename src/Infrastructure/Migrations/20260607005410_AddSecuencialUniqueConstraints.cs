using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSecuencialUniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_retenciones_EmisorId",
                table: "retenciones");

            migrationBuilder.DropIndex(
                name: "IX_notas_debito_EmisorId",
                table: "notas_debito");

            migrationBuilder.DropIndex(
                name: "IX_notas_credito_EmisorId",
                table: "notas_credito");

            migrationBuilder.DropIndex(
                name: "IX_Factura_ClaveAcceso",
                table: "facturas");

            migrationBuilder.DropIndex(
                name: "IX_facturas_EmisorId",
                table: "facturas");

            migrationBuilder.CreateIndex(
                name: "IX_Retencion_ClaveAcceso",
                table: "retenciones",
                column: "ClaveAcceso",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Retencion_Secuencial_Unico",
                table: "retenciones",
                columns: new[] { "EmisorId", "Serie", "Secuencial" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotaDebito_ClaveAcceso",
                table: "notas_debito",
                column: "ClaveAcceso",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotaDebito_Secuencial_Unico",
                table: "notas_debito",
                columns: new[] { "EmisorId", "Serie", "Secuencial" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotaCredito_ClaveAcceso",
                table: "notas_credito",
                column: "ClaveAcceso",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotaCredito_Secuencial_Unico",
                table: "notas_credito",
                columns: new[] { "EmisorId", "Serie", "Secuencial" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Factura_ClaveAcceso",
                table: "facturas",
                column: "ClaveAcceso",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Factura_Secuencial_Unico",
                table: "facturas",
                columns: new[] { "EmisorId", "Serie", "Secuencial" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Retencion_ClaveAcceso",
                table: "retenciones");

            migrationBuilder.DropIndex(
                name: "IX_Retencion_Secuencial_Unico",
                table: "retenciones");

            migrationBuilder.DropIndex(
                name: "IX_NotaDebito_ClaveAcceso",
                table: "notas_debito");

            migrationBuilder.DropIndex(
                name: "IX_NotaDebito_Secuencial_Unico",
                table: "notas_debito");

            migrationBuilder.DropIndex(
                name: "IX_NotaCredito_ClaveAcceso",
                table: "notas_credito");

            migrationBuilder.DropIndex(
                name: "IX_NotaCredito_Secuencial_Unico",
                table: "notas_credito");

            migrationBuilder.DropIndex(
                name: "IX_Factura_ClaveAcceso",
                table: "facturas");

            migrationBuilder.DropIndex(
                name: "IX_Factura_Secuencial_Unico",
                table: "facturas");

            migrationBuilder.CreateIndex(
                name: "IX_retenciones_EmisorId",
                table: "retenciones",
                column: "EmisorId");

            migrationBuilder.CreateIndex(
                name: "IX_notas_debito_EmisorId",
                table: "notas_debito",
                column: "EmisorId");

            migrationBuilder.CreateIndex(
                name: "IX_notas_credito_EmisorId",
                table: "notas_credito",
                column: "EmisorId");

            migrationBuilder.CreateIndex(
                name: "IX_Factura_ClaveAcceso",
                table: "facturas",
                column: "ClaveAcceso");

            migrationBuilder.CreateIndex(
                name: "IX_facturas_EmisorId",
                table: "facturas",
                column: "EmisorId");
        }
    }
}
