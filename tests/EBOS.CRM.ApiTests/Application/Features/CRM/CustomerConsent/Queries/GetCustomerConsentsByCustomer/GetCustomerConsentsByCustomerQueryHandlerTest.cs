using EBOS.CRM.Application.Features.CRM.CustomerConsent.Queries.GetCustomerConsentsByCustomer;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;
using CRMConsent = EBOS.CRM.Domain.Entities.CRM.CustomerConsent;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerConsent.Queries.GetCustomerConsentsByCustomer;

public class GetCustomerConsentsByCustomerQueryHandlerTest
{
    [Fact]
    public async Task Handle_ReturnsPagedResult()
    {
        var repository = new Mock<ICustomerConsentRepository>();
        var mapper = new Mock<IMapper>();

        var page = new List<CRMConsent> { CRMConsent.Create(1, 2, "EMAIL", true, DateTime.UtcNow, "api", null) };
        repository.Setup(x => x.GetLatestByCustomerPagedAsync(1, 2, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(page);
        repository.Setup(x => x.CountLatestByCustomerAsync(1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        mapper.Setup(x => x.Map<IReadOnlyCollection<CustomerConsentResponse>>(page))
            .Returns(new[] { new CustomerConsentResponse(1, 1, 2, "EMAIL", true, DateTime.UtcNow, "api", null, null, true) });

        var handler = new GetCustomerConsentsByCustomerQueryHandler(repository.Object, mapper.Object);
        var result = await handler.Handle(new GetCustomerConsentsByCustomerQuery(1, 2, 1, 10), CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(1, result.Total);
    }
}
