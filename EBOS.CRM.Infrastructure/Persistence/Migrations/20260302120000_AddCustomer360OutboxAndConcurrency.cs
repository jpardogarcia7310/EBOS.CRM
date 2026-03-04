using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations;

public partial class AddCustomer360OutboxAndConcurrency : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<byte[]>(
            name: "RowVersion",
            schema: "CRM",
            table: "CustomerPreferences",
            type: "rowversion",
            rowVersion: true,
            nullable: false);

        migrationBuilder.AddColumn<byte[]>(
            name: "RowVersion",
            schema: "CRM",
            table: "CustomerConsents",
            type: "rowversion",
            rowVersion: true,
            nullable: false);

        migrationBuilder.AddColumn<byte[]>(
            name: "RowVersion",
            schema: "CRM",
            table: "AccountHierarchies",
            type: "rowversion",
            rowVersion: true,
            nullable: false);

        migrationBuilder.AddColumn<byte[]>(
            name: "RowVersion",
            schema: "CRM",
            table: "AccountContactRoles",
            type: "rowversion",
            rowVersion: true,
            nullable: false);

        migrationBuilder.AddColumn<byte[]>(
            name: "RowVersion",
            schema: "CRM",
            table: "AccountContacts",
            type: "rowversion",
            rowVersion: true,
            nullable: false);

        migrationBuilder.CreateTable(
            name: "AuditOutboxMessages",
            schema: "EBOS",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Operation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                NextAttemptAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                AttemptCount = table.Column<int>(type: "int", nullable: false),
                LastError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AuditOutboxMessages", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_AuditOutbox_Pending",
            schema: "EBOS",
            table: "AuditOutboxMessages",
            columns: new[] { "ProcessedAt", "NextAttemptAt" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AuditOutboxMessages",
            schema: "EBOS");

        migrationBuilder.DropColumn(
            name: "RowVersion",
            schema: "CRM",
            table: "CustomerPreferences");

        migrationBuilder.DropColumn(
            name: "RowVersion",
            schema: "CRM",
            table: "CustomerConsents");

        migrationBuilder.DropColumn(
            name: "RowVersion",
            schema: "CRM",
            table: "AccountHierarchies");

        migrationBuilder.DropColumn(
            name: "RowVersion",
            schema: "CRM",
            table: "AccountContactRoles");

        migrationBuilder.DropColumn(
            name: "RowVersion",
            schema: "CRM",
            table: "AccountContacts");
    }
}
