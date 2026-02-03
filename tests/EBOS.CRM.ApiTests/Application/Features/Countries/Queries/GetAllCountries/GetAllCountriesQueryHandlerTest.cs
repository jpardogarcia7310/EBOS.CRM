using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Features.Countries.Queries.GetAllCountries;
using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.Countries.Queries.GetAllCountries;

public class GetAllCountriesQueryHandlerTest
{
    private readonly Mock<ICountryRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllCountriesQueryHandler _handler;

    public GetAllCountriesQueryHandlerTest()
    {
        _repositoryMock = new Mock<ICountryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllCountriesQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_CountriesExist_ReturnsMappedDtos()
    {
        // Arrange
        var countries = new List<Country>
        {
            new() { Id = 1, Name = "España", Iso31661A2Code = "ES", Iso31661A3Code = "ESP", Iso31661NumCode = "724",
                Domain = ".es", Currency = "Euro", CurrencyCode = "EUR", InternationalPhoneCode = "34" }
        };
        var dtos = new List<CountryResponse>
        {
            new(1, "España", "ES", "ESP", "724", ".es", "Euro", "EUR", "34")
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(countries);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<CountryResponse>>(countries)).Returns(dtos);

        var query = new GetAllCountriesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("España", result.First().Name);
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()),
            Times.Once);
        _mapperMock.Verify(m => m.Map<IReadOnlyCollection<CountryResponse>>(countries), Times.Once);
    }

    [Fact]
    public async Task Handle_NoCountries_ReturnsEmptyEnumerable()
    {
        // Arrange
        var countries = new List<Country>();
        var dtos = new List<CountryResponse>();

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(countries);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<CountryResponse>>(countries)).Returns(dtos);

        var query = new GetAllCountriesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_PropagatesException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new Exception("DB error"));
        var query = new GetAllCountriesQuery();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CancellationRequested_ThrowsOperationCanceled()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var query = new GetAllCountriesQuery();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(query, cts.Token));
    }

    [Fact]
    public async Task Handle_MapperConfigurationInvalid_ThrowsMappingException()
    {
        // Arrange
        var countries = new List<Country> { new() { Id = 1, Name = "España" } };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(countries);

        // Simulamos que el mapper de Mapster falla
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<CountryResponse>>(countries))
                   .Throws(new InvalidOperationException("Mapping failed"));

        var query = new GetAllCountriesQuery();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NullEntityProperty_MapsGracefully()
    {
        // Arrange
        var countries = new List<Country> { new() { Id = 1, Name = null! } };
        var dtos = new List<CountryResponse> { new(1, null!, "ES", "ESP", "724", ".es", "Euro", "EUR", "34") };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(countries);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<CountryResponse>>(countries)).Returns(dtos);

        var query = new GetAllCountriesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Null(result.First().Name);
    }

    [Fact]
    public async Task Handle_MapperCalledWithCorrectSourceType()
    {
        // Arrange
        var countries = new List<Country>();
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(countries);

        var query = new GetAllCountriesQuery();

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mapperMock.Verify(m => m.Map<IReadOnlyCollection<CountryResponse>>(countries), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryCalledOnce_WithCancellationToken()
    {
        // Arrange
        var query = new GetAllCountriesQuery();
        var countries = new List<Country>();
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(countries);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}










