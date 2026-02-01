using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BranchOffices_Addresses_AddressId",
                schema: "CRM",
                table: "BranchOffices");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Addresses_PrimaryAddressId",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxInformation_Addresses_AddressId",
                schema: "CRM",
                table: "TaxInformation");

            migrationBuilder.DropIndex(
                name: "IX_TaxInformation_AddressId",
                schema: "CRM",
                table: "TaxInformation");

            migrationBuilder.DropIndex(
                name: "IX_Customers_PrimaryAddressId",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_StatusId",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_BranchOffice_AddressId",
                schema: "CRM",
                table: "BranchOffices");

            migrationBuilder.DropIndex(
                name: "IX_Address_CountryId",
                schema: "CRM",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Address_Unique_Primary",
                schema: "CRM",
                table: "Addresses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Address_IsPrimary_Boolean",
                schema: "CRM",
                table: "Addresses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Address_Latitude_Range",
                schema: "CRM",
                table: "Addresses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Address_Longitude_Range",
                schema: "CRM",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "AddressId",
                schema: "CRM",
                table: "TaxInformation");

            migrationBuilder.DropColumn(
                name: "PrimaryAddressId",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "AddressId",
                schema: "CRM",
                table: "BranchOffices");

            migrationBuilder.DropColumn(
                name: "IsPrimary",
                schema: "CRM",
                table: "Addresses");

            migrationBuilder.RenameIndex(
                name: "IX_Address_CustomerId",
                schema: "CRM",
                table: "Addresses",
                newName: "IX_Addresses_CustomerId");

            migrationBuilder.AddColumn<bool>(
                name: "AllowsMultiple",
                schema: "EBOS",
                table: "AddressTypes",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                schema: "EBOS",
                table: "AddressTypes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresPrimary",
                schema: "EBOS",
                table: "AddressTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                schema: "CRM",
                table: "Addresses",
                type: "decimal(10,6)",
                precision: 10,
                scale: 6,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float(10)",
                oldPrecision: 10,
                oldScale: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                schema: "CRM",
                table: "Addresses",
                type: "decimal(10,6)",
                precision: 10,
                scale: 6,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float(10)",
                oldPrecision: 10,
                oldScale: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CustomerId",
                schema: "CRM",
                table: "Addresses",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Address_Latitude_Range",
                schema: "CRM",
                table: "Addresses",
                sql: "[Latitude] IS NULL OR ([Latitude] BETWEEN -90 AND 90)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Address_Longitude_Range",
                schema: "CRM",
                table: "Addresses",
                sql: "[Longitude] IS NULL OR ([Longitude] BETWEEN -180 AND 180)");

            migrationBuilder.CreateTable(
                name: "BranchOfficeAddresses",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchOfficeId = table.Column<long>(type: "bigint", nullable: false),
                    AddressId = table.Column<long>(type: "bigint", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchOfficeAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchOfficeAddresses_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "CRM",
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BranchOfficeAddresses_BranchOffices_BranchOfficeId",
                        column: x => x.BranchOfficeId,
                        principalSchema: "CRM",
                        principalTable: "BranchOffices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerAddresses",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    AddressId = table.Column<long>(type: "bigint", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAddresses", x => x.Id);
                    table.CheckConstraint("CK_CustomerAddress_ValidFrom_NotNull", "[ValidFrom] IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_CustomerAddresses_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "CRM",
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerAddresses_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "CRM",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaxInformationAddresses",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaxInformationId = table.Column<long>(type: "bigint", nullable: false),
                    AddressId = table.Column<long>(type: "bigint", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxInformationAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxInformationAddresses_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "CRM",
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaxInformationAddresses_TaxInformation_TaxInformationId",
                        column: x => x.TaxInformationId,
                        principalSchema: "CRM",
                        principalTable: "TaxInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BranchOfficeAddress_Current_Primary",
                schema: "CRM",
                table: "BranchOfficeAddresses",
                columns: new[] { "BranchOfficeId", "IsCurrent", "IsPrimary" });

            migrationBuilder.CreateIndex(
                name: "IX_BranchOfficeAddresses_AddressId",
                schema: "CRM",
                table: "BranchOfficeAddresses",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddress_Current_Primary",
                schema: "CRM",
                table: "CustomerAddresses",
                columns: new[] { "CustomerId", "IsCurrent", "IsPrimary" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_AddressId",
                schema: "CRM",
                table: "CustomerAddresses",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxInformationAddress_Current_Primary",
                schema: "CRM",
                table: "TaxInformationAddresses",
                columns: new[] { "TaxInformationId", "IsCurrent", "IsPrimary" });

            migrationBuilder.CreateIndex(
                name: "IX_TaxInformationAddresses_AddressId",
                schema: "CRM",
                table: "TaxInformationAddresses",
                column: "AddressId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BranchOfficeAddresses",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "CustomerAddresses",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "TaxInformationAddresses",
                schema: "CRM");

            migrationBuilder.DropColumn(
                name: "AllowsMultiple",
                schema: "EBOS",
                table: "AddressTypes");

            migrationBuilder.DropColumn(
                name: "Category",
                schema: "EBOS",
                table: "AddressTypes");

            migrationBuilder.DropColumn(
                name: "RequiresPrimary",
                schema: "EBOS",
                table: "AddressTypes");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_CustomerId",
                schema: "CRM",
                table: "Addresses",
                newName: "IX_Address_CustomerId");

            migrationBuilder.AddColumn<long>(
                name: "AddressId",
                schema: "CRM",
                table: "TaxInformation",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PrimaryAddressId",
                schema: "CRM",
                table: "Customers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AddressId",
                schema: "CRM",
                table: "BranchOffices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.DropCheckConstraint(
                name: "CK_Address_Latitude_Range",
                schema: "CRM",
                table: "Addresses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Address_Longitude_Range",
                schema: "CRM",
                table: "Addresses");

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                schema: "CRM",
                table: "Addresses",
                type: "float(10)",
                precision: 10,
                scale: 6,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,6)",
                oldPrecision: 10,
                oldScale: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                schema: "CRM",
                table: "Addresses",
                type: "float(10)",
                precision: 10,
                scale: 6,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,6)",
                oldPrecision: 10,
                oldScale: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CustomerId",
                schema: "CRM",
                table: "Addresses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                schema: "CRM",
                table: "Addresses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TaxInformation_AddressId",
                schema: "CRM",
                table: "TaxInformation",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PrimaryAddressId",
                schema: "CRM",
                table: "Customers",
                column: "PrimaryAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_StatusId",
                schema: "CRM",
                table: "Customers",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchOffice_AddressId",
                schema: "CRM",
                table: "BranchOffices",
                column: "AddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Address_CountryId",
                schema: "CRM",
                table: "Addresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_Unique_Primary",
                schema: "CRM",
                table: "Addresses",
                columns: new[] { "CustomerId", "IsPrimary" },
                unique: true,
                filter: "[IsPrimary] = 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Address_IsPrimary_Boolean",
                schema: "CRM",
                table: "Addresses",
                sql: "[IsPrimary] IN (0, 1)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Address_Latitude_Range",
                schema: "CRM",
                table: "Addresses",
                sql: "[Latitude] IS NULL OR ([Latitude] BETWEEN -90 AND 90)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Address_Longitude_Range",
                schema: "CRM",
                table: "Addresses",
                sql: "[Longitude] IS NULL OR ([Longitude] BETWEEN -180 AND 180)");

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
                name: "FK_Customers_Addresses_PrimaryAddressId",
                schema: "CRM",
                table: "Customers",
                column: "PrimaryAddressId",
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
    }
}
