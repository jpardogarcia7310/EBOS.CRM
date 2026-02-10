using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;

namespace EBOS.CRM.ApiTests.Infrastructure.Persistence;

public class CrmEntityConfigurationTests
{
    private static CrmDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;
        return new CrmDbContext(options);
    }

    [Fact]
    public void Address_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<Address>(context);

        AssertTable(entity, "Addresses", "CRM");
        AssertProperty(entity, "Street", required: true, maxLength: 200);
        AssertProperty(entity, "ExternalNumber", required: true, maxLength: 20);
        AssertProperty(entity, "InternalNumber", required: false, maxLength: 20);
        AssertProperty(entity, "BetweenStreet1", required: false, maxLength: 200);
        AssertProperty(entity, "BetweenStreet2", required: false, maxLength: 200);
        AssertProperty(entity, "Neighbourhood", required: false, maxLength: 200);
        AssertProperty(entity, "City", required: true, maxLength: 150);
        AssertProperty(entity, "StateOrProvince", required: true, maxLength: 150);
        AssertProperty(entity, "PostalCode", required: true, maxLength: 20);
        AssertProperty(entity, "GoogleMapsUrl", required: false, maxLength: 500);
        AssertPrecision(entity, "Latitude", 10, 6);
        AssertPrecision(entity, "Longitude", 10, 6);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "IX_Address_City_State", "IX_Address_Country_City", "IX_Address_TenantId");
        AssertCheckConstraints(entity, "CK_Address_Latitude_Range", "CK_Address_Longitude_Range",
            "CK_Address_PostalCode_Length", "CK_Address_GoogleMapsUrl_Valid");
    }

    [Fact]
    public void Customer_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<Customer>(context);

        AssertTable(entity, "Customers", "CRM");
        AssertProperty(entity, "Code", required: true, maxLength: 50);
        AssertProperty(entity, "Email", required: true, maxLength: 100);
        AssertProperty(entity, "Phone", required: true, maxLength: 12);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "IX_Customer_Status_CreatedAt", "IX_Customer_TenantId", "UX_Customer_TenantId_Code");
        AssertCheckConstraints(entity, "CK_Customer_Email_Valid", "CK_Customer_Phone_Digits");
    }

    [Fact]
    public void CorporateCustomer_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<CorporateCustomer>(context);

        AssertProperty(entity, "LegalName", required: true, maxLength: 200);
        AssertProperty(entity, "TaxIdentification", required: true, maxLength: 20);
        AssertProperty(entity, "Erased", required: true);

        AssertCheckConstraints(entity, "CK_CorporateCustomer_TaxId_Valid");
    }

    [Fact]
    public void IndividualCustomer_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<IndividualCustomer>(context);

        AssertProperty(entity, "FirstName", required: true, maxLength: 50);
        AssertProperty(entity, "LastName", required: true, maxLength: 100);
        AssertProperty(entity, "IdentificationNumber", required: true, maxLength: 10);
        AssertProperty(entity, "BirthDate", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "IX_IndividualCustomer_IdentificationTypeId");
    }

    [Fact]
    public void CustomerAddress_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<CustomerAddress>(context);

        AssertTable(entity, "CustomerAddresses", "CRM");
        AssertProperty(entity, "IsPrimary", required: true);
        AssertProperty(entity, "ValidFrom", required: true);
        AssertProperty(entity, "IsCurrent", required: true);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "IX_CustomerAddress_Current_Primary", "IX_CustomerAddress_TenantId");
        AssertCheckConstraints(entity, "CK_CustomerAddress_ValidFrom_NotNull");
    }

    [Fact]
    public void BranchOffice_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<BranchOffice>(context);

        AssertTable(entity, "BranchOffices", "CRM");
        AssertProperty(entity, "Name", required: true, maxLength: 200);
        AssertProperty(entity, "PhoneNumber", required: true, maxLength: 20);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "IX_BranchOffice_CorporateCustomerId", "IX_BranchOffice_TenantId");
    }

    [Fact]
    public void BranchOfficeAddress_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<BranchOfficeAddress>(context);

        AssertTable(entity, "BranchOfficeAddresses", "CRM");
        AssertProperty(entity, "IsPrimary", required: true);
        AssertProperty(entity, "ValidFrom", required: true);
        AssertProperty(entity, "IsCurrent", required: true);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "IX_BranchOfficeAddress_Current_Primary", "IX_BranchOfficeAddress_TenantId");
    }

    [Fact]
    public void BankInformation_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<BankInformation>(context);

        AssertTable(entity, "BankInformation", "CRM");
        AssertProperty(entity, "Iban", required: true, maxLength: 34);
        AssertProperty(entity, "Bic", required: false, maxLength: 11);
        AssertProperty(entity, "BankName", required: false, maxLength: 200);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "IX_BankInformation_TenantId");
    }

    [Fact]
    public void TaxInformation_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<TaxInformation>(context);

        AssertTable(entity, "TaxInformation", "CRM");
        AssertProperty(entity, "TaxName", required: true, maxLength: 200);
        AssertProperty(entity, "TaxIdentificationNumber", required: true, maxLength: 20);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "IX_TaxInformation_TenantId");
        AssertCheckConstraints(entity, "CK_TaxInformation_TIN_Valid");
    }

    [Fact]
    public void TaxInformationAddress_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<TaxInformationAddress>(context);

        AssertTable(entity, "TaxInformationAddresses", "CRM");
        AssertProperty(entity, "IsPrimary", required: true);
        AssertProperty(entity, "ValidFrom", required: true);
        AssertProperty(entity, "IsCurrent", required: true);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "IX_TaxInformationAddress_Current_Primary", "IX_TaxInformationAddress_TenantId");
    }

    [Fact]
    public void Lead_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<Lead>(context);

        AssertTable(entity, "Leads", "CRM");
        AssertProperty(entity, "Source", required: true, maxLength: 100);
        AssertProperty(entity, "Status", required: true, maxLength: 50);
        AssertProperty(entity, "OwnerUserId", required: true);
        AssertProperty(entity, "CompanyName", required: true, maxLength: 200);
        AssertProperty(entity, "ContactName", required: true, maxLength: 150);
        AssertProperty(entity, "Email", required: true, maxLength: 100);
        AssertProperty(entity, "Phone", required: true, maxLength: 20);
        AssertPrecision(entity, "EstimatedValue", 18, 2);
        AssertProperty(entity, "Notes", required: false, maxLength: 2000);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "IX_Lead_TenantId", "IX_Lead_Status_CreatedAt");
        AssertCheckConstraints(entity, "CK_Lead_Email_Valid", "CK_Lead_Phone_Digits",
            "CK_Lead_EstimatedValue_NonNegative");
    }

    [Fact]
    public void Opportunity_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<Opportunity>(context);

        AssertTable(entity, "Opportunities", "CRM");
        AssertProperty(entity, "Name", required: true, maxLength: 200);
        AssertProperty(entity, "OwnerUserId", required: true);
        AssertPrecision(entity, "Amount", 18, 2, required: true);
        AssertPrecision(entity, "Probability", 5, 4, required: true);
        AssertProperty(entity, "Source", required: false, maxLength: 100);
        AssertProperty(entity, "CloseReason", required: false, maxLength: 500);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "IX_Opportunity_TenantId", "IX_Opportunity_Stage_CloseDate",
            "IX_Opportunity_Owner_Stage");
        AssertCheckConstraints(entity, "CK_Opportunity_Amount_NonNegative", "CK_Opportunity_Probability_Range");
    }

    [Fact]
    public void OpportunityStage_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<OpportunityStage>(context);

        AssertTable(entity, "OpportunityStages", "CRM");
        AssertProperty(entity, "Name", required: true, maxLength: 100);
        AssertProperty(entity, "Order", required: true);
        AssertPrecision(entity, "DefaultProbability", 5, 4, required: true);
        AssertProperty(entity, "IsClosed", required: true);
        AssertProperty(entity, "IsWon", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "IX_OpportunityStage_TenantId", "IX_OpportunityStage_TenantId_Order",
            "UX_OpportunityStage_TenantId_Name");
        AssertCheckConstraints(entity, "CK_OpportunityStage_DefaultProbability_Range");
    }

    [Fact]
    public void Quote_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<Quote>(context);

        AssertTable(entity, "Quotes", "CRM");
        AssertProperty(entity, "Status", required: true, maxLength: 50);
        AssertProperty(entity, "ReferenceNumber", required: false, maxLength: 50);
        AssertPrecision(entity, "SubtotalAmount", 18, 2, required: true);
        AssertPrecision(entity, "DiscountAmount", 18, 2, required: true);
        AssertPrecision(entity, "TotalAmount", 18, 2, required: true);
        AssertProperty(entity, "Notes", required: false, maxLength: 2000);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "IX_Quote_TenantId", "IX_Quote_Opportunity_Status", "UX_Quote_TenantId_ReferenceNumber");
        AssertCheckConstraints(entity, "CK_Quote_Subtotal_NonNegative", "CK_Quote_Discount_NonNegative",
            "CK_Quote_Discount_Lte_Subtotal", "CK_Quote_Total_NonNegative");
    }

    [Fact]
    public void CreditAccount_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<CreditAccount>(context);

        AssertTable(entity, "CreditAccounts", "CRM");
        AssertPrecision(entity, "MaxAmount", 18, 2, required: true);
        AssertPrecision(entity, "UsedAmount", 18, 2, required: true);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "IX_CreditAccount_CustomerId", "IX_CreditAccount_TenantId");
        AssertCheckConstraints(entity, "CK_CreditAccount_MaxAmount_Positive",
            "CK_CreditAccount_UsedAmount_NonNegative", "CK_CreditAccount_UsedAmount_WithinLimit");
    }

    [Fact]
    public void CreditTransaction_Config_Is_Correct()
    {
        using var context = CreateContext();
        var entity = GetEntityType<CreditTransaction>(context);

        AssertTable(entity, "CreditTransactions", "CRM");
        AssertProperty(entity, "Date", required: true);
        AssertPrecision(entity, "Amount", 18, 2, required: true);
        AssertProperty(entity, "Type", required: true, maxLength: 50);
        AssertProperty(entity, "ExternalReference", required: true, maxLength: 200);
        AssertProperty(entity, "Comments", required: true, maxLength: 500);
        AssertProperty(entity, "CreatedAt", required: true);
        AssertProperty(entity, "CreatedBy", required: true);
        AssertProperty(entity, "Erased", required: true);

        AssertIndexes(entity, "IX_CreditTransaction_Date_Account", "IX_CreditTransaction_Account_Date",
            "IX_CreditTransaction_TenantId", "IX_CreditTransactions_CreditAccountId");
        AssertCheckConstraints(entity, "CK_CreditTransaction_Amount_NotZero", "CK_CreditTransaction_Type_Valid");
    }

    private static IEntityType GetEntityType<T>(DbContext context)
    {
        var model = context.GetService<IDesignTimeModel>().Model;
        return model.FindEntityType(typeof(T)) ?? throw new InvalidOperationException($"Missing entity {typeof(T).Name}");
    }

    private static void AssertTable(IEntityType entityType, string table, string schema)
    {
        Assert.Equal(table, entityType.GetTableName());
        Assert.Equal(schema, entityType.GetSchema());
    }

    private static void AssertProperty(IEntityType entityType, string name, bool required, int? maxLength = null)
    {
        var property = entityType.FindProperty(name) ?? throw new InvalidOperationException($"Missing property {name}");
        Assert.Equal(!required, property.IsNullable);
        if (maxLength.HasValue)
        {
            Assert.Equal(maxLength.Value, property.GetMaxLength());
        }
    }

    private static void AssertPrecision(IEntityType entityType, string name, int precision, int scale, bool required = false)
    {
        var property = entityType.FindProperty(name) ?? throw new InvalidOperationException($"Missing property {name}");
        if (required)
        {
            Assert.False(property.IsNullable);
        }
        Assert.Equal(precision, property.GetPrecision());
        Assert.Equal(scale, property.GetScale());
    }

    private static void AssertIndexes(IEntityType entityType, params string[] names)
    {
        var indexNames = entityType.GetIndexes()
            .Select(i => i.GetDatabaseName())
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var name in names)
        {
            Assert.Contains(name, indexNames);
        }
    }

    private static void AssertCheckConstraints(IEntityType entityType, params string[] names)
    {
        var constraintNames = entityType.GetCheckConstraints()
            .Select(c => c.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var name in names)
        {
            Assert.Contains(name, constraintNames);
        }
    }
}
