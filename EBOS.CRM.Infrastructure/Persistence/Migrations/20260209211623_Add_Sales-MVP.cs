using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_SalesMVP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leads",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OwnerUserId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EstimatedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    ConvertedOpportunityId = table.Column<long>(type: "bigint", nullable: true),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.Id);
                    table.CheckConstraint("CK_Lead_Email_Valid", "[Email] LIKE '%@%.%'");
                    table.CheckConstraint("CK_Lead_EstimatedValue_NonNegative", "[EstimatedValue] IS NULL OR [EstimatedValue] >= 0");
                    table.CheckConstraint("CK_Lead_Phone_Digits", "[Phone] NOT LIKE '%[^0-9]%'");
                });

            migrationBuilder.CreateTable(
                name: "OpportunityStages",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    DefaultProbability = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    IsWon = table.Column<bool>(type: "bit", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunityStages", x => x.Id);
                    table.CheckConstraint("CK_OpportunityStage_DefaultProbability_Range", "[DefaultProbability] >= 0 AND [DefaultProbability] <= 1");
                });

            migrationBuilder.CreateTable(
                name: "Opportunities",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StageId = table.Column<long>(type: "bigint", nullable: false),
                    OwnerUserId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    ExpectedCloseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Probability = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SourceLeadId = table.Column<long>(type: "bigint", nullable: true),
                    CloseReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opportunities", x => x.Id);
                    table.CheckConstraint("CK_Opportunity_Amount_NonNegative", "[Amount] >= 0");
                    table.CheckConstraint("CK_Opportunity_Probability_Range", "[Probability] >= 0 AND [Probability] <= 1");
                    table.ForeignKey(
                        name: "FK_Opportunities_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "CRM",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Opportunities_Leads_SourceLeadId",
                        column: x => x.SourceLeadId,
                        principalSchema: "CRM",
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Opportunities_OpportunityStages_StageId",
                        column: x => x.StageId,
                        principalSchema: "CRM",
                        principalTable: "OpportunityStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Quotes",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    OpportunityId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SubtotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.Id);
                    table.CheckConstraint("CK_Quote_Discount_Lte_Subtotal", "[DiscountAmount] <= [SubtotalAmount]");
                    table.CheckConstraint("CK_Quote_Discount_NonNegative", "[DiscountAmount] >= 0");
                    table.CheckConstraint("CK_Quote_Subtotal_NonNegative", "[SubtotalAmount] >= 0");
                    table.CheckConstraint("CK_Quote_Total_NonNegative", "[TotalAmount] >= 0");
                    table.ForeignKey(
                        name: "FK_Quotes_Opportunities_OpportunityId",
                        column: x => x.OpportunityId,
                        principalSchema: "CRM",
                        principalTable: "Opportunities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lead_Status_CreatedAt",
                schema: "CRM",
                table: "Leads",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Lead_TenantId",
                schema: "CRM",
                table: "Leads",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_CustomerId",
                schema: "CRM",
                table: "Opportunities",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_SourceLeadId",
                schema: "CRM",
                table: "Opportunities",
                column: "SourceLeadId",
                unique: true,
                filter: "[SourceLeadId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_Owner_Stage",
                schema: "CRM",
                table: "Opportunities",
                columns: new[] { "OwnerUserId", "StageId" });

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_Stage_CloseDate",
                schema: "CRM",
                table: "Opportunities",
                columns: new[] { "StageId", "ExpectedCloseDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_TenantId",
                schema: "CRM",
                table: "Opportunities",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityStage_TenantId",
                schema: "CRM",
                table: "OpportunityStages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityStage_TenantId_Order",
                schema: "CRM",
                table: "OpportunityStages",
                columns: new[] { "TenantId", "Order" });

            migrationBuilder.CreateIndex(
                name: "UX_OpportunityStage_TenantId_Name",
                schema: "CRM",
                table: "OpportunityStages",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quote_Opportunity_Status",
                schema: "CRM",
                table: "Quotes",
                columns: new[] { "OpportunityId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Quote_TenantId",
                schema: "CRM",
                table: "Quotes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "UX_Quote_TenantId_ReferenceNumber",
                schema: "CRM",
                table: "Quotes",
                columns: new[] { "TenantId", "ReferenceNumber" },
                unique: true,
                filter: "[ReferenceNumber] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Quotes",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "Opportunities",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "Leads",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "OpportunityStages",
                schema: "CRM");
        }
    }
}
