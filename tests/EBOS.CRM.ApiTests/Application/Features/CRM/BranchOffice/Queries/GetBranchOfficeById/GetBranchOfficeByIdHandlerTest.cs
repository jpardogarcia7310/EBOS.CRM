using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.BranchOffice.Queries.GetBranchOfficeById;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BranchOffice.Queries.GetBranchOfficeById;

public class GetBranchOfficeByIdQueryHandlerTest
{
    private readonly Mock<IBranchOfficeRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_WhenFound_Maps()
    {
        var handler = new GetBranchOfficeByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entity = new EBOS.CRM.Domain.Entities.CRM.BranchOffice();

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<BranchOfficeResponse>(entity))
            .Returns((BranchOfficeResponse)null!);

        await handler.Handle(new GetBranchOfficeByIdQuery(1), CancellationToken.None);

        _mapperMock.Verify(m => m.Map<BranchOfficeResponse>(entity), Times.Once);
    }
}


