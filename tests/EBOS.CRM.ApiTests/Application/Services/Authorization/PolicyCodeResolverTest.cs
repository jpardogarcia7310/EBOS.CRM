using EBOS.CRM.Application.Features.CRM.Address.Commands.AddAddress;
using EBOS.CRM.Application.Features.EBOS.Countries.Queries.GetAllCountries;
using EBOS.CRM.Application.Shared.Authorization;

namespace EBOS.CRM.ApiTests.Application.Services.Authorization;

public class PolicyCodeResolverTest
{
    [Fact]
    public void Resolve_CrmCommand_ReturnsExpectedCode()
    {
        var code = PolicyCodeResolver.Resolve(typeof(AddAddressCommand));

        Assert.Equal("crm.address.create", code);
    }

    [Fact]
    public void Resolve_NonCrmQuery_ReturnsExpectedCode()
    {
        var code = PolicyCodeResolver.Resolve(typeof(GetAllCountriesQuery));

        Assert.Equal("ebos.country.read", code);
    }
}
