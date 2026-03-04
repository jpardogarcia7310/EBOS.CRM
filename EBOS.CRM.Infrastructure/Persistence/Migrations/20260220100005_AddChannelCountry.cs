using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddChannelCountry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChannelCountries",
                schema: "EBOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChannelTypeId = table.Column<long>(type: "bigint", nullable: false),
                    CountryId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelCountries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChannelCountries_ChannelTypes_ChannelTypeId",
                        column: x => x.ChannelTypeId,
                        principalSchema: "EBOS",
                        principalTable: "ChannelTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChannelCountries_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "EBOS",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ChannelCountries_CountryId",
                schema: "EBOS",
                table: "ChannelCountries",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelCountries_IsActive",
                schema: "EBOS",
                table: "ChannelCountries",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "UX_ChannelCountries_ChannelTypeId_CountryId",
                schema: "EBOS",
                table: "ChannelCountries",
                columns: new[] { "ChannelTypeId", "CountryId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelCountries",
                schema: "EBOS");

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
        }
    }
}
