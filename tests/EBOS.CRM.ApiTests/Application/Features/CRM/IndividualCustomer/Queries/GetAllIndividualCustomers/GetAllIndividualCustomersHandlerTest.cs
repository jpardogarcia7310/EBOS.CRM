using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.IndividualCustomer.Queries.GetAllIndividualCustomers;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.IndividualCustomer.Queries.GetAllIndividualCustomers;

public class GetAllIndividualCustomersQueryHandlerTest
{
    private readonly Mock<IIndividualCustomerRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_ReturnsList()
    {
        var handler = new GetAllIndividualCustomersQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entities = new List<EBOS.CRM.Domain.Entities.CRM.IndividualCustomer> { new() };
        var dtos = new List<IndividualCustomerResponse>();

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<IndividualCustomerResponse>>(entities))
            .Returns(dtos);

        var result = await handler.Handle(new GetAllIndividualCustomersQuery(), CancellationToken.None);

        Assert.NotNull(result);
    }
}







