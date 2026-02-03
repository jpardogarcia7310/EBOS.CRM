using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.IndividualCustomer.Queries.GetIndividualCustomerById;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.IndividualCustomer.Queries.GetIndividualCustomerById;

public class GetIndividualCustomerByIdQueryHandlerTest
{
    private readonly Mock<IIndividualCustomerRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_WhenFound_Maps()
    {
        var handler = new GetIndividualCustomerByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entity = new EBOS.CRM.Domain.Entities.CRM.IndividualCustomer();

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<IndividualCustomerResponse>(entity))
            .Returns((IndividualCustomerResponse)null!);

        await handler.Handle(new GetIndividualCustomerByIdQuery(1), CancellationToken.None);

        _mapperMock.Verify(m => m.Map<IndividualCustomerResponse>(entity), Times.Once);
    }
}
