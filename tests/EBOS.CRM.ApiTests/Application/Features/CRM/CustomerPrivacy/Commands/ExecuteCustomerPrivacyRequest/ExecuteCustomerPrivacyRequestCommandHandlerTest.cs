using EBOS.CRM.Application.Features.CRM.CustomerPrivacy;
using EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Commands.ExecuteCustomerPrivacyRequest;
using EBOS.CRM.Application.Shared.Observability;
using EBOS.CRM.Contracts.Requests.CRM.CustomerPrivacy;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerPrivacy.Commands.ExecuteCustomerPrivacyRequest;

public class ExecuteCustomerPrivacyRequestCommandHandlerTest
{
    [Fact]
    public async Task Handle_WhenNotFound_ReturnsNull()
    {
        var repo = new Mock<ICustomerPrivacyRequestRepository>();
        repo.Setup(x => x.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync((CustomerPrivacyRequest?)null);
        var handler = new ExecuteCustomerPrivacyRequestCommandHandler(
            repo.Object,
            CustomerPrivacyTestHelper.BuildExecutionService(),
            new Mock<IDomainOperationalEventPublisher>().Object);

        var result = await handler.Handle(new ExecuteCustomerPrivacyRequestCommand(5, new ExecuteCustomerPrivacyRequestRequest(1)), CancellationToken.None);

        Assert.Null(result);
    }
}
