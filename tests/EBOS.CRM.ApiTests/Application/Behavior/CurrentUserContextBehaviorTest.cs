using EBOS.CRM.Application.Behavior;
using EBOS.CRM.Application.Contracts.Requests.CRM.Address;
using EBOS.CRM.Application.Contracts.Requests.Security;
using EBOS.CRM.Application.Features.CRM.Address.Commands.AddAddress;
using EBOS.CRM.Application.Features.Security.Authentication.Commands.AuthenticateUser;
using EBOS.CRM.Application.Services.Interfaces;
using MediatR;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Behavior;

public class CurrentUserContextBehaviorTest
{
    private readonly Mock<ICurrentUserContext> _currentUser;

    public CurrentUserContextBehaviorTest()
    {
        _currentUser = new Mock<ICurrentUserContext>();
    }

    [Fact]
    public async Task Handle_CommandWithoutUser_ThrowsUnauthorized()
    {
        _currentUser.SetupGet(x => x.UserId).Returns(0);
        var behavior = new CurrentUserContextBehavior<AddAddressCommand, object>(_currentUser.Object);
        var request = new AddAddressCommand(BuildRequest());

        RequestHandlerDelegate<object> next = _ => Task.FromResult(new object());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => behavior.Handle(request, next, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CommandWithUser_AllowsExecution()
    {
        _currentUser.SetupGet(x => x.UserId).Returns(5);
        var behavior = new CurrentUserContextBehavior<AddAddressCommand, object>(_currentUser.Object);
        var request = new AddAddressCommand(BuildRequest());

        var nextCalled = false;
        RequestHandlerDelegate<object> next = _ =>
        {
            nextCalled = true;
            return Task.FromResult(new object());
        };

        await behavior.Handle(request, next, CancellationToken.None);

        Assert.True(nextCalled);
    }

    [Fact]
    public async Task Handle_SecurityCommand_DoesNotEnforceUser()
    {
        _currentUser.SetupGet(x => x.UserId).Returns(0);
        var behavior = new CurrentUserContextBehavior<AuthenticateUserCommand, object>(_currentUser.Object);
        var request = new AuthenticateUserCommand(new AuthenticateUserRequest(
            ExternalId: "external-1",
            Username: "user",
            Email: "user@demo.local",
            DisplayName: "User Demo",
            IsActive: true));

        var nextCalled = false;
        RequestHandlerDelegate<object> next = _ =>
        {
            nextCalled = true;
            return Task.FromResult(new object());
        };

        await behavior.Handle(request, next, CancellationToken.None);

        Assert.True(nextCalled);
    }

    [Fact]
    public async Task Handle_AllowAnonymousRequest_DoesNotEnforceUser()
    {
        _currentUser.SetupGet(x => x.UserId).Returns(0);
        var behavior = new CurrentUserContextBehavior<AnonymousRequest, object>(_currentUser.Object);
        var request = new AnonymousRequest();

        var nextCalled = false;
        RequestHandlerDelegate<object> next = _ =>
        {
            nextCalled = true;
            return Task.FromResult(new object());
        };

        await behavior.Handle(request, next, CancellationToken.None);

        Assert.True(nextCalled);
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

    private sealed record AnonymousRequest : IAllowAnonymousRequest;
}
