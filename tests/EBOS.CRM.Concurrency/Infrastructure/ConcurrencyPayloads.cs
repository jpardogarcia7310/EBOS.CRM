using System.Net.Http.Json;
using System.Text;
using System.Collections.Concurrent;
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
using EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformation;
using EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformationAddress;

namespace EBOS.CRM.Concurrency.Infrastructure;

public sealed record ConcurrencyPayloadFactories(
    Func<HttpContent>? Post,
    Func<long, HttpContent>? Put,
    Func<long, HttpContent>? Patch,
    bool AllowDelete,
    bool UseIsolatedWrite,
    Func<Task<IsolatedWritePayloads>>? IsolatedWriteFactory = null);

public sealed record IsolatedWritePayloads(
    Func<HttpContent>? Post,
    Func<long, HttpContent>? Put,
    Func<long, HttpContent>? Patch,
    bool AllowDelete);

public static class ConcurrencyPayloads
{
    private const long TenantId = 1;
    private static readonly Dictionary<string, long> IdCache = new(StringComparer.OrdinalIgnoreCase);
    private static readonly SemaphoreSlim IdSemaphore = new(1, 1);

    public static async Task<ConcurrencyPayloadFactories> GetPayloadFactoriesAsync(
        HttpClient client,
        string version,
        string entity)
    {
        var noWriteEntities = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "AddressType",
            "Country",
            "IdentificationType",
            "Status",
            "TenantConfiguration",
            "TenantQuota",
            "TenantUsageMetric"
        };

        if (noWriteEntities.Contains(entity))
        {
            return new ConcurrencyPayloadFactories(
                Post: null,
                Put: null,
                Patch: null,
                AllowDelete: false,
                UseIsolatedWrite: false);
        }

        switch (entity)
        {
            case "Address":
            {
                var countryId = await GetIdAsync(client, version, "Country");
                var addressTypeId = await GetIdAsync(client, version, "AddressType");
                return new ConcurrencyPayloadFactories(
                    Post: () => JsonContent.Create(new AddAddressRequest(
                        TenantId,
                        Street: $"Concurrency St {ShortCode()}",
                        ExternalNumber: "1",
                        InternalNumber: "2B",
                        BetweenStreet1: "Street A",
                        BetweenStreet2: "Street B",
                        Neighbourhood: "Centro",
                        City: "Madrid",
                        StateOrProvince: "Madrid",
                        PostalCode: "28001",
                        GoogleMapsUrl: "https://maps.example.com/concurrency",
                        Latitude: "40.0",
                        Longitude: "-3.7",
                        CountryId: countryId,
                        AddressTypeId: addressTypeId)),
                    Put: id => JsonContent.Create(new UpdateAddressRequest(
                        TenantId,
                        Street: $"Concurrency St {ShortCode()}",
                        ExternalNumber: "2",
                        InternalNumber: "3C",
                        BetweenStreet1: "Street A",
                        BetweenStreet2: "Street B",
                        Neighbourhood: "Centro",
                        City: "Madrid",
                        StateOrProvince: "Madrid",
                        PostalCode: "28002",
                        GoogleMapsUrl: "https://maps.example.com/concurrency",
                        Latitude: "40.1",
                        Longitude: "-3.6",
                        CountryId: countryId,
                        AddressTypeId: addressTypeId)),
                    Patch: id => JsonContent.Create(new UpdateAddressRequest(
                        TenantId,
                        Street: $"Concurrency St {ShortCode()}",
                        ExternalNumber: "3",
                        InternalNumber: "4D",
                        BetweenStreet1: "Street A",
                        BetweenStreet2: "Street B",
                        Neighbourhood: "Centro",
                        City: "Madrid",
                        StateOrProvince: "Madrid",
                        PostalCode: "28003",
                        GoogleMapsUrl: "https://maps.example.com/concurrency",
                        Latitude: "40.2",
                        Longitude: "-3.5",
                        CountryId: countryId,
                        AddressTypeId: addressTypeId)),
                    AllowDelete: true,
                    UseIsolatedWrite: true);
            }
            case "BankInformation":
            {
                return new ConcurrencyPayloadFactories(
                    Post: null,
                    Put: null,
                    Patch: null,
                    AllowDelete: true,
                    UseIsolatedWrite: true,
                    IsolatedWriteFactory: async () =>
                    {
                        var customerId = await CreateCustomerAsync(client, version);
                        return new IsolatedWritePayloads(
                            Post: () => JsonContent.Create(new AddBankInformationRequest(
                                TenantId,
                                Iban: GenerateIban(),
                                Bic: "BBVAESMM",
                                BankName: "Banco Concurrency",
                                CustomerId: customerId)),
                            Put: _ => JsonContent.Create(new UpdateBankInformationRequest(
                                TenantId,
                                Iban: GenerateIban(),
                                Bic: "SANTESMM",
                                BankName: "Banco Concurrency 2",
                                CustomerId: customerId)),
                            Patch: _ => JsonContent.Create(new UpdateBankInformationRequest(
                                TenantId,
                                Iban: GenerateIban(),
                                Bic: "BESMMXMM",
                                BankName: "Banco Concurrency 3",
                                CustomerId: customerId)),
                            AllowDelete: true);
                    });
            }
            case "BranchOffice":
            {
                return new ConcurrencyPayloadFactories(
                    Post: null,
                    Put: null,
                    Patch: null,
                    AllowDelete: true,
                    UseIsolatedWrite: true,
                    IsolatedWriteFactory: async () =>
                    {
                        var corporateCustomerId = await CreateCorporateCustomerAsync(client, version);
                        return new IsolatedWritePayloads(
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
                                CorporateCustomerId: corporateCustomerId)),
                            AllowDelete: true);
                    });
            }
            case "BranchOfficeAddress":
            {
                return new ConcurrencyPayloadFactories(
                    Post: null,
                    Put: null,
                    Patch: null,
                    AllowDelete: true,
                    UseIsolatedWrite: true,
                    IsolatedWriteFactory: async () =>
                    {
                        var (branchOfficeId, addressId) = await CreateBranchOfficeAndAddressAsync(client, version);
                        return new IsolatedWritePayloads(
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
                                IsCurrent: true)),
                            AllowDelete: true);
                    });
            }
            case "CorporateCustomer":
            {
                var statusId = await GetIdAsync(client, version, "Status");
                return new ConcurrencyPayloadFactories(
                    Post: () => JsonContent.Create(new AddCorporateCustomerRequest(
                        TenantId,
                        Code: ShortCode(),
                        Email: $"{ShortCode()}@example.com",
                        Phone: "+34 600 000 050",
                        StatusId: statusId,
                        LegalName: $"Corp {ShortCode()}",
                        TaxIdentification: RandomDigits(10))),
                    Put: _ => JsonContent.Create(new UpdateCorporateCustomerRequest(
                        TenantId,
                        Code: ShortCode(),
                        Email: $"{ShortCode()}@example.com",
                        Phone: "+34 600 000 051",
                        StatusId: statusId,
                        LegalName: $"Corp {ShortCode()}",
                        TaxIdentification: RandomDigits(10))),
                    Patch: _ => JsonContent.Create(new UpdateCorporateCustomerRequest(
                        TenantId,
                        Code: ShortCode(),
                        Email: $"{ShortCode()}@example.com",
                        Phone: "+34 600 000 052",
                        StatusId: statusId,
                        LegalName: $"Corp {ShortCode()}",
                        TaxIdentification: RandomDigits(10))),
                    AllowDelete: true,
                    UseIsolatedWrite: true,
                    IsolatedWriteFactory: () => Task.FromResult(new IsolatedWritePayloads(
                        Post: () => JsonContent.Create(new AddCorporateCustomerRequest(
                            TenantId,
                            Code: ShortCode(),
                            Email: $"{ShortCode()}@example.com",
                            Phone: "+34 600 000 050",
                            StatusId: statusId,
                            LegalName: $"Corp {ShortCode()}",
                            TaxIdentification: RandomDigits(10))),
                        Put: _ => JsonContent.Create(new UpdateCorporateCustomerRequest(
                            TenantId,
                            Code: ShortCode(),
                            Email: $"{ShortCode()}@example.com",
                            Phone: "+34 600 000 051",
                            StatusId: statusId,
                            LegalName: $"Corp {ShortCode()}",
                            TaxIdentification: RandomDigits(10))),
                        Patch: _ => JsonContent.Create(new UpdateCorporateCustomerRequest(
                            TenantId,
                            Code: ShortCode(),
                            Email: $"{ShortCode()}@example.com",
                            Phone: "+34 600 000 052",
                            StatusId: statusId,
                            LegalName: $"Corp {ShortCode()}",
                            TaxIdentification: RandomDigits(10))),
                        AllowDelete: true)));
            }
            case "CreditAccount":
            {
                return new ConcurrencyPayloadFactories(
                    Post: null,
                    Put: null,
                    Patch: null,
                    AllowDelete: true,
                    UseIsolatedWrite: true,
                    IsolatedWriteFactory: async () =>
                    {
                        var customerId = await CreateCustomerAsync(client, version);
                        return new IsolatedWritePayloads(
                            Post: () => JsonContent.Create(new AddCreditAccountRequest(
                                TenantId,
                                MaxAmount: 10000m,
                                UsedAmount: 100m,
                                CustomerId: customerId)),
                            Put: _ => JsonContent.Create(new UpdateCreditAccountRequest(
                                Id: 0,
                                TenantId: TenantId,
                                MaxAmount: 15000m,
                                UsedAmount: 200m,
                                CustomerId: customerId)),
                            Patch: _ => JsonContent.Create(new PatchCreditAccountRequest(
                                TenantId,
                                MaxAmount: 20000m,
                                UsedAmount: 300m,
                                CustomerId: customerId)),
                            AllowDelete: true);
                    });
            }
            case "CreditTransaction":
            {
                return new ConcurrencyPayloadFactories(
                    Post: null,
                    Put: null,
                    Patch: null,
                    AllowDelete: true,
                    UseIsolatedWrite: true,
                    IsolatedWriteFactory: async () =>
                    {
                        var creditAccountId = await CreateCreditAccountAsync(client, version);
                        return new IsolatedWritePayloads(
                            Post: () => JsonContent.Create(new AddCreditTransactionRequest(
                                TenantId,
                                Date: DateTime.UtcNow.Date,
                                Amount: 500m,
                                Type: "Payment",
                                ExternalReference: ShortCode(),
                                Comments: "Concurrency",
                                CreditAccountId: creditAccountId)),
                            Put: _ => JsonContent.Create(new UpdateCreditTransactionRequest(
                                TenantId,
                                Date: DateTime.UtcNow.Date,
                                Amount: 600m,
                                Type: "Refund",
                                ExternalReference: ShortCode(),
                                Comments: "Concurrency Update",
                                CreditAccountId: creditAccountId)),
                            Patch: _ => JsonContent.Create(new UpdateCreditTransactionRequest(
                                TenantId,
                                Date: DateTime.UtcNow.Date,
                                Amount: 700m,
                                Type: "Adjust",
                                ExternalReference: ShortCode(),
                                Comments: "Concurrency Patch",
                                CreditAccountId: creditAccountId)),
                            AllowDelete: true);
                    });
            }
            case "Customer":
            {
                var statusId = await GetIdAsync(client, version, "Status");
                return new ConcurrencyPayloadFactories(
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
                        StatusId: statusId)),
                    AllowDelete: true,
                    UseIsolatedWrite: true,
                    IsolatedWriteFactory: () => Task.FromResult(new IsolatedWritePayloads(
                        Post: () => JsonContent.Create(new AddCustomerRequest(
                            TenantId,
                            Code: ShortCode(),
                            Email: $"{ShortCode()}@example.com",
                            Phone: "+34 600 000 010",
                            StatusId: statusId)),
                        Put: _ => JsonContent.Create(new UpdateCustomerRequest(
                            Id: 0,
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
                            StatusId: statusId)),
                        AllowDelete: true)));
            }
            case "CustomerAddress":
            {
                return new ConcurrencyPayloadFactories(
                    Post: null,
                    Put: null,
                    Patch: null,
                    AllowDelete: true,
                    UseIsolatedWrite: true,
                    IsolatedWriteFactory: async () =>
                    {
                        var (customerId, addressId) = await CreateCustomerAndAddressAsync(client, version);
                        return new IsolatedWritePayloads(
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
                                IsCurrent: true)),
                            AllowDelete: true);
                    });
            }
            case "IndividualCustomer":
            {
                var statusId = await GetIdAsync(client, version, "Status");
                var identificationTypeId = await GetIdAsync(client, version, "IdentificationType");
                return new ConcurrencyPayloadFactories(
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
                        IdentificationTypeId: identificationTypeId)),
                    AllowDelete: true,
                    UseIsolatedWrite: true,
                    IsolatedWriteFactory: () => Task.FromResult(new IsolatedWritePayloads(
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
                            IdentificationTypeId: identificationTypeId)),
                        AllowDelete: true)));
            }
            case "TaxInformation":
            {
                return new ConcurrencyPayloadFactories(
                    Post: null,
                    Put: null,
                    Patch: null,
                    AllowDelete: true,
                    UseIsolatedWrite: true,
                    IsolatedWriteFactory: async () =>
                    {
                        var customerId = await CreateCustomerAsync(client, version);
                        return new IsolatedWritePayloads(
                            Post: () => JsonContent.Create(new AddTaxInformationRequest(
                                TenantId,
                                TaxName: $"Tax {ShortCode()}",
                                TaxIdentificationNumber: RandomDigits(10),
                                CustomerId: customerId)),
                            Put: _ => JsonContent.Create(new UpdateTaxInformationRequest(
                                Id: 0,
                                TenantId: TenantId,
                                TaxName: $"Tax {ShortCode()}",
                                TaxIdentificationNumber: RandomDigits(10),
                                CustomerId: customerId)),
                            Patch: _ => JsonContent.Create(new UpdateTaxInformationRequest(
                                Id: 0,
                                TenantId: TenantId,
                                TaxName: $"Tax {ShortCode()}",
                                TaxIdentificationNumber: RandomDigits(10),
                                CustomerId: customerId)),
                            AllowDelete: true);
                    });
            }
            case "TaxInformationAddress":
            {
                return new ConcurrencyPayloadFactories(
                    Post: null,
                    Put: null,
                    Patch: null,
                    AllowDelete: true,
                    UseIsolatedWrite: true,
                    IsolatedWriteFactory: async () =>
                    {
                        var (taxInformationId, addressId) = await CreateTaxInformationAndAddressAsync(client, version);
                        return new IsolatedWritePayloads(
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
                                IsCurrent: true)),
                            AllowDelete: true);
                    });
            }
            default:
                return new ConcurrencyPayloadFactories(
                    Post: () => new StringContent("{}", Encoding.UTF8, "application/json"),
                    Put: _ => new StringContent("{}", Encoding.UTF8, "application/json"),
                    Patch: _ => new StringContent("{}", Encoding.UTF8, "application/json"),
                    AllowDelete: true,
                    UseIsolatedWrite: false);
        }
    }

    private static async Task<long> GetIdAsync(HttpClient client, string version, string route)
    {
        await IdSemaphore.WaitAsync();
        try
        {
            if (IdCache.TryGetValue(route, out var id))
            {
                return id;
            }

            var resolvedId = await ConcurrencyEndpoints.GetFirstIdAsync(client, version, route);
            IdCache[route] = resolvedId;
            return resolvedId;
        }
        finally
        {
            IdSemaphore.Release();
        }
    }

    private static string ShortCode()
        => $"CC{Guid.NewGuid():N}"[..10];

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

    private static async Task<long> TryReadIdAsync(HttpResponseMessage response)
    {
        var payload = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(payload))
        {
            return 0;
        }

        using var document = System.Text.Json.JsonDocument.Parse(payload);
        if (document.RootElement.ValueKind != System.Text.Json.JsonValueKind.Object)
        {
            return 0;
        }

        foreach (var property in document.RootElement.EnumerateObject())
        {
            if (!string.Equals(property.Name, "id", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (property.Value.ValueKind == System.Text.Json.JsonValueKind.Number &&
                property.Value.TryGetInt64(out var id))
            {
                return id;
            }
        }

        return 0;
    }
}
