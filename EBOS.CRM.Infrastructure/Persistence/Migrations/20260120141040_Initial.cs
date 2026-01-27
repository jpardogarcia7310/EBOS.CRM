using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "CRM");

            migrationBuilder.EnsureSchema(
                name: "EBOS");

            migrationBuilder.CreateTable(
                name: "BankInformation",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Iban = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    Bic = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankInformation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                schema: "EBOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Iso31661A2Code = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Iso31661A3Code = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Iso31661NumCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Domain = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    InternationalPhoneCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                    table.CheckConstraint("CK_Countries_IsoA2_Length", "LEN([Iso31661A2Code]) = 2");
                    table.CheckConstraint("CK_Countries_IsoA3_Length", "LEN([Iso31661A3Code]) = 3");
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxInformation",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaxName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TaxIdentificationNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FiscalAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    City = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CountryId = table.Column<long>(type: "bigint", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxInformation_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "EBOS",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaxInformationId = table.Column<long>(type: "bigint", nullable: true),
                    BankInformationId = table.Column<long>(type: "bigint", nullable: true),
                    StatusId = table.Column<long>(type: "bigint", nullable: false),
                    TipoCliente = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TaxIdentification = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdentityDocument = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_BankInformation_BankInformationId",
                        column: x => x.BankInformationId,
                        principalSchema: "CRM",
                        principalTable: "BankInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Customers_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "CRM",
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Customers_TaxInformation_TaxInformationId",
                        column: x => x.TaxInformationId,
                        principalSchema: "CRM",
                        principalTable: "TaxInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ExternalNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    InternalNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BetweenStreet1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BetweenStreet2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Neighborhood = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    StateOrProvince = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GoogleMapsUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Latitude = table.Column<double>(type: "float(10)", precision: 10, scale: 6, nullable: true),
                    Longitude = table.Column<double>(type: "float(10)", precision: 10, scale: 6, nullable: true),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    CountryId = table.Column<long>(type: "bigint", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "EBOS",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Addresses_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "CRM",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BranchOffices",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorporateCustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AddressLine = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    City = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CountryId = table.Column<long>(type: "bigint", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchOffices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchOffices_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "EBOS",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BranchOffices_Customers_CorporateCustomerId",
                        column: x => x.CorporateCustomerId,
                        principalSchema: "CRM",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CreditAccounts",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UsedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ClienteId = table.Column<long>(type: "bigint", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditAccounts_Customers_ClienteId",
                        column: x => x.ClienteId,
                        principalSchema: "CRM",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CreditTransactions",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExternalReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreditoId = table.Column<long>(type: "bigint", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditTransactions_CreditAccounts_CreditoId",
                        column: x => x.CreditoId,
                        principalSchema: "CRM",
                        principalTable: "CreditAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_CountryId",
                schema: "CRM",
                table: "Addresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_CustomerId",
                schema: "CRM",
                table: "Addresses",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchOffice_CorporateCustomerId",
                schema: "CRM",
                table: "BranchOffices",
                column: "CorporateCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchOffice_CountryId",
                schema: "CRM",
                table: "BranchOffices",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_CurrencyCode",
                schema: "EBOS",
                table: "Countries",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Domain",
                schema: "EBOS",
                table: "Countries",
                column: "Domain");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Iso31661A2Code",
                schema: "EBOS",
                table: "Countries",
                column: "Iso31661A2Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Iso31661A3Code",
                schema: "EBOS",
                table: "Countries",
                column: "Iso31661A3Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Iso31661NumCode",
                schema: "EBOS",
                table: "Countries",
                column: "Iso31661NumCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Name",
                schema: "EBOS",
                table: "Countries",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CreditAccount_CustomerId",
                schema: "CRM",
                table: "CreditAccounts",
                column: "ClienteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_CreditAccountId",
                schema: "CRM",
                table: "CreditTransactions",
                column: "CreditoId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_BankInformationId",
                schema: "CRM",
                table: "Customers",
                column: "BankInformationId",
                unique: true,
                filter: "[BankInformationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_StatusId",
                schema: "CRM",
                table: "Customers",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_TaxInformationId",
                schema: "CRM",
                table: "Customers",
                column: "TaxInformationId",
                unique: true,
                filter: "[TaxInformationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TaxInformation_CountryId",
                schema: "CRM",
                table: "TaxInformation",
                column: "CountryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "BranchOffices",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "CreditTransactions",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "CreditAccounts",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "Customers",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "BankInformation",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "Statuses",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "TaxInformation",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "Countries",
                schema: "EBOS");
        }
    }
}
