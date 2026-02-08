using EBOS.CRM.Application.Contracts.Requests.CRM.TaxInformation;
using EBOS.CRM.Application.Contracts.Responses.Services;
using EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.PatchTaxInformation;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using Moq;
using CRMTaxInformation = EBOS.CRM.Domain.Entities.CRM.TaxInformation;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.TaxInformation.Commands.PatchTaxInformation;

public class PatchTaxInformationCommandHandlerTest
{
    private readonly Mock<ITaxInformationRepository> _repositoryMock;
    private readonly PatchTaxInformationCommandHandler _handler;

    public PatchTaxInformationCommandHandlerTest()
    {
        _repositoryMock = new Mock<ITaxInformationRepository>();
        var auditServiceMock = new Mock<IAuditService>();
        var currentUserMock = new Mock<ICurrentUserContext>();

        currentUserMock.SetupGet(x => x.UserId).Returns(1);
        currentUserMock.SetupGet(x => x.CorrelationId).Returns("corr-1");

        auditServiceMock.Setup(a => a.InsertAuditAsync(
                It.IsAny<EBOS.CRM.Application.Contracts.Requests.Services.AuditInsertRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        _handler = new PatchTaxInformationCommandHandler(
            _repositoryMock.Object,
            auditServiceMock.Object,
            currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_DoesNotOverwrite_AuditFields_Or_Tenant()
    {
        var createdAt = DateTime.UtcNow.AddDays(-7);
        var updatedAt = DateTime.UtcNow.AddDays(-3);
        var entity = new CRMTaxInformation
        {
            Id = 1,
            TenantId = 5,
            TaxName = "Old",
            TaxIdentificationNumber = "123",
            CustomerId = 10,
            CreatedAt = createdAt,
            CreatedBy = 10,
            UpdatedAt = updatedAt,
            UpdatedBy = 20
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var request = new PatchTaxInformationRequest(
            TenantId: 9,
            TaxName: "New",
            TaxIdentificationNumber: null,
            CustomerId: null);

        var result = await _handler.Handle(new PatchTaxInformationCommand(1, request), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(5, entity.TenantId);
        Assert.Equal(createdAt, entity.CreatedAt);
        Assert.Equal(10, entity.CreatedBy);
        Assert.Equal(updatedAt, entity.UpdatedAt);
        Assert.Equal(20, entity.UpdatedBy);
    }
}
