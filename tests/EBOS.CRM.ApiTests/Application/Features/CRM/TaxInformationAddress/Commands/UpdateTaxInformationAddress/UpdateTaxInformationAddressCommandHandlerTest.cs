using EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformationAddress;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Application.Contracts.Responses.Services;
using EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Commands.UpdateTaxInformationAddress;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;
using CRMTaxInformationAddress = EBOS.CRM.Domain.Entities.CRM.TaxInformationAddress;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.TaxInformationAddress.Commands.UpdateTaxInformationAddress;

public class UpdateTaxInformationAddressCommandHandlerTest
{
    private readonly Mock<ITaxInformationAddressRepository> _repositoryMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateTaxInformationAddressCommandHandler _handler;

    public UpdateTaxInformationAddressCommandHandlerTest()
    {
        _repositoryMock = new Mock<ITaxInformationAddressRepository>();
        _auditServiceMock = new Mock<IAuditService>();
        var currentUserMock = new Mock<ICurrentUserContext>();
        _mapperMock = new Mock<IMapper>();

        currentUserMock.SetupGet(x => x.UserId).Returns(1);
        currentUserMock.SetupGet(x => x.CorrelationId).Returns("corr-1");

        _auditServiceMock.Setup(a => a.InsertAuditAsync(
                It.IsAny<EBOS.CRM.Application.Contracts.Requests.Services.AuditInsertRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        _handler = new UpdateTaxInformationAddressCommandHandler(
            _repositoryMock.Object,
            _auditServiceMock.Object,
            currentUserMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WhenEntityNotFound_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CRMTaxInformationAddress?)null!);

        var result = await _handler.Handle(new UpdateTaxInformationAddressCommand(1, BuildUpdateRequest()), CancellationToken.None);

        Assert.Null(result);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CRMTaxInformationAddress>(), It.IsAny<CancellationToken>()), Times.Never);
        _auditServiceMock.Verify(a => a.InsertAuditAsync(
            It.IsAny<EBOS.CRM.Application.Contracts.Requests.Services.AuditInsertRequest>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenEntityFound_UpdatesAndAudits()
    {
        var entity = new CRMTaxInformationAddress();

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map(It.IsAny<UpdateTaxInformationAddressRequest>(), entity))
            .Returns(entity);
        _mapperMock.Setup(m => m.Map<TaxInformationAddressResponse>(entity))
            .Returns(TestResponseFactory.Create<TaxInformationAddressResponse>());

        var result = await _handler.Handle(new UpdateTaxInformationAddressCommand(1, BuildUpdateRequest()), CancellationToken.None);

        Assert.NotNull(result);
        _repositoryMock.Verify(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(a => a.InsertAuditAsync(
            It.IsAny<EBOS.CRM.Application.Contracts.Requests.Services.AuditInsertRequest>(),
            It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSaveChangesThrows_RollsBack()
    {
        var entity = new CRMTaxInformationAddress();

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map(It.IsAny<UpdateTaxInformationAddressRequest>(), entity))
            .Returns(entity);
        _repositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(new UpdateTaxInformationAddressCommand(1, BuildUpdateRequest()), CancellationToken.None));

        _repositoryMock.Verify(r => r.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static UpdateTaxInformationAddressRequest BuildUpdateRequest() => new(
            TaxInformationId: 1,
            AddressId: 1,
            IsPrimary: true,
            ValidFrom: DateTime.UtcNow,
            ValidTo: null,
            IsCurrent: true
        );
}