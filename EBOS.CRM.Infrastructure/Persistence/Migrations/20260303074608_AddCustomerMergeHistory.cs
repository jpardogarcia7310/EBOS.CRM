using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerMergeHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerMergeHistories",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    WinnerCustomerId = table.Column<long>(type: "bigint", nullable: false),
                    MergedCustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerMergeHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerMergeHistories_Customers_MergedCustomerId",
                        column: x => x.MergedCustomerId,
                        principalSchema: "CRM",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerMergeHistories_Customers_WinnerCustomerId",
                        column: x => x.WinnerCustomerId,
                        principalSchema: "CRM",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMergeHistories_MergedCustomerId",
                schema: "CRM",
                table: "CustomerMergeHistories",
                column: "MergedCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMergeHistories_WinnerCustomerId",
                schema: "CRM",
                table: "CustomerMergeHistories",
                column: "WinnerCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMergeHistory_Tenant_Merged",
                schema: "CRM",
                table: "CustomerMergeHistories",
                columns: new[] { "TenantId", "MergedCustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMergeHistory_Tenant_Winner_CreatedAt",
                schema: "CRM",
                table: "CustomerMergeHistories",
                columns: new[] { "TenantId", "WinnerCustomerId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerMergeHistories",
                schema: "CRM");
        }
    }
}
