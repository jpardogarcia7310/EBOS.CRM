using EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.GetCustomerMergeHistoryByWinner;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerMerge.Queries.GetCustomerMergeHistoryByWinner;

public class GetCustomerMergeHistoryByWinnerQueryHandlerTest
{
    [Fact]
    public async Task Handle_ReturnsPagedResult()
    {
        var repo = new Mock<ICustomerMergeHistoryRepository>();
        var items = new[] { CustomerMergeHistory.Create(1, 10, 11, "dedupe", 1) };
        repo.Setup(x => x.GetByWinnerPagedAsync(1, 10, 1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(items);
        repo.Setup(x => x.CountByWinnerAsync(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new GetCustomerMergeHistoryByWinnerQueryHandler(repo.Object);
        var result = await handler.Handle(new GetCustomerMergeHistoryByWinnerQuery(1, 10, 1, 10), CancellationToken.None);

        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
        Assert.Equal(10, result.Items.First().WinnerCustomerId);
    }
}
