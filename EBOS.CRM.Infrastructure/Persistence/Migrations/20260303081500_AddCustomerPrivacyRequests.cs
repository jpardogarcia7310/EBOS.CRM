using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerPrivacyRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerPrivacyRequests",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    RequestType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequestedBy = table.Column<long>(type: "bigint", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    ProcessedBy = table.Column<long>(type: "bigint", nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPrivacyRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerPrivacyRequests_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "CRM",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPrivacyRequest_Tenant_Customer_RequestedAt",
                schema: "CRM",
                table: "CustomerPrivacyRequests",
                columns: new[] { "TenantId", "CustomerId", "RequestedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPrivacyRequest_Tenant_Status_RequestedAt",
                schema: "CRM",
                table: "CustomerPrivacyRequests",
                columns: new[] { "TenantId", "Status", "RequestedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPrivacyRequests_CustomerId",
                schema: "CRM",
                table: "CustomerPrivacyRequests",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "UX_CustomerPrivacyRequest_ActiveByType",
                schema: "CRM",
                table: "CustomerPrivacyRequests",
                columns: new[] { "TenantId", "CustomerId", "RequestType" },
                unique: true,
                filter: "[Erased] = 0 AND [Status] IN ('PENDING','IN_PROGRESS')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerPrivacyRequests",
                schema: "CRM");
        }
    }
}
