using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.CustomerAddress.Queries.GetAllCustomerAddresses;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

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

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<CustomerAddressResponse>>(entities))
            .Returns(dtos);

        var result = await handler.Handle(new GetAllCustomerAddressesQuery(), CancellationToken.None);

        Assert.NotNull(result);
    }
}

