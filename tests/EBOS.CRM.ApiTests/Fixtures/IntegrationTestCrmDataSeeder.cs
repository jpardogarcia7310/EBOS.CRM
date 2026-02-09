using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Infrastructure.Persistence;

namespace EBOS.CRM.ApiTests.Fixtures;

public static class IntegrationTestCrmDataSeeder
{
    public static void Seed(CrmDbContext context)
    {
        IntegrationTestCountriesDataSeeder.Seed(context);
        IntegrationTestStatusesDataSeeder.Seed(context);

        if (!context.AddressTypes.Any())
        {
            context.AddressTypes.AddRange(
                new AddressType
                {
                    Code = "HOME",
                    Description = "Home",
                    Category = "Residential",
                    AllowsMultiple = true,
                    RequiresPrimary = true
                },
                new AddressType
                {
                    Code = "BILL",
                    Description = "Billing",
                    Category = "Billing",
                    AllowsMultiple = false,
                    RequiresPrimary = true
                },
                new AddressType
                {
                    Code = "SHIP",
                    Description = "Shipping",
                    Category = "Logistics",
                    AllowsMultiple = true,
                    RequiresPrimary = false
                }
            );
        }

        if (!context.IdentificationTypes.Any())
        {
            context.IdentificationTypes.AddRange(
                new IdentificationType
                {
                    Code = "DNI",
                    Description = "Documento Nacional de Identidad"
                },
                new IdentificationType
                {
                    Code = "PASS",
                    Description = "Passport"
                }
            );
        }

        context.SaveChanges();

        var statusActiveId = context.Statuses.First().Id;
        var countrySpainId = context.Countries.First().Id;
        var addressTypeHomeId = context.AddressTypes.First().Id;
        var idTypeDniId = context.IdentificationTypes.First().Id;

        if (!context.Customers.Any())
        {
            var corp = new CorporateCustomer
            {
                Code = "CORP-001",
                Email = "accounting@contoso.com",
                Phone = "+34 911 000 111",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                StatusId = statusActiveId,
                LegalName = "Contoso S.A.",
                TaxIdentification = "B12345678"
            };

            var indiv = new IndividualCustomer
            {
                Code = "IND-001",
                Email = "jane.doe@example.com",
                Phone = "+34 600 123 456",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                StatusId = statusActiveId,
                FirstName = "Jane",
                LastName = "Doe",
                BirthDate = new DateTime(1990, 5, 20),
                IdentificationNumber = "1234567890",
                IdentificationTypeId = idTypeDniId
            };

            context.CorporateCustomers.Add(corp);
            context.IndividualCustomers.Add(indiv);
            context.SaveChanges();
        }

        if (!context.Addresses.Any())
        {
            context.Addresses.AddRange(
                new Address
                {
                    Street = "Gran Vía",
                    ExternalNumber = "1",
                    InternalNumber = "2B",
                    BetweenStreet1 = "Calle de Alcalá",
                    BetweenStreet2 = "Calle de la Montera",
                    Neighbourhood = "Centro",
                    City = "Madrid",
                    StateOrProvince = "Madrid",
                    PostalCode = "28013",
                    GoogleMapsUrl = "https://maps.example.com/gran-via-1",
                    Latitude = 40.4203m,
                    Longitude = -3.7058m,
                    CountryId = countrySpainId,
                    AddressTypeId = addressTypeHomeId
                },
                new Address
                {
                    Street = "Passeig de Gràcia",
                    ExternalNumber = "45",
                    InternalNumber = null,
                    BetweenStreet1 = "Carrer d'Aragó",
                    BetweenStreet2 = "Carrer de València",
                    Neighbourhood = "Eixample",
                    City = "Barcelona",
                    StateOrProvince = "Catalunya",
                    PostalCode = "08007",
                    GoogleMapsUrl = "https://maps.example.com/passeig-gracia-45",
                    Latitude = 41.3927m,
                    Longitude = 2.1649m,
                    CountryId = countrySpainId,
                    AddressTypeId = addressTypeHomeId
                }
            );
            context.SaveChanges();
        }

        var corporateCustomerId = context.CorporateCustomers.Select(c => c.Id).First();
        var individualCustomerId = context.IndividualCustomers.Select(c => c.Id).First();
        var addressId = context.Addresses.Select(a => a.Id).First();

        if (!context.BankInformation.Any())
        {
            context.BankInformation.Add(new BankInformation
            {
                Iban = "ES7921000813610123456789",
                Bic = "CAIXESBBXXX",
                BankName = "Banco Ejemplo",
                CustomerId = corporateCustomerId
            });
        }

        if (!context.CreditAccounts.Any())
        {
            context.CreditAccounts.Add(new CreditAccount
            {
                MaxAmount = 10000m,
                UsedAmount = 1500m,
                CustomerId = corporateCustomerId
            });
        }

        context.SaveChanges();

        var creditAccountId = context.CreditAccounts.Select(c => c.Id).First();

        if (!context.CreditTransactions.Any())
        {
            context.CreditTransactions.Add(new CreditTransaction
            {
                Date = DateTime.UtcNow.AddDays(-2),
                Amount = 200m,
                Type = "Consumption",
                ExternalReference = "INV-1001",
                Comments = "Monthly service charge",
                CreditAccountId = creditAccountId
            });
        }

        if (!context.TaxInformation.Any())
        {
            context.TaxInformation.Add(new TaxInformation
            {
                TaxName = "IVA",
                TaxIdentificationNumber = "ESB12345678",
                CustomerId = corporateCustomerId
            });
        }

        if (!context.BranchOffices.Any())
        {
            context.BranchOffices.Add(new BranchOffice
            {
                Name = "HQ Madrid",
                PhoneNumber = "+34 911 000 222",
                CorporateCustomerId = corporateCustomerId
            });
        }

        context.SaveChanges();

        var branchOfficeId = context.BranchOffices.Select(b => b.Id).First();
        var taxInformationId = context.TaxInformation.Select(t => t.Id).First();

        if (!context.Set<CustomerAddress>().Any())
        {
            context.Set<CustomerAddress>().Add(new CustomerAddress
            {
                CustomerId = individualCustomerId,
                AddressId = addressId,
                IsPrimary = true,
                ValidFrom = DateTime.UtcNow.AddDays(-5),
                ValidTo = null,
                IsCurrent = true
            });
        }

        if (!context.Set<BranchOfficeAddress>().Any())
        {
            context.Set<BranchOfficeAddress>().Add(new BranchOfficeAddress
            {
                BranchOfficeId = branchOfficeId,
                AddressId = addressId,
                IsPrimary = true,
                ValidFrom = DateTime.UtcNow.AddDays(-10),
                ValidTo = null,
                IsCurrent = true
            });
        }

        if (!context.Set<TaxInformationAddress>().Any())
        {
            context.Set<TaxInformationAddress>().Add(new TaxInformationAddress
            {
                TaxInformationId = taxInformationId,
                AddressId = addressId,
                IsPrimary = true,
                ValidFrom = DateTime.UtcNow.AddDays(-10),
                ValidTo = null,
                IsCurrent = true
            });
        }

        context.SaveChanges();

        if (!context.TenantConfigurations.Any())
        {
            context.TenantConfigurations.AddRange(
                new TenantConfiguration
                {
                    TenantId = 1,
                    Key = "limits.maxUsers",
                    ValueJson = "{\"value\":25}",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 1
                },
                new TenantConfiguration
                {
                    TenantId = 1,
                    Key = "features.beta",
                    ValueJson = "{\"enabled\":false}",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 1
                });
        }

        if (!context.TenantQuotas.Any())
        {
            context.TenantQuotas.Add(
                new TenantQuota
                {
                    TenantId = 1,
                    Metric = "users",
                    Limit = 100,
                    Unit = "count",
                    EffectiveFrom = DateTime.UtcNow.AddDays(-1)
                });
        }

        if (!context.TenantUsageMetrics.Any())
        {
            context.TenantUsageMetrics.Add(
                new TenantUsageMetric
                {
                    TenantId = 1,
                    Metric = "api.calls",
                    Value = 250,
                    Unit = "count",
                    PeriodStart = DateTime.UtcNow.AddDays(-7),
                    PeriodEnd = DateTime.UtcNow,
                    Source = "gateway"
                });
        }

        context.SaveChanges();
    }
}


