using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFieldsToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "IAM",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "IAM",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "IAM",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "IAM",
                table: "UserRoles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "IAM",
                table: "UserRoles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "IAM",
                table: "UserRoles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "IAM",
                table: "UserRoles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "IAM",
                table: "UserPolicies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "IAM",
                table: "UserPolicies",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "IAM",
                table: "UserPolicies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "IAM",
                table: "UserPolicies",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "CRM",
                table: "TaxInformationAddresses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "CRM",
                table: "TaxInformationAddresses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "CRM",
                table: "TaxInformationAddresses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "CRM",
                table: "TaxInformationAddresses",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "CRM",
                table: "TaxInformation",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "CRM",
                table: "TaxInformation",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "CRM",
                table: "TaxInformation",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "CRM",
                table: "TaxInformation",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "EBOS",
                table: "Statuses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "EBOS",
                table: "Statuses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "EBOS",
                table: "Statuses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "EBOS",
                table: "Statuses",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "IAM",
                table: "Roles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "IAM",
                table: "Roles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "IAM",
                table: "Roles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "IAM",
                table: "RolePermissions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "IAM",
                table: "RolePermissions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "IAM",
                table: "RolePermissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "IAM",
                table: "RolePermissions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "IAM",
                table: "PolicyPermissions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "IAM",
                table: "PolicyPermissions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "IAM",
                table: "PolicyPermissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "IAM",
                table: "PolicyPermissions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "IAM",
                table: "Policies",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "IAM",
                table: "Policies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "IAM",
                table: "Policies",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "IAM",
                table: "Permissions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "IAM",
                table: "Permissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "IAM",
                table: "Permissions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "EBOS",
                table: "IdentificationTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "EBOS",
                table: "IdentificationTypes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "EBOS",
                table: "IdentificationTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "EBOS",
                table: "IdentificationTypes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "CRM",
                table: "Customers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "CRM",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "CRM",
                table: "Customers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "CRM",
                table: "CustomerAddresses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "CRM",
                table: "CustomerAddresses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "CRM",
                table: "CustomerAddresses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "CRM",
                table: "CustomerAddresses",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "CRM",
                table: "CreditTransactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "CRM",
                table: "CreditTransactions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "CRM",
                table: "CreditTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "CRM",
                table: "CreditTransactions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "CRM",
                table: "CreditAccounts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "CRM",
                table: "CreditAccounts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "CRM",
                table: "CreditAccounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "CRM",
                table: "CreditAccounts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "EBOS",
                table: "Countries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "EBOS",
                table: "Countries",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "EBOS",
                table: "Countries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "EBOS",
                table: "Countries",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "CRM",
                table: "BranchOffices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "CRM",
                table: "BranchOffices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "CRM",
                table: "BranchOffices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "CRM",
                table: "BranchOffices",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "CRM",
                table: "BranchOfficeAddresses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "CRM",
                table: "BranchOfficeAddresses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "CRM",
                table: "BranchOfficeAddresses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "CRM",
                table: "BranchOfficeAddresses",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "CRM",
                table: "BankInformation",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "CRM",
                table: "BankInformation",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "CRM",
                table: "BankInformation",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "CRM",
                table: "BankInformation",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "EBOS",
                table: "AddressTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "EBOS",
                table: "AddressTypes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "EBOS",
                table: "AddressTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "EBOS",
                table: "AddressTypes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "CRM",
                table: "Addresses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "CRM",
                table: "Addresses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "CRM",
                table: "Addresses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                schema: "CRM",
                table: "Addresses",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "IAM",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "IAM",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "IAM",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "IAM",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "IAM",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "IAM",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "IAM",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "IAM",
                table: "UserPolicies");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "IAM",
                table: "UserPolicies");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "IAM",
                table: "UserPolicies");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "IAM",
                table: "UserPolicies");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "CRM",
                table: "TaxInformationAddresses");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "CRM",
                table: "TaxInformationAddresses");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "CRM",
                table: "TaxInformationAddresses");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "CRM",
                table: "TaxInformationAddresses");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "CRM",
                table: "TaxInformation");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "CRM",
                table: "TaxInformation");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "CRM",
                table: "TaxInformation");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "CRM",
                table: "TaxInformation");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "EBOS",
                table: "Statuses");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "EBOS",
                table: "Statuses");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "EBOS",
                table: "Statuses");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "EBOS",
                table: "Statuses");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "IAM",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "IAM",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "IAM",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "IAM",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "IAM",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "IAM",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "IAM",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "IAM",
                table: "PolicyPermissions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "IAM",
                table: "PolicyPermissions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "IAM",
                table: "PolicyPermissions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "IAM",
                table: "PolicyPermissions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "IAM",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "IAM",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "IAM",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "IAM",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "IAM",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "IAM",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "EBOS",
                table: "IdentificationTypes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "EBOS",
                table: "IdentificationTypes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "EBOS",
                table: "IdentificationTypes");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "EBOS",
                table: "IdentificationTypes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "CRM",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "CRM",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "CRM",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "CRM",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "CRM",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "CRM",
                table: "CreditTransactions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "CRM",
                table: "CreditTransactions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "CRM",
                table: "CreditTransactions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "CRM",
                table: "CreditTransactions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "CRM",
                table: "CreditAccounts");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "CRM",
                table: "CreditAccounts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "CRM",
                table: "CreditAccounts");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "CRM",
                table: "CreditAccounts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "EBOS",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "EBOS",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "EBOS",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "EBOS",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "CRM",
                table: "BranchOffices");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "CRM",
                table: "BranchOffices");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "CRM",
                table: "BranchOffices");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "CRM",
                table: "BranchOffices");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "CRM",
                table: "BranchOfficeAddresses");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "CRM",
                table: "BranchOfficeAddresses");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "CRM",
                table: "BranchOfficeAddresses");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "CRM",
                table: "BranchOfficeAddresses");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "CRM",
                table: "BankInformation");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "CRM",
                table: "BankInformation");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "CRM",
                table: "BankInformation");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "CRM",
                table: "BankInformation");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "EBOS",
                table: "AddressTypes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "EBOS",
                table: "AddressTypes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "EBOS",
                table: "AddressTypes");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "EBOS",
                table: "AddressTypes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "CRM",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "CRM",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "CRM",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "CRM",
                table: "Addresses");
        }
    }
}
