using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.Customer.Queries.GetAllCustomers;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Customer.Queries.GetAllCustomers;

public class GetAllCustomersQueryHandlerTest
{
    private readonly Mock<ICustomerRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_ReturnsList()
    {
        var handler = new GetAllCustomersQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entities = new List<EBOS.CRM.Domain.Entities.CRM.Customer> { new() };
        var dtos = new List<CustomerResponse>();

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<CustomerResponse>>(entities))
            .Returns(dtos);

        var result = await handler.Handle(new GetAllCustomersQuery(), CancellationToken.None);

        Assert.NotNull(result);
    }
}







