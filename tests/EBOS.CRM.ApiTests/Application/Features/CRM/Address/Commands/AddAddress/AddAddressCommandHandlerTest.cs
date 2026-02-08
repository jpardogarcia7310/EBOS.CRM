using EBOS.CRM.Application.Contracts.Requests.CRM.Address;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Application.Contracts.Responses.Services;
using EBOS.CRM.Application.Features.CRM.Address.Commands.AddAddress;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;
using CRMAddress = EBOS.CRM.Domain.Entities.CRM.Address;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Address.Commands.AddAddress;

public class AddAddressCommandHandlerTest
{
    private readonly Mock<IAddressRepository> _repositoryMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<ICurrentUserContext> _currentUserMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AddAddressCommandHandler _handler;

    public AddAddressCommandHandlerTest()
    {
        _repositoryMock = new Mock<IAddressRepository>();
        _auditServiceMock = new Mock<IAuditService>();
        _currentUserMock = new Mock<ICurrentUserContext>();
        _mapperMock = new Mock<IMapper>();

        _currentUserMock.SetupGet(x => x.UserId).Returns(1);
        _currentUserMock.SetupGet(x => x.CorrelationId).Returns("corr-1");

        _auditServiceMock.Setup(a => a.InsertAuditAsync(
                It.IsAny<EBOS.CRM.Application.Contracts.Requests.Services.AuditInsertRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        _handler = new AddAddressCommandHandler(
            _repositoryMock.Object,
            _auditServiceMock.Object,
            _currentUserMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_AddsAndAudits()
    {
        var request = BuildValidRequest();
        var entity = new CRMAddress
        {
            Id = 1,
            Street = request.Street,
            ExternalNumber = request.ExternalNumber,
            City = request.City,
            StateOrProvince = request.StateOrProvince,
            PostalCode = request.PostalCode,
            CountryId = request.CountryId,
            AddressTypeId = request.AddressTypeId
        };
        var response = new AddressResponse(1, request.TenantId, request.Street, request.ExternalNumber, request.InternalNumber,
            request.BetweenStreet1, request.BetweenStreet2, request.Neighbourhood, request.City,
            request.StateOrProvince, request.PostalCode, request.GoogleMapsUrl, request.Latitude,
            request.Longitude, request.CountryId, request.AddressTypeId, true);

        _mapperMock.Setup(m => m.Map<CRMAddress>(request)).Returns(entity);
        _mapperMock.Setup(m => m.Map<AddressResponse>(entity)).Returns(response);

        var result = await _handler.Handle(new AddAddressCommand(request), CancellationToken.None);

        Assert.NotNull(result);
        _repositoryMock.Verify(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(a => a.InsertAuditAsync(
            It.IsAny<EBOS.CRM.Application.Contracts.Requests.Services.AuditInsertRequest>(),
            It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSaveChangesThrows_RollsBack()
    {
        var request = BuildValidRequest();
        var entity = new CRMAddress
        {
            Street = request.Street,
            ExternalNumber = request.ExternalNumber,
            City = request.City,
            StateOrProvince = request.StateOrProvince,
            PostalCode = request.PostalCode,
            CountryId = request.CountryId,
            AddressTypeId = request.AddressTypeId
        };

        _mapperMock.Setup(m => m.Map<CRMAddress>(request)).Returns(entity);
        _repositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(new AddAddressCommand(request), CancellationToken.None));

        _repositoryMock.Verify(r => r.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static AddAddressRequest BuildValidRequest() => new(
            TenantId: 1,
        Street: "Main St",
        ExternalNumber: "123",
        InternalNumber: null,
        BetweenStreet1: null,
        BetweenStreet2: null,
        Neighbourhood: null,
        City: "Quito",
        StateOrProvince: "Pichincha",
        PostalCode: "EC17001",
        GoogleMapsUrl: null,
        Latitude: null,
        Longitude: null,
        CountryId: 1,
        AddressTypeId: 1
    );
}


