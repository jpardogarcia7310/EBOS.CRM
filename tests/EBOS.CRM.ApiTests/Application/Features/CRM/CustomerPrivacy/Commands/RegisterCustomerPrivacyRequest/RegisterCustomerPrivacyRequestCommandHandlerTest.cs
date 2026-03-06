using EBOS.CRM.Application.Features.CRM.CustomerPrivacy;
using EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Commands.RegisterCustomerPrivacyRequest;
using EBOS.CRM.Application.Shared.Observability;
using EBOS.CRM.Contracts.Requests.CRM.CustomerPrivacy;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerPrivacy.Commands.RegisterCustomerPrivacyRequest;

public class RegisterCustomerPrivacyRequestCommandHandlerTest
{
    [Fact]
    public async Task Handle_WhenCustomerNotFound_Throws()
    {
        var repo = new Mock<ICustomerPrivacyRequestRepository>();
        var customerRepo = new Mock<ICustomerRepository>();
        customerRepo.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.Customer?)null);

        var handler = new RegisterCustomerPrivacyRequestCommandHandler(
            repo.Object,
            customerRepo.Object,
            new Mock<IAuditService>().Object,
            CustomerPrivacyTestHelper.BuildCurrentUser().Object,
            CustomerPrivacyTestHelper.BuildExecutionService(),
            new Mock<IDomainOperationalEventPublisher>().Object);

        var act = () => handler.Handle(new RegisterCustomerPrivacyRequestCommand(
            new RegisterCustomerPrivacyRequestRequest(1, 10, "ANONYMIZE", null, false)), CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    [Fact]
    public async Task Handle_WhenEquivalentActiveRequestExists_ReturnsExistingIdempotently()
    {
        var repo = new Mock<ICustomerPrivacyRequestRepository>();
        var customerRepo = new Mock<ICustomerRepository>();
        customerRepo.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new global::EBOS.CRM.Domain.Entities.CRM.Customer { Id = 10, TenantId = 1 });

        var active = CustomerPrivacyRequest.Create(
            tenantId: 1,
            customerId: 10,
            requestType: "ANONYMIZE",
            requestedBy: 1,
            reason: "same",
            correlationId: "corr");
        repo.Setup(x => x.GetActiveByCustomerAndTypeAsync(1, 10, "ANONYMIZE", It.IsAny<CancellationToken>()))
            .ReturnsAsync(active);

        var handler = new RegisterCustomerPrivacyRequestCommandHandler(
            repo.Object,
            customerRepo.Object,
            new Mock<IAuditService>().Object,
            CustomerPrivacyTestHelper.BuildCurrentUser().Object,
            CustomerPrivacyTestHelper.BuildExecutionService(),
            new Mock<IDomainOperationalEventPublisher>().Object);

        var response = await handler.Handle(
            new RegisterCustomerPrivacyRequestCommand(
                new RegisterCustomerPrivacyRequestRequest(1, 10, "ANONYMIZE", "same", false)),
            CancellationToken.None);

        Assert.Equal(10, response.CustomerId);
        repo.Verify(x => x.AddAsync(It.IsAny<CustomerPrivacyRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDifferentActiveRequestExists_ThrowsDomainConflict()
    {
        var repo = new Mock<ICustomerPrivacyRequestRepository>();
        var customerRepo = new Mock<ICustomerRepository>();
        customerRepo.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new global::EBOS.CRM.Domain.Entities.CRM.Customer { Id = 10, TenantId = 1 });

        var active = CustomerPrivacyRequest.Create(
            tenantId: 1,
            customerId: 10,
            requestType: "ANONYMIZE",
            requestedBy: 1,
            reason: "original-reason",
            correlationId: "corr");
        repo.Setup(x => x.GetActiveByCustomerAndTypeAsync(1, 10, "ANONYMIZE", It.IsAny<CancellationToken>()))
            .ReturnsAsync(active);

        var handler = new RegisterCustomerPrivacyRequestCommandHandler(
            repo.Object,
            customerRepo.Object,
            new Mock<IAuditService>().Object,
            CustomerPrivacyTestHelper.BuildCurrentUser().Object,
            CustomerPrivacyTestHelper.BuildExecutionService(),
            new Mock<IDomainOperationalEventPublisher>().Object);

        var act = () => handler.Handle(
            new RegisterCustomerPrivacyRequestCommand(
                new RegisterCustomerPrivacyRequestRequest(1, 10, "ANONYMIZE", "different-reason", false)),
            CancellationToken.None);

        await Assert.ThrowsAsync<DomainConflictException>(act);
    }
}
