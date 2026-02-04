using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.Api.IntegrationTests.Infrastructure;

public class SecurityServiceRegistrationTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public SecurityServiceRegistrationTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public void Services_AreRegistered_InHost()
    {
        using var scope = _factory.Services.CreateScope();
        var provider = scope.ServiceProvider;

        var authenticationService = provider.GetService<IAuthenticationService>();
        var authorizationService = provider.GetService<IAuthorizationService>();

        Assert.NotNull(authenticationService);
        Assert.NotNull(authorizationService);
    }
}
