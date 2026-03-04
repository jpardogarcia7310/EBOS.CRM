using EBOS.CRM.Application.Features.CRM.CustomerPrivacy;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerPrivacy;

internal static class CustomerPrivacyTestHelper
{
    internal static Mock<ICurrentUserContext> BuildCurrentUser()
    {
        var currentUser = new Mock<ICurrentUserContext>();
        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr");
        return currentUser;
    }

    internal static CustomerPrivacyExecutionService BuildExecutionService()
    {
        var customerRepo = new Mock<ICustomerRepository>();
        var corporateRepo = new Mock<ICorporateCustomerRepository>();
        var individualRepo = new Mock<IIndividualCustomerRepository>();
        var privacyRepo = new Mock<ICustomerPrivacyRequestRepository>();
        var audit = new Mock<IAuditService>();
        audit.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new global::EBOS.CRM.Contracts.Responses.Services.AuditInsertResponse(true, 1));
        return new CustomerPrivacyExecutionService(
            customerRepo.Object,
            corporateRepo.Object,
            individualRepo.Object,
            privacyRepo.Object,
            audit.Object,
            BuildCurrentUser().Object);
    }
}
