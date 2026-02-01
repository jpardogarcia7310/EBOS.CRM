using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.CustomerAddress.Queries.GetAllCustomerAddresses;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;
using EBOS.CRM.Domain.Primitives.Paging;
using EBOS.CRM.Application.Contracts.Requests.Common;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerAddress.Queries.GetAllCustomerAddresses;

public class GetAllCustomerAddressesQueryHandlerTest
{
    private readonly Mock<ICustomerAddressRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_ReturnsList()
    {
        var handler = new GetAllCustomerAddressesQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entities = new List<EBOS.CRM.Domain.Entities.CRM.CustomerAddress> { new() };
        var dtos = new List<CustomerAddressResponse>();

        _repositoryMock.Setup(r => r.GetPagedAsync(It.IsAny<PagedQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<EBOS.CRM.Domain.Entities.CRM.CustomerAddress>(entities, 1, 50, entities.Count, entities.Count == 0 ? 0 : 1, null, null, null));
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<CustomerAddressResponse>>(entities))
            .Returns(dtos);

        var result = await handler.Handle(new GetAllCustomerAddressesQuery(new PagedQueryRequest()), CancellationToken.None);

        Assert.NotNull(result);
    }
}




