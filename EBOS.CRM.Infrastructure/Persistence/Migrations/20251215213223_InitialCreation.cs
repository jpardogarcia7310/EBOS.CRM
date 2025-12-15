using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "EBOS");

            migrationBuilder.EnsureSchema(
                name: "CRM");

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
                name: "TaxRegimes",
                schema: "EBOS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxRegimes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Balance = table.Column<decimal>(type: "money", nullable: false, defaultValue: 0m),
                    IsCompany = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CompanyType = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    RFC = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CURP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TaxDuplicateShippingAddress = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreditLimit = table.Column<decimal>(type: "money", nullable: false, defaultValue: 0m),
                    StatusId = table.Column<long>(type: "bigint", nullable: false),
                    TaxRegimeId = table.Column<long>(type: "bigint", nullable: false),
                    TaxAddressId = table.Column<long>(type: "bigint", nullable: false),
                    ShippingAddressId = table.Column<long>(type: "bigint", nullable: true),
                    SalesConfigurationId = table.Column<long>(type: "bigint", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "EBOS",
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Customers_TaxRegimes_TaxRegimeId",
                        column: x => x.TaxRegimeId,
                        principalSchema: "EBOS",
                        principalTable: "TaxRegimes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesData",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HasCredit = table.Column<bool>(type: "bit", nullable: false),
                    CreditDays = table.Column<int>(type: "int", nullable: true),
                    ReviewDay = table.Column<int>(type: "int", nullable: true),
                    PaymentDay = table.Column<int>(type: "int", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(23)", maxLength: 23, nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "money", nullable: false, defaultValue: 0m),
                    AccountingAccount = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    SellerId = table.Column<long>(type: "bigint", nullable: false),
                    PaymentMethodId = table.Column<long>(type: "bigint", nullable: false),
                    PriceListId = table.Column<long>(type: "bigint", nullable: false),
                    TemplateDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesData", x => x.Id);
                    table.CheckConstraint("CK_Country_CreditDays_PositiveMultipleOf30", "[CreditDays] IS NULL OR ([CreditDays] > 0 AND [CreditDays] % 30 = 0)");
                    table.ForeignKey(
                        name: "FK_SalesData_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "CRM",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShippingAddresses",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TradeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(255)", maxLength: 200, nullable: false),
                    ExternalNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    InternalNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BetweenStreet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AndStreet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Municipality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Neighborhood = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Zone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    CountryId = table.Column<long>(type: "bigint", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingAddresses_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "EBOS",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingAddresses_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "CRM",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaxAddresses",
                schema: "CRM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Street = table.Column<string>(type: "nvarchar(255)", maxLength: 200, nullable: false),
                    ExternalNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    InternalNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Municipality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Neighborhood = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    CountryId = table.Column<long>(type: "bigint", nullable: false),
                    Erased = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxAddresses_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "EBOS",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaxAddresses_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "CRM",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_Customers_StatusId",
                schema: "CRM",
                table: "Customers",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_TaxRegimeId",
                schema: "CRM",
                table: "Customers",
                column: "TaxRegimeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesData_CustomerId",
                schema: "CRM",
                table: "SalesData",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingAddresses_CountryId",
                schema: "CRM",
                table: "ShippingAddresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingAddresses_CustomerId",
                schema: "CRM",
                table: "ShippingAddresses",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaxAddresses_CountryId",
                schema: "CRM",
                table: "TaxAddresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxAddresses_CustomerId",
                schema: "CRM",
                table: "TaxAddresses",
                column: "CustomerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesData",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "ShippingAddresses",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "TaxAddresses",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "Countries",
                schema: "EBOS");

            migrationBuilder.DropTable(
                name: "Customers",
                schema: "CRM");

            migrationBuilder.DropTable(
                name: "Statuses",
                schema: "EBOS");

            migrationBuilder.DropTable(
                name: "TaxRegimes",
                schema: "EBOS");
        }
    }
}
