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
                name: "AddressTypes",
                schema: "EBOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressTypes", x => x.Id);
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
                    table.CheckConstraint("CK_Country_IsoA2_Uppercase", "UPPER([Iso31661A2Code]) = [Iso31661A2Code]");
                    table.CheckConstraint("CK_Country_IsoA3_Uppercase", "UPPER([Iso31661A3Code]) = [Iso31661A3Code]");
                    table.CheckConstraint("CK_Country_IsoNum_Digits", "[Iso31661NumCode] NOT LIKE '%[^0-9]%'");
                });

            migrationBuilder.CreateTable(
                name: "IdentificationTypes",
                schema: "EBOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentificationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                schema: "EBOS",
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
                name: "Addresses",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ExternalNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    InternalNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BetweenStreet1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BetweenStreet2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Neighbourhood = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    StateOrProvince = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GoogleMapsUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Latitude = table.Column<double>(type: "float(10)", precision: 10, scale: 6, nullable: true),
                    Longitude = table.Column<double>(type: "float(10)", precision: 10, scale: 6, nullable: true),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    CountryId = table.Column<long>(type: "bigint", nullable: false),
                    AddressTypeId = table.Column<long>(type: "bigint", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.CheckConstraint("CK_Address_GoogleMapsUrl_Valid", "[GoogleMapsUrl] IS NULL OR [GoogleMapsUrl] LIKE 'https://maps.%'");
                    table.CheckConstraint("CK_Address_IsPrimary_Boolean", "[IsPrimary] IN (0, 1)");
                    table.CheckConstraint("CK_Address_Latitude_Range", "[Latitude] IS NULL OR ([Latitude] BETWEEN -90 AND 90)");
                    table.CheckConstraint("CK_Address_Longitude_Range", "[Longitude] IS NULL OR ([Longitude] BETWEEN -180 AND 180)");
                    table.CheckConstraint("CK_Address_PostalCode_Length", "LEN([PostalCode]) >= 3");
                    table.ForeignKey(
                        name: "FK_Addresses_AddressTypes_AddressTypeId",
                        column: x => x.AddressTypeId,
                        principalSchema: "EBOS",
                        principalTable: "AddressTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Addresses_Countries_CountryId",
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
                    PrimaryAddressId = table.Column<long>(type: "bigint", nullable: false),
                    StatusId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerType = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TaxIdentification = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdentificationNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IdentificationTypeId = table.Column<long>(type: "bigint", nullable: true),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.CheckConstraint("CK_CorporateCustomer_TaxId_Valid", "[TaxIdentification] NOT LIKE '%[^A-Za-z0-9]%'");
                    table.CheckConstraint("CK_Customer_Email_Valid", "[Email] LIKE '%@%.%'");
                    table.CheckConstraint("CK_Customer_Phone_Digits", "[Phone] NOT LIKE '%[^0-9]%'");
                    table.ForeignKey(
                        name: "FK_Customers_Addresses_PrimaryAddressId",
                        column: x => x.PrimaryAddressId,
                        principalSchema: "CRM",
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Customers_IdentificationTypes_IdentificationTypeId",
                        column: x => x.IdentificationTypeId,
                        principalSchema: "EBOS",
                        principalTable: "IdentificationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Customers_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "EBOS",
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankInformation_Customers_CustomerId",
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
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AddressId = table.Column<long>(type: "bigint", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchOffices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchOffices_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "CRM",
                        principalTable: "Addresses",
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
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditAccounts", x => x.Id);
                    table.CheckConstraint("CK_CreditAccount_MaxAmount_Positive", "[MaxAmount] > 0");
                    table.CheckConstraint("CK_CreditAccount_UsedAmount_NonNegative", "[UsedAmount] >= 0");
                    table.CheckConstraint("CK_CreditAccount_UsedAmount_WithinLimit", "[UsedAmount] <= [MaxAmount]");
                    table.ForeignKey(
                        name: "FK_CreditAccounts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "CRM",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    AddressId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxInformation", x => x.Id);
                    table.CheckConstraint("CK_TaxInformation_TIN_Valid", "[TaxIdentificationNumber] NOT LIKE '%[^A-Za-z0-9]%'");
                    table.ForeignKey(
                        name: "FK_TaxInformation_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "CRM",
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaxInformation_Customers_CustomerId",
                        column: x => x.CustomerId,
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
                    CreditAccountId = table.Column<long>(type: "bigint", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditTransactions", x => x.Id);
                    table.CheckConstraint("CK_CreditTransaction_Amount_NotZero", "[Amount] <> 0");
                    table.CheckConstraint("CK_CreditTransaction_Type_Valid", "[Type] IN ('Consumo', 'Ajuste', 'Devolucion')");
                    table.ForeignKey(
                        name: "FK_CreditTransactions_CreditAccounts_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalSchema: "CRM",
                        principalTable: "CreditAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_City_State",
                schema: "CRM",
                table: "Addresses",
                columns: new[] { "City", "StateOrProvince" });

            migrationBuilder.CreateIndex(
                name: "IX_Address_Country_City",
                schema: "CRM",
                table: "Addresses",
                columns: new[] { "CountryId", "City" });

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
                name: "IX_Address_Unique_Primary",
                schema: "CRM",
                table: "Addresses",
                columns: new[] { "CustomerId", "IsPrimary" },
                unique: true,
                filter: "[IsPrimary] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_AddressTypeId",
                schema: "CRM",
                table: "Addresses",
                column: "AddressTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BankInformation_CustomerId",
                schema: "CRM",
                table: "BankInformation",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BranchOffice_AddressId",
                schema: "CRM",
                table: "BranchOffices",
                column: "AddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BranchOffice_CorporateCustomerId",
                schema: "CRM",
                table: "BranchOffices",
                column: "CorporateCustomerId");

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
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransaction_Account_Date",
                schema: "CRM",
                table: "CreditTransactions",
                columns: new[] { "CreditAccountId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransaction_Date_Account",
                schema: "CRM",
                table: "CreditTransactions",
                columns: new[] { "Date", "CreditAccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_CreditTransactions_CreditAccountId",
                schema: "CRM",
                table: "CreditTransactions",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Status_CreatedAt",
                schema: "CRM",
                table: "Customers",
                columns: new[] { "StatusId", "CreatedAt" });

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
                name: "IX_IndividualCustomer_IdentificationTypeId",
                schema: "CRM",
                table: "Customers",
                column: "IdentificationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxInformation_AddressId",
                schema: "CRM",
                table: "TaxInformation",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxInformation_CustomerId",
                schema: "CRM",
                table: "TaxInformation",
                column: "CustomerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Customers_CustomerId",
                schema: "CRM",
                table: "Addresses",
                column: "CustomerId",
                principalSchema: "CRM",
                principalTable: "Customers",
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
                name: "FK_Addresses_Countries_CountryId",
                schema: "CRM",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Customers_CustomerId",
                schema: "CRM",
                table: "Addresses");

            migrationBuilder.DropTable(
                name: "BankInformation",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "BranchOffices",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "CreditTransactions",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "TaxInformation",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "CreditAccounts",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "AddressTypes",
                schema: "EBOS");

            migrationBuilder.DropTable(
                name: "Countries",
                schema: "EBOS");

            migrationBuilder.DropTable(
                name: "Customers",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "Addresses",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "IdentificationTypes",
                schema: "EBOS");

            migrationBuilder.DropTable(
                name: "Statuses",
                schema: "EBOS");
        }
    }
}
