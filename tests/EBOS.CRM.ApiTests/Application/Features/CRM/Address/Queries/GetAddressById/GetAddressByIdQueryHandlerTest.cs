using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.Address.Queries.GetAddressById;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Address.Queries.GetAddressById;

public class GetAddressByIdQueryHandlerTest
{
    private readonly Mock<IAddressRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_WhenFound_Maps()
    {
        var handler = new GetAddressByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entity = new EBOS.CRM.Domain.Entities.CRM.Address();

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<AddressResponse>(entity))
            .Returns((AddressResponse)null!);

        await handler.Handle(new GetAddressByIdQuery(1), CancellationToken.None);

        _mapperMock.Verify(m => m.Map<AddressResponse>(entity), Times.Once);
    }
}
