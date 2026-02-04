using EBOS.CRM.Application.Contracts.Requests.Security;
using EBOS.CRM.Application.Contracts.Responses.Security;
using EBOS.CRM.Application.Features.Security.Authorization.Queries.AuthorizeUser;
using EBOS.CRM.Application.Services.Interfaces;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.Security.Authorization.Queries.AuthorizeUser;

public class AuthorizeUserQueryHandlerTest
{
    private readonly Mock<IAuthorizationService> _serviceMock;
    private readonly AuthorizeUserQueryHandler _handler;

    public AuthorizeUserQueryHandlerTest()
    {
        _serviceMock = new Mock<IAuthorizationService>();
        _handler = new AuthorizeUserQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsResponse()
    {
        var request = new AuthorizeUserRequest(UserId: 1, PolicyCode: "crm.customer.access");
        var response = new AuthorizeUserResponse(true);

        _serviceMock.Setup(s => s.AuthorizeAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var query = new AuthorizeUserQuery(request);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsAuthorized);
        _serviceMock.Verify(s => s.AuthorizeAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceThrows_PropagatesException()
    {
        var request = new AuthorizeUserRequest(UserId: 1, PolicyCode: "crm.customer.access");

        _serviceMock.Setup(s => s.AuthorizeAsync(request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Service error"));

        var query = new AuthorizeUserQuery(request);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CancellationRequested_ThrowsOperationCanceled()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var request = new AuthorizeUserRequest(UserId: 1, PolicyCode: "crm.customer.access");
        var query = new AuthorizeUserQuery(request);

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(query, cts.Token));
    }
}
