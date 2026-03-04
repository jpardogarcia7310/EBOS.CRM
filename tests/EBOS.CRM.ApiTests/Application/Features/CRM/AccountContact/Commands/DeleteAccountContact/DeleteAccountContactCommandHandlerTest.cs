using EBOS.CRM.Application.Features.CRM.AccountContact.Commands.DeleteAccountContact;
using EBOS.CRM.Contracts.Requests.CRM.AccountContact;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using Moq;
using CRMAccountContact = EBOS.CRM.Domain.Entities.CRM.AccountContact;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContact.Commands.DeleteAccountContact;

public class DeleteAccountContactCommandHandlerTest
{
    [Fact]
    public async Task Handle_EntityExists_DeletesAndReturnsTrue()
    {
        var repository = new Mock<IAccountContactRepository>();
        var auditService = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        auditService.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var entity = CRMAccountContact.Create(1, 20, 30, false, DateTime.UtcNow, null, 1);
        entity.Id = 10;
        repository.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var handler = new DeleteAccountContactCommandHandler(repository.Object, auditService.Object, currentUser.Object);
        var result = await handler.Handle(new DeleteAccountContactCommand(10, new DeleteAccountContactRequest(1)),
            CancellationToken.None);

        Assert.True(result);
        repository.Verify(x => x.DeleteAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EntityNotFound_ReturnsFalse()
    {
        var repository = new Mock<IAccountContactRepository>();
        var auditService = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        repository.Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((CRMAccountContact?)null);

        var handler = new DeleteAccountContactCommandHandler(repository.Object, auditService.Object, currentUser.Object);
        var result = await handler.Handle(new DeleteAccountContactCommand(999, new DeleteAccountContactRequest(1)),
            CancellationToken.None);

        Assert.False(result);
    }
}
