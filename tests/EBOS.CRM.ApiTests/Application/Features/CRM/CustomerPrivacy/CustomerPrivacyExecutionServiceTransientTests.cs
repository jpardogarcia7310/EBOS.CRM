using EBOS.CRM.Application.Features.CRM.CustomerPrivacy;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerPrivacy;

public class CustomerPrivacyExecutionServiceTransientTests
{
    [Fact]
    public async Task ExecuteAsync_WhenTimeoutOnCustomerLookup_ThrowsTransientDomainFailure()
    {
        var customerRepo = new Mock<ICustomerRepository>();
        var corporateRepo = new Mock<ICorporateCustomerRepository>();
        var individualRepo = new Mock<IIndividualCustomerRepository>();
        var privacyRepo = new Mock<ICustomerPrivacyRequestRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-transient");
        customerRepo.Setup(x => x.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException("simulated timeout"));
        privacyRepo.Setup(x => x.UpdateAsync(It.IsAny<CustomerPrivacyRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        privacyRepo.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        audit.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var sut = new CustomerPrivacyExecutionService(
            customerRepo.Object,
            corporateRepo.Object,
            individualRepo.Object,
            privacyRepo.Object,
            audit.Object,
            currentUser.Object);

        var request = CustomerPrivacyRequest.Create(
            tenantId: 1,
            customerId: 10,
            requestType: CustomerPrivacyRequest.TypeAnonymize,
            requestedBy: 1,
            reason: null,
            correlationId: "corr-transient");

        var act = () => sut.ExecuteAsync(request, CancellationToken.None);

        var ex = await Assert.ThrowsAsync<TransientDomainFailureException>(act);
        Assert.Equal("DOMAIN_TRANSIENT_TIMEOUT", ex.Code);
    }
}
