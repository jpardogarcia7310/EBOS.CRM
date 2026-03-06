using EBOS.CRM.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.Application.Features.CRM.Customer.Commands.UpdateCustomer;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using Moq;
using CRMCustomer = EBOS.CRM.Domain.Entities.CRM.Customer;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Customer.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandlerTest
{
    private readonly Mock<ICustomerRepository> _repositoryMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ICustomerReferenceValidationService> _referenceValidationMock;
    private readonly UpdateCustomerCommandHandler _handler;

    public UpdateCustomerCommandHandlerTest()
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

        _handler = new UpdateCustomerCommandHandler(
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
            .ReturnsAsync((CRMCustomer?)null!);

        var result = await _handler.Handle(new UpdateCustomerCommand(1, BuildUpdateRequest()), CancellationToken.None);

        Assert.Null(result);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CRMCustomer>(), It.IsAny<CancellationToken>()), Times.Never);
        _auditServiceMock.Verify(a => a.InsertAuditAsync(
            It.IsAny<AuditInsertRequest>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenEntityFound_UpdatesAndAudits()
    {
        var entity = new CRMCustomer();
        var request = BuildUpdateRequest();

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _referenceValidationMock
            .Setup(x => x.EnsureStatusAndCountryAvailableAsync(request.StatusId, request.CountryId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map(It.IsAny<UpdateCustomerRequest>(), entity))
            .Returns(entity);
        _mapperMock.Setup(m => m.Map<CustomerResponse>(entity))
            .Returns(TestResponseFactory.Create<CustomerResponse>());

        var result = await _handler.Handle(new UpdateCustomerCommand(1, request), CancellationToken.None);

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
        var entity = new CRMCustomer();
        var request = BuildUpdateRequest();

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _referenceValidationMock
            .Setup(x => x.EnsureStatusAndCountryAvailableAsync(request.StatusId, request.CountryId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map(It.IsAny<UpdateCustomerRequest>(), entity))
            .Returns(entity);
        _repositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(new UpdateCustomerCommand(1, request), CancellationToken.None));

        _repositoryMock.Verify(r => r.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static UpdateCustomerRequest BuildUpdateRequest() => new(
            Id: 1,
            TenantId: 1,
            Code: "C001",
            Email: "a@b.com",
            Phone: "123",
            StatusId: 1
        );
}



