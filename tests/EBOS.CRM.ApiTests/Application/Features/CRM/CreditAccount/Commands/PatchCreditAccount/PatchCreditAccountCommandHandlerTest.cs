using EBOS.CRM.Contracts.Requests.CRM.CreditAccount;
using EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.PatchCreditAccount;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using Moq;
using CRMCreditAccount = EBOS.CRM.Domain.Entities.CRM.CreditAccount;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CreditAccount.Commands.PatchCreditAccount;

public class PatchCreditAccountCommandHandlerTest
{
    private readonly Mock<ICreditAccountRepository> _repositoryMock;
    private readonly PatchCreditAccountCommandHandler _handler;

    public PatchCreditAccountCommandHandlerTest()
    {
        _repositoryMock = new Mock<ICreditAccountRepository>();
        var auditServiceMock = new Mock<IAuditService>();
        var currentUserMock = new Mock<ICurrentUserContext>();

        currentUserMock.SetupGet(x => x.UserId).Returns(1);
        currentUserMock.SetupGet(x => x.CorrelationId).Returns("corr-1");

        auditServiceMock.Setup(a => a.InsertAuditAsync(
                It.IsAny<AuditInsertRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        _handler = new PatchCreditAccountCommandHandler(
            _repositoryMock.Object,
            auditServiceMock.Object,
            currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_DoesNotOverwrite_AuditFields_Or_Tenant()
    {
        var createdAt = DateTime.UtcNow.AddDays(-6);
        var updatedAt = DateTime.UtcNow.AddDays(-2);
        var entity = new CRMCreditAccount
        {
            Id = 1,
            TenantId = 5,
            MaxAmount = 100,
            UsedAmount = 10,
            CustomerId = 10,
            CreatedAt = createdAt,
            CreatedBy = 10,
            UpdatedAt = updatedAt,
            UpdatedBy = 20
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var request = new PatchCreditAccountRequest(
            TenantId: 9,
            MaxAmount: 200,
            UsedAmount: null,
            CustomerId: null);

        var result = await _handler.Handle(new PatchCreditAccountCommand(1, request), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(5, entity.TenantId);
        Assert.Equal(createdAt, entity.CreatedAt);
        Assert.Equal(10, entity.CreatedBy);
        Assert.Equal(updatedAt, entity.UpdatedAt);
        Assert.Equal(20, entity.UpdatedBy);
    }
}
