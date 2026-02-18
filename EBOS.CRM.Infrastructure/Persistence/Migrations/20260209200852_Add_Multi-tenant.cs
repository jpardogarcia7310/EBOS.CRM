using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Multitenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenantConfigurations",
                schema: "EBOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ValueJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantQuotas",
                schema: "EBOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    Metric = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Limit = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantQuotas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantUsageMetrics",
                schema: "EBOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    Metric = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUsageMetrics", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaxInformationAddress_TenantId",
                schema: "CRM",
                table: "TaxInformationAddresses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxInformation_TenantId",
                schema: "CRM",
                table: "TaxInformation",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_TenantId",
                schema: "CRM",
                table: "Customers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "UX_Customer_TenantId_Code",
                schema: "CRM",
                table: "Customers",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddress_TenantId",
                schema: "CRM",
                table: "CustomerAddresses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransaction_TenantId",
                schema: "CRM",
                table: "CreditTransactions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditAccount_TenantId",
                schema: "CRM",
                table: "CreditAccounts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchOffice_TenantId",
                schema: "CRM",
                table: "BranchOffices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchOfficeAddress_TenantId",
                schema: "CRM",
                table: "BranchOfficeAddresses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_BankInformation_TenantId",
                schema: "CRM",
                table: "BankInformation",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_TenantId",
                schema: "CRM",
                table: "Addresses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantConfiguration_TenantId",
                schema: "EBOS",
                table: "TenantConfigurations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "UX_TenantConfiguration_TenantId_Key",
                schema: "EBOS",
                table: "TenantConfigurations",
                columns: new[] { "TenantId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantQuota_TenantId",
                schema: "EBOS",
                table: "TenantQuotas",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "UX_TenantQuota_TenantId_Metric_EffectiveFrom",
                schema: "EBOS",
                table: "TenantQuotas",
                columns: new[] { "TenantId", "Metric", "EffectiveFrom" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsageMetric_TenantId",
                schema: "EBOS",
                table: "TenantUsageMetrics",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsageMetric_TenantId_Metric_PeriodStart",
                schema: "EBOS",
                table: "TenantUsageMetrics",
                columns: new[] { "TenantId", "Metric", "PeriodStart" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantConfigurations",
                schema: "EBOS");

            migrationBuilder.DropTable(
                name: "TenantQuotas",
                schema: "EBOS");

            migrationBuilder.DropTable(
                name: "TenantUsageMetrics",
                schema: "EBOS");

            migrationBuilder.DropIndex(
                name: "IX_TaxInformationAddress_TenantId",
                schema: "CRM",
                table: "TaxInformationAddresses");

            migrationBuilder.DropIndex(
                name: "IX_TaxInformation_TenantId",
                schema: "CRM",
                table: "TaxInformation");

            migrationBuilder.DropIndex(
                name: "IX_Customer_TenantId",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "UX_Customer_TenantId_Code",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAddress_TenantId",
                schema: "CRM",
                table: "CustomerAddresses");

            migrationBuilder.DropIndex(
                name: "IX_CreditTransaction_TenantId",
                schema: "CRM",
                table: "CreditTransactions");

            migrationBuilder.DropIndex(
                name: "IX_CreditAccount_TenantId",
                schema: "CRM",
                table: "CreditAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BranchOffice_TenantId",
                schema: "CRM",
                table: "BranchOffices");

            migrationBuilder.DropIndex(
                name: "IX_BranchOfficeAddress_TenantId",
                schema: "CRM",
                table: "BranchOfficeAddresses");

            migrationBuilder.DropIndex(
                name: "IX_BankInformation_TenantId",
                schema: "CRM",
                table: "BankInformation");

            migrationBuilder.DropIndex(
                name: "IX_Address_TenantId",
                schema: "CRM",
                table: "Addresses");
        }
    }
}
