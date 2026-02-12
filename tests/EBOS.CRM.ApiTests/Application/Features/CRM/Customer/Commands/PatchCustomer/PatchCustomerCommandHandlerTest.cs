using EBOS.CRM.Application.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Application.Contracts.Requests.Services;
using EBOS.CRM.Application.Contracts.Responses.Services;
using EBOS.CRM.Application.Features.CRM.Customer.Commands.PatchCustomer;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using Moq;
using CRMCustomer = EBOS.CRM.Domain.Entities.CRM.Customer;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Customer.Commands.PatchCustomer;

public class PatchCustomerCommandHandlerTest
{
    private readonly Mock<ICustomerRepository> _repositoryMock;
    private readonly PatchCustomerCommandHandler _handler;

    public PatchCustomerCommandHandlerTest()
    {
        _repositoryMock = new Mock<ICustomerRepository>();
        var auditServiceMock = new Mock<IAuditService>();
        var currentUserMock = new Mock<ICurrentUserContext>();

        currentUserMock.SetupGet(x => x.UserId).Returns(1);
        currentUserMock.SetupGet(x => x.CorrelationId).Returns("corr-1");

        auditServiceMock.Setup(a => a.InsertAuditAsync(
                It.IsAny<AuditInsertRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        _handler = new PatchCustomerCommandHandler(
            _repositoryMock.Object,
            auditServiceMock.Object,
            currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_DoesNotOverwrite_AuditFields_Or_Tenant()
    {
        var createdAt = DateTime.UtcNow.AddDays(-10);
        var updatedAt = DateTime.UtcNow.AddDays(-1);
        var entity = new CRMCustomer
        {
            Id = 1,
            TenantId = 5,
            Code = "OLD",
            Email = "old@site.com",
            Phone = "111",
            StatusId = 1,
            CreatedAt = createdAt,
            CreatedBy = 10,
            UpdatedAt = updatedAt,
            UpdatedBy = 20
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var request = new PatchCustomerRequest(
            TenantId: 9,
            Code: "NEW",
            Email: null,
            Phone: null,
            StatusId: null);

        var result = await _handler.Handle(new PatchCustomerCommand(1, request), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(5, entity.TenantId);
        Assert.Equal(createdAt, entity.CreatedAt);
        Assert.Equal(10, entity.CreatedBy);
        Assert.Equal(updatedAt, entity.UpdatedAt);
        Assert.Equal(20, entity.UpdatedBy);
    }
}
