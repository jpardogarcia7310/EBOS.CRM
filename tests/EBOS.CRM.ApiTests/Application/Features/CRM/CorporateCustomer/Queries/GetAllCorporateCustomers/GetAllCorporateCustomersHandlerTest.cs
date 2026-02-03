using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.CorporateCustomer.Queries.GetAllCorporateCustomers;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CorporateCustomer.Queries.GetAllCorporateCustomers;

public class GetAllCorporateCustomersQueryHandlerTest
{
    private readonly Mock<ICorporateCustomerRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_ReturnsList()
    {
        var handler = new GetAllCorporateCustomersQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entities = new List<EBOS.CRM.Domain.Entities.CRM.CorporateCustomer> { new() };
        var dtos = new List<CorporateCustomerResponse>();

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<CorporateCustomerResponse>>(entities))
            .Returns(dtos);

        var result = await handler.Handle(new GetAllCorporateCustomersQuery(), CancellationToken.None);

        Assert.NotNull(result);
    }
}







