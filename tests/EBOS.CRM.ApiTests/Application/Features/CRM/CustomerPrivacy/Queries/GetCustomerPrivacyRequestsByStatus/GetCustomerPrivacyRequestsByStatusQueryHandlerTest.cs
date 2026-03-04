using EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestsByStatus;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestsByStatus;

public class GetCustomerPrivacyRequestsByStatusQueryHandlerTest
{
    [Fact]
    public async Task Handle_ReturnsPaged()
    {
        var repo = new Mock<ICustomerPrivacyRequestRepository>();
        var items = new[] { CustomerPrivacyRequest.Create(1, 10, CustomerPrivacyRequest.TypeAnonymize, 1, null, null) };
        repo.Setup(x => x.GetByStatusPagedAsync(1, CustomerPrivacyRequest.StatusPending, 1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(items);
        repo.Setup(x => x.CountByStatusAsync(1, CustomerPrivacyRequest.StatusPending, It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new GetCustomerPrivacyRequestsByStatusQueryHandler(repo.Object);
        var result = await handler.Handle(new GetCustomerPrivacyRequestsByStatusQuery(1, CustomerPrivacyRequest.StatusPending, 1, 10), CancellationToken.None);

        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
    }
}
