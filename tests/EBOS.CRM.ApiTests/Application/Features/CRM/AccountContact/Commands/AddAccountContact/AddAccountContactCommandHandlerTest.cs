using EBOS.CRM.Application.Features.CRM.AccountContact.Commands.AddAccountContact;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.CRM.AccountContact;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using Moq;
using CRMAccountContact = EBOS.CRM.Domain.Entities.CRM.AccountContact;
using CRMCorporateCustomer = EBOS.CRM.Domain.Entities.CRM.CorporateCustomer;
using CRMIndividualCustomer = EBOS.CRM.Domain.Entities.CRM.IndividualCustomer;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContact.Commands.AddAccountContact;

public class AddAccountContactCommandHandlerTest
{
    [Fact]
    public async Task Handle_ValidRequest_PersistsAndReturnsMappedResponse()
    {
        var repository = new Mock<IAccountContactRepository>();
        var corporateRepo = new Mock<ICorporateCustomerRepository>();
        var individualRepo = new Mock<IIndividualCustomerRepository>();
        var primaryGuard = new Mock<IAccountContactPrimaryGuard>();
        var auditService = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        auditService.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        corporateRepo.Setup(x => x.GetByIdAsync(20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CRMCorporateCustomer { Id = 20, TenantId = 1, LegalName = "Corp", TaxIdentification = "X" });
        individualRepo.Setup(x => x.GetByIdAsync(30, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CRMIndividualCustomer { Id = 30, TenantId = 1, FirstName = "A", LastName = "B" });
        primaryGuard.Setup(x => x.GetOtherPrimariesAsync(1, 20, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<CRMAccountContact>());

        mapper.Setup(x => x.Map<AccountContactResponse>(It.IsAny<CRMAccountContact>()))
            .Returns((CRMAccountContact e) => new AccountContactResponse(
                e.Id, e.TenantId, e.CorporateCustomerId, e.IndividualCustomerId, e.IsPrimary, e.StartAt, e.EndAt, true));

        var handler = new AddAccountContactCommandHandler(
            repository.Object, corporateRepo.Object, individualRepo.Object, primaryGuard.Object,
            auditService.Object, currentUser.Object, mapper.Object);

        var command = new AddAccountContactCommand(new AddAccountContactRequest(1, 20, 30, true, DateTime.UtcNow, null));
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        repository.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.AddAsync(It.IsAny<CRMAccountContact>(), It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CorporateTenantMismatch_Throws()
    {
        var repository = new Mock<IAccountContactRepository>();
        var corporateRepo = new Mock<ICorporateCustomerRepository>();
        var individualRepo = new Mock<IIndividualCustomerRepository>();
        var primaryGuard = new Mock<IAccountContactPrimaryGuard>();
        var auditService = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();

        corporateRepo.Setup(x => x.GetByIdAsync(20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CRMCorporateCustomer { Id = 20, TenantId = 99, LegalName = "Corp", TaxIdentification = "X" });
        individualRepo.Setup(x => x.GetByIdAsync(30, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CRMIndividualCustomer { Id = 30, TenantId = 1, FirstName = "A", LastName = "B" });

        var handler = new AddAccountContactCommandHandler(
            repository.Object, corporateRepo.Object, individualRepo.Object, primaryGuard.Object,
            auditService.Object, currentUser.Object, mapper.Object);

        var command = new AddAccountContactCommand(new AddAccountContactRequest(1, 20, 30, false, DateTime.UtcNow, null));
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }
}
