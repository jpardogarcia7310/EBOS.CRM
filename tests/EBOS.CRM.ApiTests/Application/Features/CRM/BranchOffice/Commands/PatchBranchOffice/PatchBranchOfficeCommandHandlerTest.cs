using EBOS.CRM.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.PatchBranchOffice;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using Moq;
using CRMBranchOffice = EBOS.CRM.Domain.Entities.CRM.BranchOffice;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BranchOffice.Commands.PatchBranchOffice;

public class PatchBranchOfficeCommandHandlerTest
{
    private readonly Mock<IBranchOfficeRepository> _repositoryMock;
    private readonly PatchBranchOfficeCommandHandler _handler;

    public PatchBranchOfficeCommandHandlerTest()
    {
        _repositoryMock = new Mock<IBranchOfficeRepository>();
        var auditServiceMock = new Mock<IAuditService>();
        var currentUserMock = new Mock<ICurrentUserContext>();

        currentUserMock.SetupGet(x => x.UserId).Returns(1);
        currentUserMock.SetupGet(x => x.CorrelationId).Returns("corr-1");

        auditServiceMock.Setup(a => a.InsertAuditAsync(
                It.IsAny<AuditInsertRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        _handler = new PatchBranchOfficeCommandHandler(
            _repositoryMock.Object,
            auditServiceMock.Object,
            currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_DoesNotOverwrite_AuditFields_Or_Tenant()
    {
        var createdAt = DateTime.UtcNow.AddDays(-5);
        var updatedAt = DateTime.UtcNow.AddDays(-1);
        var entity = new CRMBranchOffice
        {
            Id = 1,
            TenantId = 5,
            Name = "Old",
            PhoneNumber = "111",
            CorporateCustomerId = 10,
            CreatedAt = createdAt,
            CreatedBy = 10,
            UpdatedAt = updatedAt,
            UpdatedBy = 20
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var request = new PatchBranchOfficeRequest(
            TenantId: 9,
            Name: "New",
            PhoneNumber: null,
            CorporateCustomerId: null);

        var result = await _handler.Handle(new PatchBranchOfficeCommand(1, request), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(5, entity.TenantId);
        Assert.Equal(createdAt, entity.CreatedAt);
        Assert.Equal(10, entity.CreatedBy);
        Assert.Equal(updatedAt, entity.UpdatedAt);
        Assert.Equal(20, entity.UpdatedBy);
    }
}
