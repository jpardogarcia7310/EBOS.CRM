using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAbacPrimitives : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbacAttributes",
                schema: "IAM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbacAttributes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PolicyRoles",
                schema: "IAM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolicyRoles_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalSchema: "IAM",
                        principalTable: "Policies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PolicyRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "IAM",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PolicyRules",
                schema: "IAM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Effect = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolicyRules_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalSchema: "IAM",
                        principalTable: "Policies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PolicyRuleConditions",
                schema: "IAM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyRuleId = table.Column<long>(type: "bigint", nullable: false),
                    AttributeId = table.Column<long>(type: "bigint", nullable: false),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ValueType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsNegated = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Erased = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyRuleConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolicyRuleConditions_AbacAttributes_AttributeId",
                        column: x => x.AttributeId,
                        principalSchema: "IAM",
                        principalTable: "AbacAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PolicyRuleConditions_PolicyRules_PolicyRuleId",
                        column: x => x.PolicyRuleId,
                        principalSchema: "IAM",
                        principalTable: "PolicyRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbacAttribute_Category_Code",
                schema: "IAM",
                table: "AbacAttributes",
                columns: new[] { "Category", "Code" });

            migrationBuilder.CreateIndex(
                name: "UX_AbacAttribute_Code",
                schema: "IAM",
                table: "AbacAttributes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PolicyRoles_RoleId",
                schema: "IAM",
                table: "PolicyRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "UX_PolicyRole_Policy_Role",
                schema: "IAM",
                table: "PolicyRoles",
                columns: new[] { "PolicyId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PolicyRuleCondition_Rule_Attribute",
                schema: "IAM",
                table: "PolicyRuleConditions",
                columns: new[] { "PolicyRuleId", "AttributeId" });

            migrationBuilder.CreateIndex(
                name: "IX_PolicyRuleConditions_AttributeId",
                schema: "IAM",
                table: "PolicyRuleConditions",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyRule_Policy_Priority",
                schema: "IAM",
                table: "PolicyRules",
                columns: new[] { "PolicyId", "Priority" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PolicyRoles",
                schema: "IAM");

            migrationBuilder.DropTable(
                name: "PolicyRuleConditions",
                schema: "IAM");

            migrationBuilder.DropTable(
                name: "AbacAttributes",
                schema: "IAM");

            migrationBuilder.DropTable(
                name: "PolicyRules",
                schema: "IAM");
        }
    }
}
