using EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestById;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestById;

public class GetCustomerPrivacyRequestByIdQueryHandlerTest
{
    [Fact]
    public async Task Handle_WhenTenantMatches_ReturnsResponse()
    {
        var repo = new Mock<ICustomerPrivacyRequestRepository>();
        var entity = CustomerPrivacyRequest.Create(1, 10, CustomerPrivacyRequest.TypeAnonymize, 1, null, null);
        repo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var handler = new GetCustomerPrivacyRequestByIdQueryHandler(repo.Object);
        var result = await handler.Handle(new GetCustomerPrivacyRequestByIdQuery(1, 1), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.TenantId);
    }
}
