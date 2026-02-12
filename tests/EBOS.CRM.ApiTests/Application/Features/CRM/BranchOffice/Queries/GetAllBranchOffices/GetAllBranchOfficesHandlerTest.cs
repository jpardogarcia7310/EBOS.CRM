using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.BranchOffice.Queries.GetAllBranchOffices;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BranchOffice.Queries.GetAllBranchOffices;

public class GetAllBranchOfficesQueryHandlerTest
{
    private readonly Mock<IBranchOfficeRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_ReturnsList()
    {
        var handler = new GetAllBranchOfficesQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entities = new List<EBOS.CRM.Domain.Entities.CRM.BranchOffice> { new() };
        var dtos = new List<BranchOfficeResponse>();

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<BranchOfficeResponse>>(entities))
            .Returns(dtos);

        var result = await handler.Handle(new GetAllBranchOfficesQuery(), CancellationToken.None);

        Assert.NotNull(result);
    }
}
