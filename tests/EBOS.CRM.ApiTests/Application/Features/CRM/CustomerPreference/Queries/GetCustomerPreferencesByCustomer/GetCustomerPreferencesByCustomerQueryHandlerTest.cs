using EBOS.CRM.Application.Features.CRM.CustomerPreference.Queries.GetCustomerPreferencesByCustomer;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;
using CRMPreference = EBOS.CRM.Domain.Entities.CRM.CustomerPreference;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerPreference.Queries.GetCustomerPreferencesByCustomer;

public class GetCustomerPreferencesByCustomerQueryHandlerTest
{
    [Fact]
    public async Task Handle_ReturnsPagedResult()
    {
        var repository = new Mock<ICustomerPreferenceRepository>();
        var mapper = new Mock<IMapper>();

        var page = new List<CRMPreference> { CRMPreference.Create(1, 2, 3, true, DateTime.UtcNow, 1) };
        repository.Setup(x => x.GetByCustomerPagedAsync(1, 2, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(page);
        repository.Setup(x => x.CountByCustomerAsync(1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        mapper.Setup(x => x.Map<IReadOnlyCollection<CustomerPreferenceResponse>>(page))
            .Returns(new[] { new CustomerPreferenceResponse(1, 1, 2, 3, true, true) });

        var handler = new GetCustomerPreferencesByCustomerQueryHandler(repository.Object, mapper.Object);
        var result = await handler.Handle(new GetCustomerPreferencesByCustomerQuery(1, 2, 1, 10), CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(1, result.Total);
    }
}
