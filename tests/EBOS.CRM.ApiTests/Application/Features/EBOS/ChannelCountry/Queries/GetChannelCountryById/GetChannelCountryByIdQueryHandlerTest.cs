using EBOS.CRM.Application.Features.EBOS.ChannelCountry.Queries.GetChannelCountryById;
using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using Moq;
using ChannelCountryEntity = EBOS.CRM.Domain.Entities.EBOS.ChannelCountry;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.ChannelCountry.Queries.GetChannelCountryById;

public class GetChannelCountryByIdQueryHandlerTest
{
    private readonly Mock<IChannelCountryRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetChannelCountryByIdQueryHandler _handler;

    public GetChannelCountryByIdQueryHandlerTest()
    {
        _repositoryMock = new Mock<IChannelCountryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetChannelCountryByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsMappedDto()
    {
        var entity = new ChannelCountryEntity { Id = 1, ChannelTypeId = 10, CountryId = 20, IsActive = true };
        var dto = new ChannelCountryResponse(1, 10, "Email", 20, "ES", "Spain", "10:20", true);

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<ChannelCountryResponse>(entity)).Returns(dto);

        var result = await _handler.Handle(new GetChannelCountryByIdQuery(1), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(dto, result);
        _repositoryMock.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<ChannelCountryResponse>(entity), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingId_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ChannelCountryEntity?)null);

        var result = await _handler.Handle(new GetChannelCountryByIdQuery(99), CancellationToken.None);

        Assert.Null(result);
        _mapperMock.Verify(m => m.Map<ChannelCountryResponse>(It.IsAny<ChannelCountryEntity>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CancellationRequested_ThrowsOperationCanceled()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(new GetChannelCountryByIdQuery(1), cts.Token));
    }
}
