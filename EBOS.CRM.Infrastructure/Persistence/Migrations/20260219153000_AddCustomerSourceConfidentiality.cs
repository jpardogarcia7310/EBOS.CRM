using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations;

public partial class AddCustomerSourceConfidentiality : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Source",
            schema: "CRM",
            table: "Customers",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Confidentiality",
            schema: "CRM",
            table: "Customers",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Source",
            schema: "CRM",
            table: "Customers");

        migrationBuilder.DropColumn(
            name: "Confidentiality",
            schema: "CRM",
            table: "Customers");
    }
}
