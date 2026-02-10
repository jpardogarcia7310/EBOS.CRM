using System.Net.Http.Json;
using System.Text;
using EBOS.CRM.Application.Contracts.Requests.CRM.Address;
using EBOS.CRM.Application.Contracts.Requests.CRM.BankInformation;
using EBOS.CRM.Application.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Application.Contracts.Requests.CRM.BranchOfficeAddress;
using EBOS.CRM.Application.Contracts.Requests.CRM.CorporateCustomer;
using EBOS.CRM.Application.Contracts.Requests.CRM.CreditAccount;
using EBOS.CRM.Application.Contracts.Requests.CRM.CreditTransaction;
using EBOS.CRM.Application.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Application.Contracts.Requests.CRM.CustomerAddress;
using EBOS.CRM.Application.Contracts.Requests.CRM.IndividualCustomer;
using EBOS.CRM.Application.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Application.Contracts.Requests.CRM.Opportunity;
using EBOS.CRM.Application.Contracts.Requests.CRM.OpportunityStage;
using EBOS.CRM.Application.Contracts.Requests.CRM.Quote;
using EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformation;
using EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformationAddress;

namespace EBOS.CRM.StressTests.Infrastructure;

public sealed record StressPayloadFactories(
    Func<HttpContent> Post,
    Func<long, HttpContent> Put,
    Func<long, HttpContent> Patch);

public static class StressPayloads
{
    private const long TenantId = 1;
    private static readonly Dictionary<string, long> IdCache = new(StringComparer.OrdinalIgnoreCase);

    public static async Task<StressPayloadFactories> GetPayloadFactoriesAsync(
        HttpClient client,
        string version,
        string entity)
    {
        switch (entity)
        {
            case "Address":
            {
                var countryId = await GetIdAsync(client, version, "Country");
                var addressTypeId = await GetIdAsync(client, version, "AddressType");

                return new StressPayloadFactories(
                    Post: () => JsonContent.Create(new AddAddressRequest(
                        TenantId,
                        Street: $"Stress St {ShortCode()}",
                        ExternalNumber: "1",
                        InternalNumber: "2B",
                        BetweenStreet1: "Street A",
                        BetweenStreet2: "Street B",
                        Neighbourhood: "Centro",
                        City: "Madrid",
                        StateOrProvince: "Madrid",
                        PostalCode: "28001",
                        GoogleMapsUrl: "https://maps.example.com/stress",
                        Latitude: "40.0",
                        Longitude: "-3.7",
                        CountryId: countryId,
                        AddressTypeId: addressTypeId)),
                    Put: _ => JsonContent.Create(new UpdateAddressRequest(
                        TenantId,
                        Street: $"Stress St {ShortCode()}",
                        ExternalNumber: "2",
                        InternalNumber: "3C",
                        BetweenStreet1: "Street A",
                        BetweenStreet2: "Street B",
                        Neighbourhood: "Centro",
                        City: "Madrid",
                        StateOrProvince: "Madrid",
                        PostalCode: "28002",
                        GoogleMapsUrl: "https://maps.example.com/stress",
                        Latitude: "40.1",
                        Longitude: "-3.6",
                        CountryId: countryId,
                        AddressTypeId: addressTypeId)),
                    Patch: _ => JsonContent.Create(new UpdateAddressRequest(
                        TenantId,
                        Street: $"Stress St {ShortCode()}",
                        ExternalNumber: "3",
                        InternalNumber: "4D",
                        BetweenStreet1: "Street A",
                        BetweenStreet2: "Street B",
                        Neighbourhood: "Centro",
                        City: "Madrid",
                        StateOrProvince: "Madrid",
                        PostalCode: "28003",
                        GoogleMapsUrl: "https://maps.example.com/stress",
                        Latitude: "40.2",
                        Longitude: "-3.5",
                        CountryId: countryId,
                        AddressTypeId: addressTypeId)));
            }
            case "BankInformation":
            {
                var customerId = await GetIdAsync(client, version, "Customer");
                return new StressPayloadFactories(
                    Post: () => JsonContent.Create(new AddBankInformationRequest(
                        TenantId,
                        Iban: GenerateIban(),
                        Bic: "CAIXESBBXXX",
                        BankName: "Stress Bank",
                        CustomerId: customerId)),
                    Put: _ => JsonContent.Create(new UpdateBankInformationRequest(
                        TenantId,
                        Iban: GenerateIban(),
                        Bic: "CAIXESBBXXX",
                        BankName: "Stress Bank",
                        CustomerId: customerId)),
                    Patch: _ => JsonContent.Create(new UpdateBankInformationRequest(
                        TenantId,
                        Iban: GenerateIban(),
                        Bic: "CAIXESBBXXX",
                        BankName: "Stress Bank",
                        CustomerId: customerId)));
            }
            case "BranchOffice":
            {
                var corporateCustomerId = await GetIdAsync(client, version, "CorporateCustomer");
                return new StressPayloadFactories(
                    Post: () => JsonContent.Create(new AddBranchOfficeRequest(
                        TenantId,
                        Name: $"Branch {ShortCode()}",
                        PhoneNumber: "+34 911 000 000",
                        CorporateCustomerId: corporateCustomerId)),
                    Put: id => JsonContent.Create(new UpdateBranchOfficeRequest(
                        Id: id,
                        TenantId: TenantId,
                        Name: $"Branch {ShortCode()}",
                        PhoneNumber: "+34 911 000 001",
                        CorporateCustomerId: corporateCustomerId)),
                    Patch: _ => JsonContent.Create(new PatchBranchOfficeRequest(
                        TenantId,
                        Name: $"Branch {ShortCode()}",
                        PhoneNumber: "+34 911 000 002",
                        CorporateCustomerId: corporateCustomerId)));
            }
            case "BranchOfficeAddress":
            {
                var branchOfficeId = await GetIdAsync(client, version, "BranchOffice");
                var addressId = await GetIdAsync(client, version, "Address");
                return new StressPayloadFactories(
                    Post: () => JsonContent.Create(new AddBranchOfficeAddressRequest(
                        TenantId,
                        BranchOfficeId: branchOfficeId,
                        AddressId: addressId,
                        IsPrimary: true,
                        ValidFrom: DateTime.UtcNow.AddDays(-1),
                        ValidTo: null,
                        IsCurrent: true)),
                    Put: _ => JsonContent.Create(new UpdateBranchOfficeAddressRequest(
                        TenantId,
                        BranchOfficeId: branchOfficeId,
                        AddressId: addressId,
                        IsPrimary: true,
                        ValidFrom: DateTime.UtcNow.AddDays(-2),
                        ValidTo: null,
                        IsCurrent: true)),
                    Patch: _ => JsonContent.Create(new UpdateBranchOfficeAddressRequest(
                        TenantId,
                        BranchOfficeId: branchOfficeId,
                        AddressId: addressId,
                        IsPrimary: true,
                        ValidFrom: DateTime.UtcNow.AddDays(-3),
                        ValidTo: null,
                        IsCurrent: true)));
            }
            case "CorporateCustomer":
            {
                var statusId = await GetIdAsync(client, version, "Status");
                return new StressPayloadFactories(
                    Post: () => JsonContent.Create(new AddCorporateCustomerRequest(
                        TenantId,
                        Code: ShortCode(),
                        Email: $"{ShortCode()}@example.com",
                        Phone: "+34 600 000 000",
                        StatusId: statusId,
                        LegalName: $"Contoso {ShortCode()}",
                        TaxIdentification: $"B{RandomDigits(8)}")),
                    Put: _ => JsonContent.Create(new UpdateCorporateCustomerRequest(
                        TenantId,
                        Code: ShortCode(),
                        Email: $"{ShortCode()}@example.com",
                        Phone: "+34 600 000 001",
                        StatusId: statusId,
                        LegalName: $"Contoso {ShortCode()}",
                        TaxIdentification: $"B{RandomDigits(8)}")),
                    Patch: _ => JsonContent.Create(new UpdateCorporateCustomerRequest(
                        TenantId,
                        Code: ShortCode(),
                        Email: $"{ShortCode()}@example.com",
                        Phone: "+34 600 000 002",
                        StatusId: statusId,
                        LegalName: $"Contoso {ShortCode()}",
                        TaxIdentification: $"B{RandomDigits(8)}")));
            }
            case "CreditAccount":
            {
                var customerId = await GetIdAsync(client, version, "Customer");
                return new StressPayloadFactories(
                    Post: () => JsonContent.Create(new AddCreditAccountRequest(
                        TenantId,
                        MaxAmount: 10000m,
                        UsedAmount: 100m,
                        CustomerId: customerId)),
                    Put: id => JsonContent.Create(new UpdateCreditAccountRequest(
                        Id: id,
                        TenantId: TenantId,
                        MaxAmount: 15000m,
                        UsedAmount: 200m,
                        CustomerId: customerId)),
                    Patch: _ => JsonContent.Create(new PatchCreditAccountRequest(
                        TenantId,
                        MaxAmount: 20000m,
                        UsedAmount: 300m,
                        CustomerId: customerId)));
            }
            case "CreditTransaction":
            {
                var creditAccountId = await GetIdAsync(client, version, "CreditAccount");
                return new StressPayloadFactories(
                    Post: () => JsonContent.Create(new AddCreditTransactionRequest(
                        TenantId,
                        Date: DateTime.UtcNow.AddDays(-1),
                        Amount: 100m,
                        Type: "Consumo",
                        ExternalReference: $"INV-{ShortCode()}",
                        Comments: "Stress test",
                        CreditAccountId: creditAccountId)),
                    Put: _ => JsonContent.Create(new UpdateCreditTransactionRequest(
                        TenantId,
                        Date: DateTime.UtcNow,
                        Amount: 120m,
                        Type: "Ajuste",
                        ExternalReference: $"INV-{ShortCode()}",
                        Comments: "Stress update",
                        CreditAccountId: creditAccountId)),
                    Patch: _ => JsonContent.Create(new UpdateCreditTransactionRequest(
                        TenantId,
                        Date: DateTime.UtcNow,
                        Amount: 130m,
                        Type: "Devolucion",
                        ExternalReference: $"INV-{ShortCode()}",
                        Comments: "Stress patch",
                        CreditAccountId: creditAccountId)));
            }
            case "Customer":
            {
                var statusId = await GetIdAsync(client, version, "Status");
                return new StressPayloadFactories(
                    Post: () => JsonContent.Create(new AddCustomerRequest(
                        TenantId,
                        Code: ShortCode(),
                        Email: $"{ShortCode()}@example.com",
                        Phone: "+34 600 000 010",
                        StatusId: statusId)),
                    Put: id => JsonContent.Create(new UpdateCustomerRequest(
                        Id: id,
                        TenantId: TenantId,
                        Code: ShortCode(),
                        Email: $"{ShortCode()}@example.com",
                        Phone: "+34 600 000 011",
                        StatusId: statusId)),
                    Patch: _ => JsonContent.Create(new PatchCustomerRequest(
                        TenantId,
                        Code: ShortCode(),
                        Email: $"{ShortCode()}@example.com",
                        Phone: "+34 600 000 012",
                        StatusId: statusId)));
            }
            case "CustomerAddress":
            {
                var customerId = await GetIdAsync(client, version, "Customer");
                var addressId = await GetIdAsync(client, version, "Address");
                return new StressPayloadFactories(
                    Post: () => JsonContent.Create(new AddCustomerAddressRequest(
                        TenantId,
                        CustomerId: customerId,
                        AddressId: addressId,
                        IsPrimary: true,
                        ValidFrom: DateTime.UtcNow.AddDays(-1),
                        ValidTo: null,
                        IsCurrent: true)),
                    Put: _ => JsonContent.Create(new UpdateCustomerAddressRequest(
                        TenantId,
                        CustomerId: customerId,
                        AddressId: addressId,
                        IsPrimary: true,
                        ValidFrom: DateTime.UtcNow.AddDays(-2),
                        ValidTo: null,
                        IsCurrent: true)),
                    Patch: _ => JsonContent.Create(new UpdateCustomerAddressRequest(
                        TenantId,
                        CustomerId: customerId,
                        AddressId: addressId,
                        IsPrimary: true,
                        ValidFrom: DateTime.UtcNow.AddDays(-3),
                        ValidTo: null,
                        IsCurrent: true)));
            }
            case "IndividualCustomer":
            {
                var statusId = await GetIdAsync(client, version, "Status");
                var identificationTypeId = await GetIdAsync(client, version, "IdentificationType");
                return new StressPayloadFactories(
                    Post: () => JsonContent.Create(new AddIndividualCustomerRequest(
                        TenantId,
                        Code: ShortCode(),
                        Email: $"{ShortCode()}@example.com",
                        Phone: "+34 600 000 020",
                        StatusId: statusId,
                        FirstName: "Jane",
                        LastName: "Doe",
                        BirthDate: new DateTime(1990, 5, 20),
                        IdentificationNumber: RandomDigits(10),
                        IdentificationTypeId: identificationTypeId)),
                    Put: _ => JsonContent.Create(new UpdateIndividualCustomerRequest(
                        TenantId,
                        Code: ShortCode(),
                        Email: $"{ShortCode()}@example.com",
                        Phone: "+34 600 000 021",
                        StatusId: statusId,
                        FirstName: "Jane",
                        LastName: "Doe",
                        BirthDate: new DateTime(1990, 5, 21),
                        IdentificationNumber: RandomDigits(10),
                        IdentificationTypeId: identificationTypeId)),
                    Patch: _ => JsonContent.Create(new UpdateIndividualCustomerRequest(
                        TenantId,
                        Code: ShortCode(),
                        Email: $"{ShortCode()}@example.com",
                        Phone: "+34 600 000 022",
                        StatusId: statusId,
                        FirstName: "Jane",
                        LastName: "Doe",
                        BirthDate: new DateTime(1990, 5, 22),
                        IdentificationNumber: RandomDigits(10),
                        IdentificationTypeId: identificationTypeId)));
            }
            case "Lead":
            {
                return new StressPayloadFactories(
                    Post: () => JsonContent.Create(new AddLeadRequest(
                        TenantId,
                        Source: "Web",
                        Status: "New",
                        OwnerUserId: 10,
                        CompanyName: $"Acme {ShortCode()}",
                        ContactName: "Jane Doe",
                        Email: $"{ShortCode()}@example.com",
                        Phone: "1234567890",
                        EstimatedValue: 5000m,
                        Notes: "Stress lead")),
                    Put: id => JsonContent.Create(new UpdateLeadRequest(
                        Id: id,
                        TenantId: TenantId,
                        Source: "Referral",
                        Status: "Qualified",
                        OwnerUserId: 10,
                        CompanyName: $"Acme {ShortCode()}",
                        ContactName: "Jane Doe",
                        Email: $"{ShortCode()}@example.com",
                        Phone: "1234567890",
                        EstimatedValue: 7000m,
                        Notes: "Stress update")),
                    Patch: id => JsonContent.Create(new UpdateLeadRequest(
                        Id: id,
                        TenantId: TenantId,
                        Source: "Event",
                        Status: "Working",
                        OwnerUserId: 10,
                        CompanyName: $"Acme {ShortCode()}",
                        ContactName: "Jane Doe",
                        Email: $"{ShortCode()}@example.com",
                        Phone: "1234567890",
                        EstimatedValue: 9000m,
                        Notes: "Stress patch")));
            }
            case "OpportunityStage":
            {
                var order = Random.Shared.Next(10, 1000);
                return new StressPayloadFactories(
                    Post: () => JsonContent.Create(new AddOpportunityStageRequest(
                        TenantId,
                        Name: $"Stage {ShortCode()}",
                        Order: order,
                        DefaultProbability: 0.2m,
                        IsClosed: false,
                        IsWon: false)),
                    Put: id => JsonContent.Create(new UpdateOpportunityStageRequest(
                        Id: id,
                        TenantId: TenantId,
                        Name: $"Stage {ShortCode()}",
                        Order: order + 1,
                        DefaultProbability: 0.3m,
                        IsClosed: false,
                        IsWon: false)),
                    Patch: id => JsonContent.Create(new UpdateOpportunityStageRequest(
                        Id: id,
                        TenantId: TenantId,
                        Name: $"Stage {ShortCode()}",
                        Order: order + 2,
                        DefaultProbability: 0.4m,
                        IsClosed: false,
                        IsWon: false)));
            }
            case "Opportunity":
            {
                var customerId = await GetIdAsync(client, version, "Customer");
                var stageId = await GetIdAsync(client, version, "OpportunityStage");
                return new StressPayloadFactories(
                    Post: () => JsonContent.Create(new AddOpportunityRequest(
                        TenantId,
                        Name: $"Deal {ShortCode()}",
                        StageId: stageId,
                        OwnerUserId: 10,
                        CustomerId: customerId,
                        ExpectedCloseDate: DateTime.UtcNow.AddDays(30),
                        Amount: 10000m,
                        Probability: 0.5m,
                        Source: "Stress",
                        SourceLeadId: null)),
                    Put: id => JsonContent.Create(new UpdateOpportunityRequest(
                        Id: id,
                        TenantId: TenantId,
                        Name: $"Deal {ShortCode()}",
                        StageId: stageId,
                        OwnerUserId: 10,
                        CustomerId: customerId,
                        ExpectedCloseDate: DateTime.UtcNow.AddDays(45),
                        Amount: 12000m,
                        Probability: 0.6m,
                        Source: "Stress",
                        SourceLeadId: null,
                        CloseReason: null)),
                    Patch: id => JsonContent.Create(new UpdateOpportunityRequest(
                        Id: id,
                        TenantId: TenantId,
                        Name: $"Deal {ShortCode()}",
                        StageId: stageId,
                        OwnerUserId: 10,
                        CustomerId: customerId,
                        ExpectedCloseDate: DateTime.UtcNow.AddDays(60),
                        Amount: 15000m,
                        Probability: 0.7m,
                        Source: "Stress",
                        SourceLeadId: null,
                        CloseReason: null)));
            }
            case "Quote":
            {
                var opportunityId = await GetIdAsync(client, version, "Opportunity");
                return new StressPayloadFactories(
                    Post: () => JsonContent.Create(new AddQuoteRequest(
                        TenantId,
                        OpportunityId: opportunityId,
                        Status: "Draft",
                        ReferenceNumber: $"Q-{ShortCode()}",
                        SubtotalAmount: 10000m,
                        DiscountAmount: 0m,
                        TotalAmount: 10000m,
                        ValidUntil: null,
                        Notes: "Stress quote")),
                    Put: id => JsonContent.Create(new UpdateQuoteRequest(
                        Id: id,
                        TenantId: TenantId,
                        OpportunityId: opportunityId,
                        Status: "Sent",
                        ReferenceNumber: $"Q-{ShortCode()}",
                        SubtotalAmount: 12000m,
                        DiscountAmount: 500m,
                        TotalAmount: 11500m,
                        ValidUntil: null,
                        Notes: "Stress update")),
                    Patch: id => JsonContent.Create(new UpdateQuoteRequest(
                        Id: id,
                        TenantId: TenantId,
                        OpportunityId: opportunityId,
                        Status: "Approved",
                        ReferenceNumber: $"Q-{ShortCode()}",
                        SubtotalAmount: 13000m,
                        DiscountAmount: 0m,
                        TotalAmount: 13000m,
                        ValidUntil: null,
                        Notes: "Stress patch")));
            }
            case "TaxInformation":
            {
                var customerId = await GetIdAsync(client, version, "Customer");
                return new StressPayloadFactories(
                    Post: () => JsonContent.Create(new AddTaxInformationRequest(
                        TenantId,
                        TaxName: $"IVA-{ShortCode()}",
                        TaxIdentificationNumber: $"ES{RandomDigits(9)}",
                        CustomerId: customerId)),
                    Put: id => JsonContent.Create(new UpdateTaxInformationRequest(
                        Id: id,
                        TenantId: TenantId,
                        TaxName: $"IVA-{ShortCode()}",
                        TaxIdentificationNumber: $"ES{RandomDigits(9)}",
                        CustomerId: customerId)),
                    Patch: _ => JsonContent.Create(new PatchTaxInformationRequest(
                        TenantId,
                        TaxName: $"IVA-{ShortCode()}",
                        TaxIdentificationNumber: $"ES{RandomDigits(9)}",
                        CustomerId: customerId)));
            }
            case "TaxInformationAddress":
            {
                var taxInformationId = await GetIdAsync(client, version, "TaxInformation");
                var addressId = await GetIdAsync(client, version, "Address");
                return new StressPayloadFactories(
                    Post: () => JsonContent.Create(new AddTaxInformationAddressRequest(
                        TenantId,
                        TaxInformationId: taxInformationId,
                        AddressId: addressId,
                        IsPrimary: true,
                        ValidFrom: DateTime.UtcNow.AddDays(-1),
                        ValidTo: null,
                        IsCurrent: true)),
                    Put: _ => JsonContent.Create(new UpdateTaxInformationAddressRequest(
                        TenantId,
                        TaxInformationId: taxInformationId,
                        AddressId: addressId,
                        IsPrimary: true,
                        ValidFrom: DateTime.UtcNow.AddDays(-2),
                        ValidTo: null,
                        IsCurrent: true)),
                    Patch: _ => JsonContent.Create(new UpdateTaxInformationAddressRequest(
                        TenantId,
                        TaxInformationId: taxInformationId,
                        AddressId: addressId,
                        IsPrimary: true,
                        ValidFrom: DateTime.UtcNow.AddDays(-3),
                        ValidTo: null,
                        IsCurrent: true)));
            }
            default:
                return new StressPayloadFactories(
                    Post: () => new StringContent("{}", Encoding.UTF8, "application/json"),
                    Put: _ => new StringContent("{}", Encoding.UTF8, "application/json"),
                    Patch: _ => new StringContent("{}", Encoding.UTF8, "application/json"));
        }
    }

    private static async Task<long> GetIdAsync(HttpClient client, string version, string route)
    {
        if (IdCache.TryGetValue(route, out var id))
        {
            return id;
        }

        var resolvedId = await StressEndpoints.GetFirstIdAsync(client, version, route);
        IdCache[route] = resolvedId;
        return resolvedId;
    }

    private static string ShortCode()
        => $"ST{Guid.NewGuid():N}"[..10];

    private static string RandomDigits(int length)
    {
        var builder = new StringBuilder(length);
        for (var i = 0; i < length; i++)
        {
            builder.Append(Random.Shared.Next(0, 10));
        }

        return builder.ToString();
    }

    private static string GenerateIban()
        => $"ES{RandomDigits(22)}";
}
