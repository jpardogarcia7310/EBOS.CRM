using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerDedupeIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountContacts",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    CorporateCustomerId = table.Column<long>(type: "bigint", nullable: false),
                    IndividualCustomerId = table.Column<long>(type: "bigint", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountContacts_Customers_CorporateCustomerId",
                        column: x => x.CorporateCustomerId,
                        principalSchema: "CRM",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountContacts_Customers_IndividualCustomerId",
                        column: x => x.IndividualCustomerId,
                        principalSchema: "CRM",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountHierarchies",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    ParentCorporateCustomerId = table.Column<long>(type: "bigint", nullable: false),
                    ChildCorporateCustomerId = table.Column<long>(type: "bigint", nullable: false),
                    RelationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountHierarchies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountHierarchies_Customers_ChildCorporateCustomerId",
                        column: x => x.ChildCorporateCustomerId,
                        principalSchema: "CRM",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountHierarchies_Customers_ParentCorporateCustomerId",
                        column: x => x.ParentCorporateCustomerId,
                        principalSchema: "CRM",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CaseActivities",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    CaseId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseActivities_Cases_CaseId",
                        column: x => x.CaseId,
                        principalSchema: "CRM",
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChannelTypes",
                schema: "EBOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerConsents",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    ConsentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Granted = table.Column<bool>(type: "bit", nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerConsents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerConsents_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "CRM",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountContactRoles",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    AccountContactId = table.Column<long>(type: "bigint", nullable: false),
                    RoleCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountContactRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountContactRoles_AccountContacts_AccountContactId",
                        column: x => x.AccountContactId,
                        principalSchema: "CRM",
                        principalTable: "AccountContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerPreferences",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    ChannelId = table.Column<long>(type: "bigint", nullable: false),
                    Preferred = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerPreferences_ChannelTypes_ChannelId",
                        column: x => x.ChannelId,
                        principalSchema: "EBOS",
                        principalTable: "ChannelTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerPreferences_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "CRM",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountContactRole_TenantId_AccountContactId",
                schema: "CRM",
                table: "AccountContactRoles",
                columns: new[] { "TenantId", "AccountContactId" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountContactRoles_AccountContactId",
                schema: "CRM",
                table: "AccountContactRoles",
                column: "AccountContactId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountContact_TenantId_CorporateCustomerId",
                schema: "CRM",
                table: "AccountContacts",
                columns: new[] { "TenantId", "CorporateCustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountContact_TenantId_IndividualCustomerId",
                schema: "CRM",
                table: "AccountContacts",
                columns: new[] { "TenantId", "IndividualCustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountContacts_CorporateCustomerId",
                schema: "CRM",
                table: "AccountContacts",
                column: "CorporateCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountContacts_IndividualCustomerId",
                schema: "CRM",
                table: "AccountContacts",
                column: "IndividualCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHierarchies_ChildCorporateCustomerId",
                schema: "CRM",
                table: "AccountHierarchies",
                column: "ChildCorporateCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHierarchies_ParentCorporateCustomerId",
                schema: "CRM",
                table: "AccountHierarchies",
                column: "ParentCorporateCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHierarchy_TenantId_ChildCorporateCustomerId",
                schema: "CRM",
                table: "AccountHierarchies",
                columns: new[] { "TenantId", "ChildCorporateCustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountHierarchy_TenantId_ParentCorporateCustomerId",
                schema: "CRM",
                table: "AccountHierarchies",
                columns: new[] { "TenantId", "ParentCorporateCustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_CaseActivity_CaseId_Status",
                schema: "CRM",
                table: "CaseActivities",
                columns: new[] { "CaseId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CaseActivity_TenantId",
                schema: "CRM",
                table: "CaseActivities",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelTypes_Descripcion",
                schema: "EBOS",
                table: "ChannelTypes",
                column: "Descripcion");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelTypes_IsActive",
                schema: "EBOS",
                table: "ChannelTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerConsent_TenantId_CustomerId",
                schema: "CRM",
                table: "CustomerConsents",
                columns: new[] { "TenantId", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerConsents_CustomerId",
                schema: "CRM",
                table: "CustomerConsents",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPreferences_ChannelId",
                schema: "CRM",
                table: "CustomerPreferences",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPreferences_CustomerId",
                schema: "CRM",
                table: "CustomerPreferences",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "UX_CustomerPreference_TenantId_Customer_Channel",
                schema: "CRM",
                table: "CustomerPreferences",
                columns: new[] { "TenantId", "CustomerId", "ChannelId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountContactRoles",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "AccountHierarchies",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "CaseActivities",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "CustomerConsents",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "CustomerPreferences",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "AccountContacts",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "ChannelTypes",
                schema: "EBOS");
        }
    }
}
