using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_AddressTypeIdentificationType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditAccounts_Customers_ClienteId",
                schema: "CRM",
                table: "CreditAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditTransactions_CreditAccounts_CreditoId",
                schema: "CRM",
                table: "CreditTransactions");

            migrationBuilder.DropColumn(
                name: "AddressType",
                schema: "CRM",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "IdentityDocument",
                schema: "CRM",
                table: "Customers",
                newName: "IdentificationNumber");

            migrationBuilder.RenameColumn(
                name: "CreditoId",
                schema: "CRM",
                table: "CreditTransactions",
                newName: "CreditAccountId");

            migrationBuilder.RenameColumn(
                name: "ClienteId",
                schema: "CRM",
                table: "CreditAccounts",
                newName: "CustomerId");

            migrationBuilder.AddColumn<long>(
                name: "IdentificationTypeId",
                schema: "CRM",
                table: "Customers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AddressTypeId",
                schema: "CRM",
                table: "Addresses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_IndividualCustomer_IdentificationTypeId",
                schema: "CRM",
                table: "Customers",
                column: "IdentificationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_AddressTypeId",
                schema: "CRM",
                table: "Addresses",
                column: "AddressTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_AddressTypes_AddressTypeId",
                schema: "CRM",
                table: "Addresses",
                column: "AddressTypeId",
                principalSchema: "EBOS",
                principalTable: "AddressTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditAccounts_Customers_CustomerId",
                schema: "CRM",
                table: "CreditAccounts",
                column: "CustomerId",
                principalSchema: "CRM",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditTransactions_CreditAccounts_CreditAccountId",
                schema: "CRM",
                table: "CreditTransactions",
                column: "CreditAccountId",
                principalSchema: "CRM",
                principalTable: "CreditAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_IdentificationTypes_IdentificationTypeId",
                schema: "CRM",
                table: "Customers",
                column: "IdentificationTypeId",
                principalSchema: "EBOS",
                principalTable: "IdentificationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_AddressTypes_AddressTypeId",
                schema: "CRM",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditAccounts_Customers_CustomerId",
                schema: "CRM",
                table: "CreditAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditTransactions_CreditAccounts_CreditAccountId",
                schema: "CRM",
                table: "CreditTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_IdentificationTypes_IdentificationTypeId",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_IndividualCustomer_IdentificationTypeId",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_AddressTypeId",
                schema: "CRM",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "IdentificationTypeId",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "AddressTypeId",
                schema: "CRM",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "IdentificationNumber",
                schema: "CRM",
                table: "Customers",
                newName: "IdentityDocument");

            migrationBuilder.RenameColumn(
                name: "CreditAccountId",
                schema: "CRM",
                table: "CreditTransactions",
                newName: "CreditoId");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                schema: "CRM",
                table: "CreditAccounts",
                newName: "ClienteId");

            migrationBuilder.AddColumn<string>(
                name: "AddressType",
                schema: "CRM",
                table: "Addresses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditAccounts_Customers_ClienteId",
                schema: "CRM",
                table: "CreditAccounts",
                column: "ClienteId",
                principalSchema: "CRM",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditTransactions_CreditAccounts_CreditoId",
                schema: "CRM",
                table: "CreditTransactions",
                column: "CreditoId",
                principalSchema: "CRM",
                principalTable: "CreditAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
