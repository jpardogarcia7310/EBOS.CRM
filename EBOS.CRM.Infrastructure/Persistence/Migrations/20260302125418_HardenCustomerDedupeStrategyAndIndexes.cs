using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class HardenCustomerDedupeStrategyAndIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CorporateCustomer_TenantId_TaxIdentification",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customer_TenantId_Email",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customer_TenantId_Phone",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_IndividualCustomer_TenantId_IdentificationNumber",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateCustomer_TenantId_TaxIdentification",
                schema: "CRM",
                table: "Customers",
                columns: new[] { "TenantId", "TaxIdentification" },
                filter: "[CustomerType] = 'Corporate' AND [Erased] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_TenantId_Email",
                schema: "CRM",
                table: "Customers",
                columns: new[] { "TenantId", "Email" },
                filter: "[Erased] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_TenantId_Phone",
                schema: "CRM",
                table: "Customers",
                columns: new[] { "TenantId", "Phone" },
                filter: "[Erased] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualCustomer_TenantId_IdentificationNumber",
                schema: "CRM",
                table: "Customers",
                columns: new[] { "TenantId", "IdentificationNumber" },
                filter: "[CustomerType] = 'Individual' AND [Erased] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CorporateCustomer_TenantId_TaxIdentification",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customer_TenantId_Email",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customer_TenantId_Phone",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_IndividualCustomer_TenantId_IdentificationNumber",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateCustomer_TenantId_TaxIdentification",
                schema: "CRM",
                table: "Customers",
                columns: new[] { "TenantId", "TaxIdentification" });

            migrationBuilder.CreateIndex(
                name: "IX_Customer_TenantId_Email",
                schema: "CRM",
                table: "Customers",
                columns: new[] { "TenantId", "Email" });

            migrationBuilder.CreateIndex(
                name: "IX_Customer_TenantId_Phone",
                schema: "CRM",
                table: "Customers",
                columns: new[] { "TenantId", "Phone" });

            migrationBuilder.CreateIndex(
                name: "IX_IndividualCustomer_TenantId_IdentificationNumber",
                schema: "CRM",
                table: "Customers",
                columns: new[] { "TenantId", "IdentificationNumber" });
        }
    }
}
