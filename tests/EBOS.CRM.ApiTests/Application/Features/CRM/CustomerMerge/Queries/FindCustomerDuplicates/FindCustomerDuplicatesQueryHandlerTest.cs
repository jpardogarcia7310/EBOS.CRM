using EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.FindCustomerDuplicates;
using EBOS.CRM.Contracts.Requests.CRM.CustomerMerge;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM.Models;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerMerge.Queries.FindCustomerDuplicates;

public class FindCustomerDuplicatesQueryHandlerTest
{
    [Fact]
    public async Task Handle_WhenCandidatesExist_ReturnsPagedAndRecordsMetrics()
    {
        var repo = new Mock<ICustomerDedupeRepository>();
        var normalization = new Mock<ICustomerDedupeNormalizationService>();
        var metrics = new Mock<ICustomer360Metrics>();

        normalization.Setup(x => x.NormalizeEmail(It.IsAny<string?>())).Returns("a@b.com");
        normalization.Setup(x => x.NormalizePhone(It.IsAny<string?>())).Returns("123");
        normalization.Setup(x => x.NormalizeAlphanumericUpper(It.IsAny<string?>())).Returns("X");

        repo.Setup(x => x.CountDuplicatesAsync(It.IsAny<CustomerDedupeCriteria>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);
        repo.Setup(x => x.FindDuplicatesAsync(It.IsAny<CustomerDedupeCriteria>(), 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new CustomerDuplicateCandidate(10, "EMAIL", 95) });

        var handler = new FindCustomerDuplicatesQueryHandler(repo.Object, normalization.Object, metrics.Object);
        var query = new FindCustomerDuplicatesQuery(new FindCustomerDuplicatesRequest(1, "a@b.com", "123", "x", "x"), 1, 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
        metrics.Verify(x => x.RecordDedupeQuery(1, 1, 95), Times.Once);
    }
}
