using EBOS.CRM.Contracts.Requests.CRM.CorporateCustomer;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Application.Features.CRM.CorporateCustomer.Commands.UpdateCorporateCustomer;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using Moq;
using CRMCorporateCustomer = EBOS.CRM.Domain.Entities.CRM.CorporateCustomer;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CorporateCustomer.Commands.UpdateCorporateCustomer;

public class UpdateCorporateCustomerCommandHandlerTest
{
    private readonly Mock<ICorporateCustomerRepository> _repositoryMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ICustomerReferenceValidationService> _referenceValidationMock;
    private readonly UpdateCorporateCustomerCommandHandler _handler;

    public UpdateCorporateCustomerCommandHandlerTest()
    {
        _repositoryMock = new Mock<ICorporateCustomerRepository>();
        _auditServiceMock = new Mock<IAuditService>();
        var currentUserMock = new Mock<ICurrentUserContext>();
        _mapperMock = new Mock<IMapper>();
        _referenceValidationMock = new Mock<ICustomerReferenceValidationService>();

        currentUserMock.SetupGet(x => x.UserId).Returns(1);
        currentUserMock.SetupGet(x => x.CorrelationId).Returns("corr-1");

        _auditServiceMock.Setup(a => a.InsertAuditAsync(
                It.IsAny<AuditInsertRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        _handler = new UpdateCorporateCustomerCommandHandler(
            _repositoryMock.Object,
            _auditServiceMock.Object,
            currentUserMock.Object,
            _mapperMock.Object,
            _referenceValidationMock.Object);
    }

    [Fact]
    public async Task Handle_WhenEntityNotFound_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CRMCorporateCustomer?)null!);

        var result = await _handler.Handle(new UpdateCorporateCustomerCommand(1, BuildUpdateRequest()), CancellationToken.None);

        Assert.Null(result);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CRMCorporateCustomer>(), It.IsAny<CancellationToken>()), Times.Never);
        _auditServiceMock.Verify(a => a.InsertAuditAsync(
            It.IsAny<AuditInsertRequest>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenEntityFound_UpdatesAndAudits()
    {
        var entity = new CRMCorporateCustomer();
        var request = BuildUpdateRequest();

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _referenceValidationMock
            .Setup(x => x.EnsureStatusAndCountryAvailableAsync(request.StatusId, request.CountryId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map(It.IsAny<UpdateCorporateCustomerRequest>(), entity))
            .Returns(entity);
        _mapperMock.Setup(m => m.Map<CorporateCustomerResponse>(entity))
            .Returns(TestResponseFactory.Create<CorporateCustomerResponse>());

        var result = await _handler.Handle(new UpdateCorporateCustomerCommand(1, request), CancellationToken.None);

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
        var entity = new CRMCorporateCustomer();
        var request = BuildUpdateRequest();

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _referenceValidationMock
            .Setup(x => x.EnsureStatusAndCountryAvailableAsync(request.StatusId, request.CountryId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map(It.IsAny<UpdateCorporateCustomerRequest>(), entity))
            .Returns(entity);
        _repositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(new UpdateCorporateCustomerCommand(1, request), CancellationToken.None));

        _repositoryMock.Verify(r => r.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static UpdateCorporateCustomerRequest BuildUpdateRequest() => new(
            TenantId: 1,
            Code: "C001",
            Email: "a@b.com",
            Phone: "123",
            StatusId: 1,
            LegalName: "Corp",
            TaxIdentification: "TAX999"
        );
}


