using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEbosTenantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "EBOS",
                table: "Statuses");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "EBOS",
                table: "IdentificationTypes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "EBOS",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "EBOS",
                table: "AddressTypes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "EBOS",
                table: "Statuses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "EBOS",
                table: "IdentificationTypes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "EBOS",
                table: "Countries",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "EBOS",
                table: "AddressTypes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
