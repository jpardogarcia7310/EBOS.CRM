using EBOS.CRM.Application.Features.CRM.CustomerPrivacy;
using EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Commands.RegisterCustomerPrivacyRequest;
using EBOS.CRM.Contracts.Requests.CRM.CustomerPrivacy;
using EBOS.CRM.Domain.Entities.CRM;
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
            CustomerPrivacyTestHelper.BuildExecutionService());

        var act = () => handler.Handle(new RegisterCustomerPrivacyRequestCommand(
            new RegisterCustomerPrivacyRequestRequest(1, 10, "ANONYMIZE", null, false)), CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }
}
