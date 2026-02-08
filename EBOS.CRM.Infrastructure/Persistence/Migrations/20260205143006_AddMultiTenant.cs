using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "CRM",
                table: "TaxInformationAddresses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "CRM",
                table: "TaxInformation",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

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
                schema: "CRM",
                table: "Customers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "CRM",
                table: "CustomerAddresses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "CRM",
                table: "CreditTransactions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "CRM",
                table: "CreditAccounts",
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
                schema: "CRM",
                table: "BranchOffices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "CRM",
                table: "BranchOfficeAddresses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "CRM",
                table: "BankInformation",
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

            migrationBuilder.AddColumn<long>(
                name: "TenantId",
                schema: "CRM",
                table: "Addresses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "CRM",
                table: "TaxInformationAddresses");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "CRM",
                table: "TaxInformation");

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
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "CRM",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "CRM",
                table: "CreditTransactions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "CRM",
                table: "CreditAccounts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "EBOS",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "CRM",
                table: "BranchOffices");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "CRM",
                table: "BranchOfficeAddresses");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "CRM",
                table: "BankInformation");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "EBOS",
                table: "AddressTypes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "CRM",
                table: "Addresses");
        }
    }
}
