using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmisorIdToConfiguracionSRI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EmisorId",
                table: "configuracion_sri",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            // Filas existentes (sistema previo era de un solo emisor): asignar al primer emisor registrado.
            migrationBuilder.Sql(@"
                UPDATE configuracion_sri
                SET ""EmisorId"" = (SELECT MIN(""Id"") FROM emisores)
                WHERE ""EmisorId"" = 0 AND EXISTS (SELECT 1 FROM emisores);
            ");

            // Huérfanas sin emisor al que enlazar (no pueden satisfacer la FK): eliminar.
            migrationBuilder.Sql(@"DELETE FROM configuracion_sri WHERE ""EmisorId"" = 0;");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionSRI_EmisorId",
                table: "configuracion_sri",
                column: "EmisorId");

            migrationBuilder.AddForeignKey(
                name: "FK_configuracion_sri_emisores_EmisorId",
                table: "configuracion_sri",
                column: "EmisorId",
                principalTable: "emisores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_configuracion_sri_emisores_EmisorId",
                table: "configuracion_sri");

            migrationBuilder.DropIndex(
                name: "IX_ConfiguracionSRI_EmisorId",
                table: "configuracion_sri");

            migrationBuilder.DropColumn(
                name: "EmisorId",
                table: "configuracion_sri");
        }
    }
}
