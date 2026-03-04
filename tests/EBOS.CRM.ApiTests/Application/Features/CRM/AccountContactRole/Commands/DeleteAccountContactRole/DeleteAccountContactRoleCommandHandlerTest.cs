using EBOS.CRM.Application.Features.CRM.AccountContactRole.Commands.DeleteAccountContactRole;
using EBOS.CRM.Contracts.Requests.CRM.AccountContactRole;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using Moq;
using CRMAccountContactRole = EBOS.CRM.Domain.Entities.CRM.AccountContactRole;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContactRole.Commands.DeleteAccountContactRole;

public class DeleteAccountContactRoleCommandHandlerTest
{
    [Fact]
    public async Task Handle_WhenFound_DeletesAndReturnsTrue()
    {
        var repository = new Mock<IAccountContactRoleRepository>();
        var auditService = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        auditService.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var entity = CRMAccountContactRole.Create(1, 10, "OWNER", false, DateTime.UtcNow, null);
        entity.Id = 5;
        repository.Setup(x => x.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var handler = new DeleteAccountContactRoleCommandHandler(repository.Object, auditService.Object, currentUser.Object);
        var result = await handler.Handle(
            new DeleteAccountContactRoleCommand(5, new DeleteAccountContactRoleRequest(1)),
            CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ReturnsFalse()
    {
        var repository = new Mock<IAccountContactRoleRepository>();
        var auditService = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        repository.Setup(x => x.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CRMAccountContactRole?)null);

        var handler = new DeleteAccountContactRoleCommandHandler(repository.Object, auditService.Object, currentUser.Object);
        var result = await handler.Handle(
            new DeleteAccountContactRoleCommand(5, new DeleteAccountContactRoleRequest(1)),
            CancellationToken.None);

        Assert.False(result);
    }
}
