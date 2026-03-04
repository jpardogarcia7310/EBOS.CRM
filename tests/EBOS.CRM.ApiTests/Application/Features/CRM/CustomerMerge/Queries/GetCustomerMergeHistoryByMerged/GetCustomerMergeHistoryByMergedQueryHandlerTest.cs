using EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.GetCustomerMergeHistoryByMerged;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerMerge.Queries.GetCustomerMergeHistoryByMerged;

public class GetCustomerMergeHistoryByMergedQueryHandlerTest
{
    [Fact]
    public async Task Handle_ReturnsPagedResult()
    {
        var repo = new Mock<ICustomerMergeHistoryRepository>();
        var items = new[] { CustomerMergeHistory.Create(1, 10, 11, "dedupe", 1) };
        repo.Setup(x => x.GetByMergedPagedAsync(1, 11, 1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(items);
        repo.Setup(x => x.CountByMergedAsync(1, 11, It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new GetCustomerMergeHistoryByMergedQueryHandler(repo.Object);
        var result = await handler.Handle(new GetCustomerMergeHistoryByMergedQuery(1, 11, 1, 10), CancellationToken.None);

        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
        Assert.Equal(11, result.Items.First().MergedCustomerId);
    }
}
