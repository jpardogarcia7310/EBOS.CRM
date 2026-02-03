using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Queries.GetBranchOfficeAddressById;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BranchOfficeAddress.Queries.GetBranchOfficeAddressById;

public class GetBranchOfficeAddressByIdQueryHandlerTest
{
    private readonly Mock<IBranchOfficeAddressRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_WhenFound_Maps()
    {
        var handler = new GetBranchOfficeAddressByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entity = new EBOS.CRM.Domain.Entities.CRM.BranchOfficeAddress();

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<BranchOfficeAddressResponse>(entity))
            .Returns((BranchOfficeAddressResponse)null!);

        await handler.Handle(new GetBranchOfficeAddressByIdQuery(1), CancellationToken.None);

        _mapperMock.Verify(m => m.Map<BranchOfficeAddressResponse>(entity), Times.Once);
    }
}
