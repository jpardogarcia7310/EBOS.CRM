using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.EBOS;

namespace EBOS.CRM.ConcurrencyTests.Infrastructure;

public static class ConcurrencyEndpoints
{
    public record Endpoint(string Name, string Route, bool ReadOnly);

    public static readonly Endpoint[] All =
    [
        // EBOS (read-only)
        new("AddressType", "AddressType", true),
        new("Country", "Country", true),
        new("IdentificationType", "IdentificationType", true),
        new("Status", "Status", true),
        new("TenantConfiguration", "TenantConfiguration", true),
        new("TenantQuota", "TenantQuota", true),
        new("TenantUsageMetric", "TenantUsageMetric", true),

        // CRM (CRUD)
        new("Address", "Address", false),
        new("BankInformation", "BankInformation", false),
        new("BranchOffice", "BranchOffice", false),
        new("BranchOfficeAddress", "BranchOfficeAddress", false),
        new("CorporateCustomer", "CorporateCustomer", false),
        new("CreditAccount", "CreditAccount", false),
        new("CreditTransaction", "CreditTransaction", false),
        new("Customer", "Customer", false),
        new("CustomerAddress", "CustomerAddress", false),
        new("IndividualCustomer", "IndividualCustomer", false),
        new("Lead", "Lead", false),
        new("Opportunity", "Opportunity", false),
        new("OpportunityStage", "OpportunityStage", false),
        new("Quote", "Quote", false),
        new("TaxInformation", "TaxInformation", false),
        new("TaxInformationAddress", "TaxInformationAddress", false)
    ];

    public static async Task<long> GetFirstIdAsync(HttpClient client, string version, string route)
    {
        var url = $"/api/v{version}/{route}";
        return route switch
        {
            "AddressType" => await ControllerTestHelper.GetFirstIdAsync<AddressTypeResponse>(client, url, x => x.Id),
            "Country" => await ControllerTestHelper.GetFirstIdAsync<CountryResponse>(client, url, x => x.Id),
            "IdentificationType" => await ControllerTestHelper.GetFirstIdAsync<IdentificationTypeResponse>(client, url, x => x.Id),
            "Status" => await ControllerTestHelper.GetFirstIdAsync<StatusResponse>(client, url, x => x.Id),
            "TenantConfiguration" => await ControllerTestHelper.GetFirstIdAsync<TenantConfigurationResponse>(client, url, x => x.Id),
            "TenantQuota" => await ControllerTestHelper.GetFirstIdAsync<TenantQuotaResponse>(client, url, x => x.Id),
            "TenantUsageMetric" => await ControllerTestHelper.GetFirstIdAsync<TenantUsageMetricResponse>(client, url, x => x.Id),
            "Address" => await ControllerTestHelper.GetFirstIdAsync<AddressResponse>(client, url, x => x.Id),
            "BankInformation" => await ControllerTestHelper.GetFirstIdAsync<BankInformationResponse>(client, url, x => x.Id),
            "BranchOffice" => await ControllerTestHelper.GetFirstIdAsync<BranchOfficeResponse>(client, url, x => x.Id),
            "BranchOfficeAddress" => await ControllerTestHelper.GetFirstIdAsync<BranchOfficeAddressResponse>(client, url, x => x.Id),
            "CorporateCustomer" => await ControllerTestHelper.GetFirstIdAsync<CorporateCustomerResponse>(client, url, x => x.Id),
            "CreditAccount" => await ControllerTestHelper.GetFirstIdAsync<CreditAccountResponse>(client, url, x => x.Id),
            "CreditTransaction" => await ControllerTestHelper.GetFirstIdAsync<CreditTransactionResponse>(client, url, x => x.Id),
            "Customer" => await ControllerTestHelper.GetFirstIdAsync<CustomerResponse>(client, url, x => x.Id),
            "CustomerAddress" => await ControllerTestHelper.GetFirstIdAsync<CustomerAddressResponse>(client, url, x => x.Id),
            "IndividualCustomer" => await ControllerTestHelper.GetFirstIdAsync<IndividualCustomerResponse>(client, url, x => x.Id),
            "Lead" => await ControllerTestHelper.GetFirstIdAsync<LeadResponse>(client, url, x => x.Id),
            "Opportunity" => await ControllerTestHelper.GetFirstIdAsync<OpportunityResponse>(client, url, x => x.Id),
            "OpportunityStage" => await ControllerTestHelper.GetFirstIdAsync<OpportunityStageResponse>(client, url, x => x.Id),
            "Quote" => await ControllerTestHelper.GetFirstIdAsync<QuoteResponse>(client, url, x => x.Id),
            "TaxInformation" => await ControllerTestHelper.GetFirstIdAsync<TaxInformationResponse>(client, url, x => x.Id),
            "TaxInformationAddress" => await ControllerTestHelper.GetFirstIdAsync<TaxInformationAddressResponse>(client, url, x => x.Id),
            _ => throw new InvalidOperationException($"No response type mapping for route {route}.")
        };
    }
}
