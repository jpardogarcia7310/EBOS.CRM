using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBOS.CRM.Infrastructure.Persistence.Migrations;

public partial class HardenCustomer360Infrastructure : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "UX_AccountContact_Tenant_Corporate_Individual_Active",
            schema: "CRM",
            table: "AccountContacts",
            columns: new[] { "TenantId", "CorporateCustomerId", "IndividualCustomerId" },
            unique: true,
            filter: "[Erased] = 0");

        migrationBuilder.CreateIndex(
            name: "UX_AccountContact_Tenant_Corporate_Primary_Active",
            schema: "CRM",
            table: "AccountContacts",
            columns: new[] { "TenantId", "CorporateCustomerId" },
            unique: true,
            filter: "[IsPrimary] = 1 AND [Erased] = 0");

        migrationBuilder.CreateIndex(
            name: "UX_AccountContactRole_Tenant_AccountContact_Role_Active",
            schema: "CRM",
            table: "AccountContactRoles",
            columns: new[] { "TenantId", "AccountContactId", "RoleCode" },
            unique: true,
            filter: "[Erased] = 0");

        migrationBuilder.CreateIndex(
            name: "UX_AccountContactRole_Tenant_AccountContact_Primary_Active",
            schema: "CRM",
            table: "AccountContactRoles",
            columns: new[] { "TenantId", "AccountContactId" },
            unique: true,
            filter: "[IsPrimary] = 1 AND [Erased] = 0");

        migrationBuilder.AddCheckConstraint(
            name: "CK_AccountHierarchy_Parent_Child_Different",
            schema: "CRM",
            table: "AccountHierarchies",
            sql: "[ParentCorporateCustomerId] <> [ChildCorporateCustomerId]");

        migrationBuilder.CreateIndex(
            name: "UX_AccountHierarchy_Tenant_Parent_Child_Relation_Current",
            schema: "CRM",
            table: "AccountHierarchies",
            columns: new[] { "TenantId", "ParentCorporateCustomerId", "ChildCorporateCustomerId", "RelationType" },
            unique: true,
            filter: "[IsCurrent] = 1 AND [Erased] = 0");

        migrationBuilder.DropIndex(
            name: "UX_CustomerPreference_TenantId_Customer_Channel",
            schema: "CRM",
            table: "CustomerPreferences");

        migrationBuilder.CreateIndex(
            name: "UX_CustomerPreference_TenantId_Customer_Channel",
            schema: "CRM",
            table: "CustomerPreferences",
            columns: new[] { "TenantId", "CustomerId", "ChannelId" },
            unique: true,
            filter: "[Erased] = 0");

        migrationBuilder.CreateIndex(
            name: "IX_CustomerConsent_Tenant_Customer_ConsentType_GrantedAt",
            schema: "CRM",
            table: "CustomerConsents",
            columns: new[] { "TenantId", "CustomerId", "ConsentType", "GrantedAt" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "UX_AccountContact_Tenant_Corporate_Individual_Active",
            schema: "CRM",
            table: "AccountContacts");

        migrationBuilder.DropIndex(
            name: "UX_AccountContact_Tenant_Corporate_Primary_Active",
            schema: "CRM",
            table: "AccountContacts");

        migrationBuilder.DropIndex(
            name: "UX_AccountContactRole_Tenant_AccountContact_Role_Active",
            schema: "CRM",
            table: "AccountContactRoles");

        migrationBuilder.DropIndex(
            name: "UX_AccountContactRole_Tenant_AccountContact_Primary_Active",
            schema: "CRM",
            table: "AccountContactRoles");

        migrationBuilder.DropCheckConstraint(
            name: "CK_AccountHierarchy_Parent_Child_Different",
            schema: "CRM",
            table: "AccountHierarchies");

        migrationBuilder.DropIndex(
            name: "UX_AccountHierarchy_Tenant_Parent_Child_Relation_Current",
            schema: "CRM",
            table: "AccountHierarchies");

        migrationBuilder.DropIndex(
            name: "IX_CustomerConsent_Tenant_Customer_ConsentType_GrantedAt",
            schema: "CRM",
            table: "CustomerConsents");

        migrationBuilder.DropIndex(
            name: "UX_CustomerPreference_TenantId_Customer_Channel",
            schema: "CRM",
            table: "CustomerPreferences");

        migrationBuilder.CreateIndex(
            name: "UX_CustomerPreference_TenantId_Customer_Channel",
            schema: "CRM",
            table: "CustomerPreferences",
            columns: new[] { "TenantId", "CustomerId", "ChannelId" },
            unique: true);
    }
}
