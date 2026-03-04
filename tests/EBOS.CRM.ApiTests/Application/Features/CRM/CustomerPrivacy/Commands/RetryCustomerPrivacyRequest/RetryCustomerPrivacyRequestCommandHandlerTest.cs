using EBOS.CRM.Application.Features.CRM.CustomerPrivacy;
using EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Commands.RetryCustomerPrivacyRequest;
using EBOS.CRM.Contracts.Requests.CRM.CustomerPrivacy;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerPrivacy.Commands.RetryCustomerPrivacyRequest;

public class RetryCustomerPrivacyRequestCommandHandlerTest
{
    [Fact]
    public async Task Handle_WhenCompleted_ReturnsResponseWithoutExecution()
    {
        var repo = new Mock<ICustomerPrivacyRequestRepository>();
        var entity = CustomerPrivacyRequest.Create(1, 10, CustomerPrivacyRequest.TypeAnonymize, 1, null, null);
        entity.MarkInProgress(1);
        entity.MarkCompleted(1);
        repo.Setup(x => x.GetByIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var handler = new RetryCustomerPrivacyRequestCommandHandler(
            repo.Object,
            CustomerPrivacyTestHelper.BuildExecutionService(),
            CustomerPrivacyTestHelper.BuildCurrentUser().Object);

        var result = await handler.Handle(new RetryCustomerPrivacyRequestCommand(7, new RetryCustomerPrivacyRequestRequest(1, "retry")), CancellationToken.None);

        Assert.NotNull(result);
    }
}
