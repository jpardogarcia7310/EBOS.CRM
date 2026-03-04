using EBOS.CRM.Application.Mappings.CRM;
using EBOS.CRM.Application.Mappings.EBOS;
using FluentAssertions;
using Mapster;

namespace EBOS.CRM.ApiTests.Application.Mappings;

public class MappingProfilesRegistrationTest
{
    [Fact]
    public void All_Mapping_Profiles_Register_And_Compile()
    {
        var config = new TypeAdapterConfig();
        var profiles = new IRegister[]
        {
            new MappingAccountContact(),
            new MappingAccountContactRole(),
            new MappingAccountHierarchy(),
            new MappingAddress(),
            new MappingBankInformation(),
            new MappingBranchOffice(),
            new MappingBranchOfficeAddress(),
            new MappingCase(),
            new MappingCaseActivity(),
            new MappingCorporateCustomer(),
            new MappingCreditAccount(),
            new MappingCreditTransaction(),
            new MappingCustomer(),
            new MappingCustomerAddress(),
            new MappingCustomerConsent(),
            new MappingCustomerPreference(),
            new MappingIndividualCustomer(),
            new MappingLead(),
            new MappingOpportunity(),
            new MappingOpportunityStage(),
            new MappingQueue(),
            new MappingQuote(),
            new MappingSla(),
            new MappingTaxInformation(),
            new MappingTaxInformationAddress(),
            new MappingAddressType(),
            new MappingChannelCountry(),
            new MappingChannelType(),
            new MappingCountry(),
            new MappingIdentificationType(),
            new MappingStatus(),
            new MappingTenantConfiguration(),
            new MappingTenantQuota(),
            new MappingTenantUsageMetric(),
            new MappingValidationRule()
        };

        foreach (var profile in profiles)
        {
            profile.Register(config);
        }

        var act = () => config.Compile();
        act.Should().NotThrow();
    }
}
