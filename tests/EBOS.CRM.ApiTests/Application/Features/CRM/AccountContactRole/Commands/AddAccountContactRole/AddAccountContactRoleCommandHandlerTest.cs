using EBOS.CRM.Application.Features.CRM.AccountContactRole.Commands.AddAccountContactRole;
using EBOS.CRM.Contracts.Requests.CRM.AccountContactRole;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using Moq;
using CRMAccountContact = EBOS.CRM.Domain.Entities.CRM.AccountContact;
using CRMAccountContactRole = EBOS.CRM.Domain.Entities.CRM.AccountContactRole;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContactRole.Commands.AddAccountContactRole;

public class AddAccountContactRoleCommandHandlerTest
{
    [Fact]
    public async Task Handle_ValidRequest_PersistsAndReturnsResponse()
    {
        var repository = new Mock<IAccountContactRoleRepository>();
        var accountContactRepository = new Mock<IAccountContactRepository>();
        var primaryGuard = new Mock<IAccountContactRolePrimaryGuard>();
        var auditService = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        auditService.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var contact = CRMAccountContact.Create(1, 20, 30, false, DateTime.UtcNow, null, 1);
        contact.Id = 10;
        accountContactRepository.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(contact);
        primaryGuard.Setup(x => x.GetOtherPrimariesAsync(1, 10, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<CRMAccountContactRole>());

        mapper.Setup(x => x.Map<AccountContactRoleResponse>(It.IsAny<CRMAccountContactRole>()))
            .Returns((CRMAccountContactRole e) =>
                new AccountContactRoleResponse(e.Id, e.TenantId, e.AccountContactId, e.RoleCode, e.IsPrimary, e.ValidFrom, e.ValidTo, true));

        var handler = new AddAccountContactRoleCommandHandler(
            repository.Object, accountContactRepository.Object, primaryGuard.Object, auditService.Object, currentUser.Object, mapper.Object);

        var result = await handler.Handle(
            new AddAccountContactRoleCommand(new AddAccountContactRoleRequest(1, 10, "OWNER", true, DateTime.UtcNow, null)),
            CancellationToken.None);

        Assert.NotNull(result);
        repository.Verify(x => x.AddAsync(It.IsAny<CRMAccountContactRole>(), It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
