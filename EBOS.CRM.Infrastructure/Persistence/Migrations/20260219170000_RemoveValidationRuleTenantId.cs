using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations;

public partial class RemoveValidationRuleTenantId : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "UX_ValidationRule_TenantId_Key",
            schema: "EBOS",
            table: "ValidationRules");

        migrationBuilder.DropColumn(
            name: "TenantId",
            schema: "EBOS",
            table: "ValidationRules");

        migrationBuilder.CreateIndex(
            name: "UX_ValidationRule_Key",
            schema: "EBOS",
            table: "ValidationRules",
            column: "Key",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "UX_ValidationRule_Key",
            schema: "EBOS",
            table: "ValidationRules");

        migrationBuilder.AddColumn<long>(
            name: "TenantId",
            schema: "EBOS",
            table: "ValidationRules",
            type: "bigint",
            nullable: false,
            defaultValue: 0L);

        migrationBuilder.CreateIndex(
            name: "UX_ValidationRule_TenantId_Key",
            schema: "EBOS",
            table: "ValidationRules",
            columns: new[] { "TenantId", "Key" },
            unique: true);
    }
}
