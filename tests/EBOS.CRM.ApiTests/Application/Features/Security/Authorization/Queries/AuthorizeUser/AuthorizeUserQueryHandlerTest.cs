using EBOS.CRM.Contracts.Requests.Security;
using EBOS.CRM.Contracts.Responses.Security;
using EBOS.CRM.Application.Features.Security.Authorization.Queries.AuthorizeUser;
using EBOS.CRM.Domain.Interfaces.Services;
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

        var serviceRequest = new AuthorizeUserRequest(request.UserId, request.PolicyCode);
        var serviceResponse = new AuthorizeUserResponse(true);

        _serviceMock.Setup(s => s.AuthorizeAsync(serviceRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResponse);

        var query = new AuthorizeUserQuery(request);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsAuthorized);
        _serviceMock.Verify(s => s.AuthorizeAsync(serviceRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Unauthorized_ReturnsFalse()
    {
        var request = new AuthorizeUserRequest(UserId: 1, PolicyCode: "crm.customer.access");

        var serviceRequest = new AuthorizeUserRequest(request.UserId, request.PolicyCode);
        var serviceResponse = new AuthorizeUserResponse(false);

        _serviceMock.Setup(s => s.AuthorizeAsync(serviceRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceResponse);

        var query = new AuthorizeUserQuery(request);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsAuthorized);
        _serviceMock.Verify(s => s.AuthorizeAsync(serviceRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceThrows_PropagatesException()
    {
        var request = new AuthorizeUserRequest(UserId: 1, PolicyCode: "crm.customer.access");

        var serviceRequest = new AuthorizeUserRequest(request.UserId, request.PolicyCode);

        _serviceMock.Setup(s => s.AuthorizeAsync(serviceRequest, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Service error"));

        var query = new AuthorizeUserQuery(request);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, CancellationToken.None));
    }
}