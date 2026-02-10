using EBOS.CRM.Application.Behavior;
using EBOS.CRM.Application.Contracts.Requests.CRM.Address;
using EBOS.CRM.Application.Features.CRM.Address.Commands.AddAddress;
using EBOS.CRM.Application.Services.Authorization;
using EBOS.CRM.Application.Services.Interfaces;
using MediatR;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Behavior;

public class PolicyAuthorizationBehaviorTest
{
    private readonly Mock<ICurrentUserContext> _currentUser;
    private readonly Mock<IPolicyService> _policyService;

    public PolicyAuthorizationBehaviorTest()
    {
        _currentUser = new Mock<ICurrentUserContext>();
        _policyService = new Mock<IPolicyService>();
    }

    [Fact]
    public async Task Handle_ResolvesPolicy_AndCallsService()
    {
        _currentUser.SetupGet(x => x.UserId).Returns(123);
        var behavior = new PolicyAuthorizationBehavior<AddAddressCommand, object>(
            _currentUser.Object,
            _policyService.Object);

        var request = new AddAddressCommand(BuildRequest());
        var expectedPolicy = PolicyCodeResolver.Resolve(typeof(AddAddressCommand));

        var nextCalled = false;
        RequestHandlerDelegate<object> next = _ =>
        {
            nextCalled = true;
            return Task.FromResult(new object());
        };

        await behavior.Handle(request, next, CancellationToken.None);

        Assert.True(nextCalled);
        _policyService.Verify(s => s.EnsureAuthorizedAsync(
            123,
            expectedPolicy,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NoUser_SkipsPolicyCheck()
    {
        _currentUser.SetupGet(x => x.UserId).Returns(0);
        var behavior = new PolicyAuthorizationBehavior<AddAddressCommand, object>(
            _currentUser.Object,
            _policyService.Object);

        var request = new AddAddressCommand(BuildRequest());

        var nextCalled = false;
        RequestHandlerDelegate<object> next = _ =>
        {
            nextCalled = true;
            return Task.FromResult(new object());
        };

        await behavior.Handle(request, next, CancellationToken.None);

        Assert.True(nextCalled);
        _policyService.Verify(
            s => s.EnsureAuthorizedAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static AddAddressRequest BuildRequest() => new(
        Street: "Main St",
        ExternalNumber: "123",
        InternalNumber: null,
        BetweenStreet1: null,
        BetweenStreet2: null,
        Neighbourhood: "Center",
        City: "Quito",
        StateOrProvince: "Pichincha",
        PostalCode: "EC17001",
        GoogleMapsUrl: "https://maps.example.com/q",
        Latitude: "0",
        Longitude: "0",
        CountryId: 1,
        AddressTypeId: 1
    );
}
