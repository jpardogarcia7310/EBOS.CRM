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
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Iso31661A2Code = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Iso31661A3Code = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Iso31661NumCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Domain = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_Countries_CurrencyCode",
                table: "Countries",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Domain",
                table: "Countries",
                column: "Domain");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Iso31661A2Code",
                table: "Countries",
                column: "Iso31661A2Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Iso31661A3Code",
                table: "Countries",
                column: "Iso31661A3Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Iso31661NumCode",
                table: "Countries",
                column: "Iso31661NumCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Name",
                table: "Countries",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
