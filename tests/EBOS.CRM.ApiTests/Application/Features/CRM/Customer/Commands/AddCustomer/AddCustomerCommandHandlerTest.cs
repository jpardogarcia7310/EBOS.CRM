using EBOS.CRM.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Application.Features.CRM.Customer.Commands.AddCustomer;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using Moq;
using CRMCustomer = EBOS.CRM.Domain.Entities.CRM.Customer;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Customer.Commands.AddCustomer;

public class AddCustomerCommandHandlerTest
{
    private readonly Mock<ICustomerRepository> _repositoryMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ICustomerReferenceValidationService> _referenceValidationMock;
    private readonly AddCustomerCommandHandler _handler;

    public AddCustomerCommandHandlerTest()
    {
        _repositoryMock = new Mock<ICustomerRepository>();
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

        _handler = new AddCustomerCommandHandler(
            _repositoryMock.Object,
            _auditServiceMock.Object,
            currentUserMock.Object,
            _mapperMock.Object,
            _referenceValidationMock.Object);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_AddsAndAudits()
    {
        var request = BuildAddRequest();
        var entity = new CRMCustomer();
        _referenceValidationMock
            .Setup(x => x.EnsureStatusAndCountryAvailableAsync(request.StatusId, request.CountryId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mapperMock.Setup(m => m.Map<CRMCustomer>(request)).Returns(entity);
        _mapperMock.Setup(m => m.Map<CustomerResponse>(entity)).Returns(TestResponseFactory.Create<CustomerResponse>());

        var result = await _handler.Handle(new AddCustomerCommand(request), CancellationToken.None);

        Assert.NotNull(result);
        _repositoryMock.Verify(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(a => a.InsertAuditAsync(
            It.IsAny<AuditInsertRequest>(),
            It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSaveChangesThrows_RollsBack()
    {
        var request = BuildAddRequest();
        var entity = new CRMCustomer();
        _referenceValidationMock
            .Setup(x => x.EnsureStatusAndCountryAvailableAsync(request.StatusId, request.CountryId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mapperMock.Setup(m => m.Map<CRMCustomer>(request)).Returns(entity);
        _repositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(new AddCustomerCommand(request), CancellationToken.None));

        _repositoryMock.Verify(r => r.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenReferenceResolutionIsTransient_ThrowsTransientDomainFailure()
    {
        var request = BuildAddRequest();
        _referenceValidationMock
            .Setup(x => x.EnsureStatusAndCountryAvailableAsync(request.StatusId, request.CountryId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TransientDomainFailureException(
                "Transient failure while resolving customer references.",
                "DOMAIN_TRANSIENT_CUSTOMER_REFERENCE_RESOLUTION"));

        var ex = await Assert.ThrowsAsync<TransientDomainFailureException>(() =>
            _handler.Handle(new AddCustomerCommand(request), CancellationToken.None));

        Assert.Equal("DOMAIN_TRANSIENT_CUSTOMER_REFERENCE_RESOLUTION", ex.Code);
    }

    private static AddCustomerRequest BuildAddRequest() => new(
            TenantId: 1,
            Code: "C001",
            Email: "a@b.com",
            Phone: "123",
            StatusId: 1
        );
}


