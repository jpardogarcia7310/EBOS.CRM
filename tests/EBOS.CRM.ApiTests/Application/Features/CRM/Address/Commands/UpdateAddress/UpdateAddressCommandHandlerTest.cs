using EBOS.CRM.Contracts.Requests.CRM.Address;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.Address.Commands.UpdateAddress;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using Moq;
using CRMAddress = EBOS.CRM.Domain.Entities.CRM.Address;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Address.Commands.UpdateAddress;

public class UpdateAddressCommandHandlerTest
{
    private readonly Mock<IAddressRepository> _repositoryMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateAddressCommandHandler _handler;

    public UpdateAddressCommandHandlerTest()
    {
        _repositoryMock = new Mock<IAddressRepository>();
        _auditServiceMock = new Mock<IAuditService>();
        var currentUserMock = new Mock<ICurrentUserContext>();
        _mapperMock = new Mock<IMapper>();
        var referenceValidationMock = new Mock<IAddressReferenceValidationService>();
        referenceValidationMock
            .Setup(x => x.EnsureReferencesAvailableAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        currentUserMock.SetupGet(x => x.UserId).Returns(1);
        currentUserMock.SetupGet(x => x.CorrelationId).Returns("corr-1");

        _auditServiceMock.Setup(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        _handler = new UpdateAddressCommandHandler(
            _repositoryMock.Object,
            _auditServiceMock.Object,
            currentUserMock.Object,
            _mapperMock.Object,
            referenceValidationMock.Object);
    }

    [Fact]
    public async Task Handle_WhenEntityNotFound_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CRMAddress?)null!);

        var result = await _handler.Handle(new UpdateAddressCommand(1, BuildValidRequest()), CancellationToken.None);

        Assert.Null(result);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CRMAddress>(), It.IsAny<CancellationToken>()), Times.Never);
        _auditServiceMock.Verify(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenEntityFound_UpdatesAndAudits()
    {
        var entity = new CRMAddress
        {
            Id = 1,
            Street = "Old",
            ExternalNumber = "1",
            City = "Quito",
            StateOrProvince = "Pichincha",
            PostalCode = "EC17001",
            CountryId = 1,
            AddressTypeId = 1
        };
        var response = new AddressResponse(1, 1, "New", "2", null, null, null, null, "Quito", "Pichincha",
            "EC17001", null, null, null, 1, 1, true);

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map(It.IsAny<UpdateAddressRequest>(), entity))
            .Returns(entity);
        _mapperMock.Setup(m => m.Map<AddressResponse>(entity))
            .Returns(response);

        var result = await _handler.Handle(new UpdateAddressCommand(1, BuildValidRequest()), CancellationToken.None);

        Assert.NotNull(result);
        _repositoryMock.Verify(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(a => a.InsertAuditAsync(
            It.IsAny<AuditInsertRequest>(),
            It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSaveChangesThrows_RollsBack()
    {
        var entity = new CRMAddress
        {
            Id = 1,
            Street = "Old",
            ExternalNumber = "1",
            City = "Quito",
            StateOrProvince = "Pichincha",
            PostalCode = "EC17001",
            CountryId = 1,
            AddressTypeId = 1
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map(It.IsAny<UpdateAddressRequest>(), entity))
            .Returns(entity);
        _repositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(new UpdateAddressCommand(1, BuildValidRequest()), CancellationToken.None));

        _repositoryMock.Verify(r => r.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static UpdateAddressRequest BuildValidRequest() => new(
            TenantId: 1,
        Street: "New",
        ExternalNumber: "2",
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


