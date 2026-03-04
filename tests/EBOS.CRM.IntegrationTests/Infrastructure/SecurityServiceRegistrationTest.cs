using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Infrastructure;

public class SecurityServiceRegistrationTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public void Services_AreRegistered_InHost()
    {
        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider;

        var authenticationService = provider.GetService<IAuthenticationService>();
        var authorizationService = provider.GetService<IAuthorizationService>();
        var policyService = provider.GetService<IPolicyService>();

        Assert.NotNull(authenticationService);
        Assert.NotNull(authorizationService);
        Assert.NotNull(policyService);
    }
}
