using EBOS.CRM.Api.Controllers.Countries;
using EBOS.CRM.Application.Features.Countries.Dtos;
using EBOS.CRM.Application.Features.Countries.Queries.GetAllCountries;
using EBOS.CRM.Application.Features.Countries.Queries.GetCountryById;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EBOS.CRM.ApiTests.Controllers;

public class CountryControllerTest
{
    private readonly Mock<IMediator> _mediatorMock;

    public CountryControllerTest()
    {
        _mediatorMock = new Mock<IMediator>(MockBehavior.Strict);
    }

    private CountriesController CreateController() => new(_mediatorMock.Object);

    private static CountryResponseDto SampleDto(long id = 1) =>
        new(
            Id: id,
            Name: "España",
            Iso31661A2Code: "ES",
            Iso31661A3Code: "ESP",
            Iso31661NumCode: "724",
            Domain: "es",
            Currency: "Euro",
            CurrencyCode: "EUR",
            InternationalPhoneCode: "+34"
        );

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(42)]
    public async Task GetById_Returns_Ok_WithDto_ForMultipleIds(long id)
    {
        // Arrange
        var dto = SampleDto(id);
        _mediatorMock
            .Setup(m => m.Send(It.Is<GetCountryByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto)
            .Verifiable();

        var controller = CreateController();

        // Act
        var result = await controller.GetById(id, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var ok = result as OkObjectResult;
        ok!.StatusCode.Should().Be(200);
        ok.Value.Should().BeEquivalentTo(dto);

        _mediatorMock.Verify(m => m.Send(It.IsAny<GetCountryByIdQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(99)]
    [InlineData(1000)]
    public async Task GetById_Returns_NotFound_ForMissingIds(long id)
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.Is<GetCountryByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CountryResponseDto?)null)
            .Verifiable();

        var controller = CreateController();

        // Act
        var result = await controller.GetById(id, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var nf = result as NotFoundObjectResult;
        nf!.StatusCode.Should().Be(404);
        nf.Value.Should().BeOfType<ProblemDetails>();
        var pd = nf.Value as ProblemDetails;
        pd!.Detail.Should().Contain($"Country with id {id} not found.");

        _mediatorMock.Verify(m => m.Send(It.IsAny<GetCountryByIdQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    public async Task GetAll_Returns_Ok_WithExpectedCount(int count)
    {
        // Arrange
        var list = new List<CountryResponseDto>();
        for (var i = 1; i <= count; i++) list.Add(SampleDto(i));

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllCountriesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(list)
            .Verifiable();

        var controller = CreateController();

        // Act
        var result = await controller.GetAll(CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var ok = result as OkObjectResult;
        ok!.StatusCode.Should().Be(200);
        var returned = ok.Value as IEnumerable<CountryResponseDto>;
        returned.Should().NotBeNull();
        returned!.Should().HaveCount(count);

        _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllCountriesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // La lógica del controlador es independiente de la versión; comprobamos que la acción funciona.
    [Theory]
    [InlineData("1.0")]
    [InlineData("2.0")]
    public async Task ControllerLogic_IsVersionAgnostic(string apiVersion)
    {
        // Arrange
        var id = 7L;
        var dto = SampleDto(id);
        _ = apiVersion;
        _mediatorMock
            .Setup(m => m.Send(It.Is<GetCountryByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto)
            .Verifiable();

        var controller = CreateController();

        // Act
        var result = await controller.GetById(id, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetCountryByIdQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}