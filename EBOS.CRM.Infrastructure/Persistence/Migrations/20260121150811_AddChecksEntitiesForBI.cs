using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddChecksEntitiesForBI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropForeignKey(
                name: "FK_BranchOffices_Addresses_AddressId",
                schema: "CRM",
                table: "BranchOffices");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Addresses_AddressId",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxInformation_Addresses_AddressId",
                schema: "CRM",
                table: "TaxInformation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Addresses",
                schema: "CRM",
                table: "Addresses");

            migrationBuilder.RenameTable(
                name: "Addresses",
                schema: "CRM",
                newName: "Address",
                newSchema: "CRM");

            migrationBuilder.RenameColumn(
                name: "TipoCliente",
                schema: "CRM",
                table: "Customers",
                newName: "CustomerType");

            migrationBuilder.RenameColumn(
                name: "AddressId",
                schema: "CRM",
                table: "Customers",
                newName: "PrimaryAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_AddressId",
                schema: "CRM",
                table: "Customers",
                newName: "IX_Customers_PrimaryAddressId");

            migrationBuilder.AlterColumn<string>(
                name: "Neighborhood",
                schema: "CRM",
                table: "Address",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "GoogleMapsUrl",
                schema: "CRM",
                table: "Address",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "BetweenStreet2",
                schema: "CRM",
                table: "Address",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "BetweenStreet1",
                schema: "CRM",
                table: "Address",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                schema: "CRM",
                table: "Address",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Address",
                schema: "CRM",
                table: "Address",
                column: "Id");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TaxInformation_TIN_Valid",
                schema: "CRM",
                table: "TaxInformation",
                sql: "[TaxIdentificationNumber] NOT LIKE '%[^A-Za-z0-9]%'");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Status_CreatedAt",
                schema: "CRM",
                table: "Customers",
                columns: new[] { "StatusId", "CreatedAt" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_CorporateCustomer_TaxId_Valid",
                schema: "CRM",
                table: "Customers",
                sql: "[TaxIdentification] NOT LIKE '%[^A-Za-z0-9]%'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Customer_Email_Valid",
                schema: "CRM",
                table: "Customers",
                sql: "[Email] LIKE '%@%.%'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Customer_Phone_Digits",
                schema: "CRM",
                table: "Customers",
                sql: "[Phone] NOT LIKE '%[^0-9]%'");

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

            migrationBuilder.AddCheckConstraint(
                name: "CK_CreditTransaction_Amount_NotZero",
                schema: "CRM",
                table: "CreditTransactions",
                sql: "[Amount] <> 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CreditTransaction_Type_Valid",
                schema: "CRM",
                table: "CreditTransactions",
                sql: "[Type] IN ('Consumo', 'Ajuste', 'Devolucion')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CreditAccount_MaxAmount_Positive",
                schema: "CRM",
                table: "CreditAccounts",
                sql: "[MaxAmount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CreditAccount_UsedAmount_NonNegative",
                schema: "CRM",
                table: "CreditAccounts",
                sql: "[UsedAmount] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CreditAccount_UsedAmount_WithinLimit",
                schema: "CRM",
                table: "CreditAccounts",
                sql: "[UsedAmount] <= [MaxAmount]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Country_IsoA2_Uppercase",
                schema: "EBOS",
                table: "Countries",
                sql: "UPPER([Iso31661A2Code]) = [Iso31661A2Code]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Country_IsoA3_Uppercase",
                schema: "EBOS",
                table: "Countries",
                sql: "UPPER([Iso31661A3Code]) = [Iso31661A3Code]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Country_IsoNum_Digits",
                schema: "EBOS",
                table: "Countries",
                sql: "[Iso31661NumCode] NOT LIKE '%[^0-9]%'");

            migrationBuilder.CreateIndex(
                name: "IX_Address_City_State",
                schema: "CRM",
                table: "Address",
                columns: new[] { "City", "StateOrProvince" });

            migrationBuilder.CreateIndex(
                name: "IX_Address_Country_City",
                schema: "CRM",
                table: "Address",
                columns: new[] { "CountryId", "City" });

            migrationBuilder.CreateIndex(
                name: "IX_Address_Unique_Primary",
                schema: "CRM",
                table: "Address",
                columns: new[] { "CustomerId", "IsPrimary" },
                unique: true,
                filter: "[IsPrimary] = 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Address_GoogleMapsUrl_Valid",
                schema: "CRM",
                table: "Address",
                sql: "[GoogleMapsUrl] IS NULL OR [GoogleMapsUrl] LIKE 'https://maps.%'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Address_IsPrimary_Boolean",
                schema: "CRM",
                table: "Address",
                sql: "[IsPrimary] IN (0, 1)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Address_Latitude_Range",
                schema: "CRM",
                table: "Address",
                sql: "[Latitude] IS NULL OR ([Latitude] BETWEEN -90 AND 90)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Address_Longitude_Range",
                schema: "CRM",
                table: "Address",
                sql: "[Longitude] IS NULL OR ([Longitude] BETWEEN -180 AND 180)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Address_PostalCode_Length",
                schema: "CRM",
                table: "Address",
                sql: "LEN([PostalCode]) >= 3");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_AddressTypes_AddressTypeId",
                schema: "CRM",
                table: "Address",
                column: "AddressTypeId",
                principalSchema: "EBOS",
                principalTable: "AddressTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Countries_CountryId",
                schema: "CRM",
                table: "Address",
                column: "CountryId",
                principalSchema: "EBOS",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Customers_CustomerId",
                schema: "CRM",
                table: "Address",
                column: "CustomerId",
                principalSchema: "CRM",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BranchOffices_Address_AddressId",
                schema: "CRM",
                table: "BranchOffices",
                column: "AddressId",
                principalSchema: "CRM",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Address_PrimaryAddressId",
                schema: "CRM",
                table: "Customers",
                column: "PrimaryAddressId",
                principalSchema: "CRM",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxInformation_Address_AddressId",
                schema: "CRM",
                table: "TaxInformation",
                column: "AddressId",
                principalSchema: "CRM",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_AddressTypes_AddressTypeId",
                schema: "CRM",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Address_Countries_CountryId",
                schema: "CRM",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Address_Customers_CustomerId",
                schema: "CRM",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_BranchOffices_Address_AddressId",
                schema: "CRM",
                table: "BranchOffices");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Address_PrimaryAddressId",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxInformation_Address_AddressId",
                schema: "CRM",
                table: "TaxInformation");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TaxInformation_TIN_Valid",
                schema: "CRM",
                table: "TaxInformation");

            migrationBuilder.DropIndex(
                name: "IX_Customer_Status_CreatedAt",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CorporateCustomer_TaxId_Valid",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Customer_Email_Valid",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Customer_Phone_Digits",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_CreditTransaction_Account_Date",
                schema: "CRM",
                table: "CreditTransactions");

            migrationBuilder.DropIndex(
                name: "IX_CreditTransaction_Date_Account",
                schema: "CRM",
                table: "CreditTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CreditTransaction_Amount_NotZero",
                schema: "CRM",
                table: "CreditTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CreditTransaction_Type_Valid",
                schema: "CRM",
                table: "CreditTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CreditAccount_MaxAmount_Positive",
                schema: "CRM",
                table: "CreditAccounts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CreditAccount_UsedAmount_NonNegative",
                schema: "CRM",
                table: "CreditAccounts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CreditAccount_UsedAmount_WithinLimit",
                schema: "CRM",
                table: "CreditAccounts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Country_IsoA2_Uppercase",
                schema: "EBOS",
                table: "Countries");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Country_IsoA3_Uppercase",
                schema: "EBOS",
                table: "Countries");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Country_IsoNum_Digits",
                schema: "EBOS",
                table: "Countries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Address",
                schema: "CRM",
                table: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Address_City_State",
                schema: "CRM",
                table: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Address_Country_City",
                schema: "CRM",
                table: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Address_Unique_Primary",
                schema: "CRM",
                table: "Address");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Address_GoogleMapsUrl_Valid",
                schema: "CRM",
                table: "Address");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Address_IsPrimary_Boolean",
                schema: "CRM",
                table: "Address");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Address_Latitude_Range",
                schema: "CRM",
                table: "Address");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Address_Longitude_Range",
                schema: "CRM",
                table: "Address");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Address_PostalCode_Length",
                schema: "CRM",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "IsPrimary",
                schema: "CRM",
                table: "Address");

            migrationBuilder.RenameTable(
                name: "Address",
                schema: "CRM",
                newName: "Addresses",
                newSchema: "CRM");

            migrationBuilder.RenameColumn(
                name: "PrimaryAddressId",
                schema: "CRM",
                table: "Customers",
                newName: "AddressId");

            migrationBuilder.RenameColumn(
                name: "CustomerType",
                schema: "CRM",
                table: "Customers",
                newName: "TipoCliente");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_PrimaryAddressId",
                schema: "CRM",
                table: "Customers",
                newName: "IX_Customers_AddressId");

            migrationBuilder.AlterColumn<string>(
                name: "Neighborhood",
                schema: "CRM",
                table: "Addresses",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GoogleMapsUrl",
                schema: "CRM",
                table: "Addresses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BetweenStreet2",
                schema: "CRM",
                table: "Addresses",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BetweenStreet1",
                schema: "CRM",
                table: "Addresses",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addresses",
                schema: "CRM",
                table: "Addresses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_AddressTypes_AddressTypeId",
                schema: "CRM",
                table: "Addresses",
                column: "AddressTypeId",
                principalSchema: "EBOS",
                principalTable: "AddressTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Countries_CountryId",
                schema: "CRM",
                table: "Addresses",
                column: "CountryId",
                principalSchema: "EBOS",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Customers_CustomerId",
                schema: "CRM",
                table: "Addresses",
                column: "CustomerId",
                principalSchema: "CRM",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_Customers_Addresses_AddressId",
                schema: "CRM",
                table: "Customers",
                column: "AddressId",
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
