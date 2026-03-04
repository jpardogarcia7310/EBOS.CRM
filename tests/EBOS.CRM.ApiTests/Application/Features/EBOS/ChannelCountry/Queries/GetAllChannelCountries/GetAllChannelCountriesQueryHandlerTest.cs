using EBOS.CRM.Application.Features.EBOS.ChannelCountry.Queries.GetAllChannelCountries;
using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using Moq;
using ChannelCountryEntity = EBOS.CRM.Domain.Entities.EBOS.ChannelCountry;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.ChannelCountry.Queries.GetAllChannelCountries;

public class GetAllChannelCountriesQueryHandlerTest
{
    private readonly Mock<IChannelCountryRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllChannelCountriesQueryHandler _handler;

    public GetAllChannelCountriesQueryHandlerTest()
    {
        _repositoryMock = new Mock<IChannelCountryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllChannelCountriesQueryHandler(_repositoryMock.Object, _mapperMock.Object);

        _repositoryMock
            .Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ChannelCountryEntity>());
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _mapperMock
            .Setup(m => m.Map<IReadOnlyCollection<ChannelCountryResponse>>(It.IsAny<IReadOnlyCollection<ChannelCountryEntity>>()))
            .Returns(Array.Empty<ChannelCountryResponse>());
    }

    [Fact]
    public async Task Handle_WithData_ReturnsPagedResult()
    {
        var entities = new List<ChannelCountryEntity>
        {
            new() { Id = 1, ChannelTypeId = 10, CountryId = 20, IsActive = true }
        };
        var mapped = new List<ChannelCountryResponse>
        {
            new(1, 10, "Email", 20, "ES", "Spain", "10:20", true)
        };

        _repositoryMock
            .Setup(r => r.GetAllPagedAsync(1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<ChannelCountryResponse>>(entities)).Returns(mapped);

        var result = await _handler.Handle(new GetAllChannelCountriesQuery(1, 20), CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(1, result.Total);
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyItems()
    {
        var result = await _handler.Handle(new GetAllChannelCountriesQuery(1, 20), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task Handle_CancellationRequested_ThrowsOperationCanceled()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(new GetAllChannelCountriesQuery(1, 20), cts.Token));
    }
}
