using EBOS.CRM.Application.Contracts.Requests.Security;
using EBOS.CRM.Application.Contracts.Responses.Security;
using EBOS.CRM.Application.Features.Security.Authentication.Commands.AuthenticateUser;
using EBOS.CRM.Application.Services.Interfaces;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.Security.Authentication.Commands.AuthenticateUser;

public class AuthenticateUserCommandHandlerTest
{
    private readonly Mock<IAuthenticationService> _serviceMock;
    private readonly AuthenticateUserCommandHandler _handler;

    public AuthenticateUserCommandHandlerTest()
    {
        _serviceMock = new Mock<IAuthenticationService>();
        _handler = new AuthenticateUserCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsResponse()
    {
        var request = new AuthenticateUserRequest(
            ExternalId: "ext-1",
            Username: "jdoe",
            Email: "jdoe@example.com",
            DisplayName: "John Doe",
            IsActive: true);

        var response = new AuthenticatedUserResponse(
            UserId: 1,
            ExternalId: request.ExternalId,
            Username: request.Username,
            Email: request.Email,
            DisplayName: request.DisplayName,
            IsActive: request.IsActive,
            Roles: Array.Empty<string>(),
            Permissions: Array.Empty<string>());

        _serviceMock.Setup(s => s.AuthenticateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var command = new AuthenticateUserCommand(request);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.UserId);
        Assert.Equal("jdoe", result.Username);
        _serviceMock.Verify(s => s.AuthenticateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceThrows_PropagatesException()
    {
        var request = new AuthenticateUserRequest(
            ExternalId: "ext-1",
            Username: "jdoe",
            Email: "jdoe@example.com",
            DisplayName: "John Doe",
            IsActive: true);

        _serviceMock.Setup(s => s.AuthenticateAsync(request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Service error"));

        var command = new AuthenticateUserCommand(request);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CancellationRequested_ThrowsOperationCanceled()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var request = new AuthenticateUserRequest(
            ExternalId: "ext-1",
            Username: "jdoe",
            Email: "jdoe@example.com",
            DisplayName: "John Doe",
            IsActive: true);

        var command = new AuthenticateUserCommand(request);

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(command, cts.Token));
    }
}
