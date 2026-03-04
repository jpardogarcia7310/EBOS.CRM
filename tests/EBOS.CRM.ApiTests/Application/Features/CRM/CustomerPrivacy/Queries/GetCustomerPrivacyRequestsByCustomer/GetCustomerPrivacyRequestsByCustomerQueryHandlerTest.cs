using EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestsByCustomer;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestsByCustomer;

public class GetCustomerPrivacyRequestsByCustomerQueryHandlerTest
{
    [Fact]
    public async Task Handle_ReturnsPaged()
    {
        var repo = new Mock<ICustomerPrivacyRequestRepository>();
        var items = new[] { CustomerPrivacyRequest.Create(1, 10, CustomerPrivacyRequest.TypeAnonymize, 1, null, null) };
        repo.Setup(x => x.GetByCustomerPagedAsync(1, 10, 1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(items);
        repo.Setup(x => x.CountByCustomerAsync(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new GetCustomerPrivacyRequestsByCustomerQueryHandler(repo.Object);
        var result = await handler.Handle(new GetCustomerPrivacyRequestsByCustomerQuery(1, 10, 1, 10), CancellationToken.None);

        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
    }
}
