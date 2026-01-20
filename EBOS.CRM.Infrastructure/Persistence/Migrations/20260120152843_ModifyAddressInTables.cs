using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifyAddressInTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BranchOffices_Countries_CountryId",
                schema: "CRM",
                table: "BranchOffices");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxInformation_Countries_CountryId",
                schema: "CRM",
                table: "TaxInformation");

            migrationBuilder.DropColumn(
                name: "City",
                schema: "CRM",
                table: "TaxInformation");

            migrationBuilder.DropColumn(
                name: "FiscalAddress",
                schema: "CRM",
                table: "TaxInformation");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                schema: "CRM",
                table: "TaxInformation");

            migrationBuilder.DropColumn(
                name: "AddressLine",
                schema: "CRM",
                table: "BranchOffices");

            migrationBuilder.DropColumn(
                name: "City",
                schema: "CRM",
                table: "BranchOffices");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                schema: "CRM",
                table: "BranchOffices");

            migrationBuilder.RenameColumn(
                name: "CountryId",
                schema: "CRM",
                table: "TaxInformation",
                newName: "AddressId");

            migrationBuilder.RenameIndex(
                name: "IX_TaxInformation_CountryId",
                schema: "CRM",
                table: "TaxInformation",
                newName: "IX_TaxInformation_AddressId");

            migrationBuilder.RenameColumn(
                name: "CountryId",
                schema: "CRM",
                table: "BranchOffices",
                newName: "AddressId");

            migrationBuilder.RenameIndex(
                name: "IX_BranchOffice_CountryId",
                schema: "CRM",
                table: "BranchOffices",
                newName: "IX_BranchOffice_AddressId");

            migrationBuilder.RenameColumn(
                name: "Type",
                schema: "CRM",
                table: "Addresses",
                newName: "AddressType");

            migrationBuilder.AddColumn<long>(
                name: "AddressId",
                schema: "CRM",
                table: "Customers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AddressId",
                schema: "CRM",
                table: "Customers",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_BranchOffices_Addresses_AddressId",
                schema: "CRM",
                table: "BranchOffices",
                column: "AddressId",
                principalSchema: "CRM",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Addresses_AddressId",
                schema: "CRM",
                table: "Customers",
                column: "AddressId",
                principalSchema: "CRM",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxInformation_Addresses_AddressId",
                schema: "CRM",
                table: "TaxInformation",
                column: "AddressId",
                principalSchema: "CRM",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BranchOffices_Addresses_AddressId",
                schema: "CRM",
                table: "BranchOffices");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Addresses_AddressId",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxInformation_Addresses_AddressId",
                schema: "CRM",
                table: "TaxInformation");

            migrationBuilder.DropIndex(
                name: "IX_Customers_AddressId",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "AddressId",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "AddressId",
                schema: "CRM",
                table: "TaxInformation",
                newName: "CountryId");

            migrationBuilder.RenameIndex(
                name: "IX_TaxInformation_AddressId",
                schema: "CRM",
                table: "TaxInformation",
                newName: "IX_TaxInformation_CountryId");

            migrationBuilder.RenameColumn(
                name: "AddressId",
                schema: "CRM",
                table: "BranchOffices",
                newName: "CountryId");

            migrationBuilder.RenameIndex(
                name: "IX_BranchOffice_AddressId",
                schema: "CRM",
                table: "BranchOffices",
                newName: "IX_BranchOffice_CountryId");

            migrationBuilder.RenameColumn(
                name: "AddressType",
                schema: "CRM",
                table: "Addresses",
                newName: "Type");

            migrationBuilder.AddColumn<string>(
                name: "City",
                schema: "CRM",
                table: "TaxInformation",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FiscalAddress",
                schema: "CRM",
                table: "TaxInformation",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                schema: "CRM",
                table: "TaxInformation",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AddressLine",
                schema: "CRM",
                table: "BranchOffices",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                schema: "CRM",
                table: "BranchOffices",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                schema: "CRM",
                table: "BranchOffices",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_BranchOffices_Countries_CountryId",
                schema: "CRM",
                table: "BranchOffices",
                column: "CountryId",
                principalSchema: "EBOS",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxInformation_Countries_CountryId",
                schema: "CRM",
                table: "TaxInformation",
                column: "CountryId",
                principalSchema: "EBOS",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
