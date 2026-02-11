using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Entities.EBOS;
using FluentAssertions;
using MapsterMapper;

namespace EBOS.CRM.ApiTests.Application.Mappings;

public class EbosMappingCoverageTests(MapperFixture fixture) : IClassFixture<MapperFixture>
{
    private readonly IMapper _mapper = fixture.Mapper;

    [Fact]
    public void AddressType_Mapping_Covers_All_Fields()
    {
        var entity = new AddressType
        {
            Id = 1,
            Code = "HOME",
            Description = "Home",
            Category = "Shipping",
            AllowsMultiple = true,
            RequiresPrimary = false
        };

        var response = _mapper.Map<AddressTypeResponse>(entity);
        response.Id.Should().Be(1);
        response.Code.Should().Be("HOME");
        response.Description.Should().Be("Home");
        response.Category.Should().Be("Shipping");
        response.AllowsMultiple.Should().BeTrue();
        response.RequiresPrimary.Should().BeFalse();
    }

    [Fact]
    public void Country_Mapping_Covers_All_Fields()
    {
        var entity = new Country
        {
            Id = 2,
            Name = "Spain",
            Iso31661A2Code = "ES",
            Iso31661A3Code = "ESP",
            Iso31661NumCode = "724",
            Domain = ".es",
            Currency = "Euro",
            CurrencyCode = "EUR",
            InternationalPhoneCode = "+34"
        };

        var response = _mapper.Map<CountryResponse>(entity);
        response.Id.Should().Be(2);
        response.Name.Should().Be("Spain");
        response.Iso31661A2Code.Should().Be("ES");
        response.Iso31661A3Code.Should().Be("ESP");
        response.Iso31661NumCode.Should().Be("724");
        response.Domain.Should().Be(".es");
        response.Currency.Should().Be("Euro");
        response.CurrencyCode.Should().Be("EUR");
        response.InternationalPhoneCode.Should().Be("+34");
    }

    [Fact]
    public void IdentificationType_Mapping_Covers_All_Fields()
    {
        var entity = new IdentificationType
        {
            Id = 3,
            Code = "DNI",
            Description = "Documento"
        };

        var response = _mapper.Map<IdentificationTypeResponse>(entity);
        response.Id.Should().Be(3);
        response.Code.Should().Be("DNI");
        response.Description.Should().Be("Documento");
    }

    [Fact]
    public void Status_Mapping_Covers_All_Fields()
    {
        var entity = new Status
        {
            Id = 4,
            Description = "Active"
        };

        var response = _mapper.Map<StatusResponse>(entity);
        response.Id.Should().Be(4);
        response.Description.Should().Be("Active");
    }

    [Fact]
    public void TenantConfiguration_Mapping_Covers_All_Fields()
    {
        var entity = new TenantConfiguration
        {
            Id = 5,
            TenantId = 1,
            Key = "limits.maxUsers",
            ValueJson = "{\"value\":25}"
        };

        var response = _mapper.Map<TenantConfigurationResponse>(entity);
        response.Id.Should().Be(5);
        response.TenantId.Should().Be(1);
        response.Key.Should().Be("limits.maxUsers");
        response.ValueJson.Should().Be("{\"value\":25}");
    }

    [Fact]
    public void TenantQuota_Mapping_Covers_All_Fields()
    {
        var entity = new TenantQuota
        {
            Id = 6,
            TenantId = 1,
            Metric = "users",
            Limit = 100m,
            Unit = "count",
            EffectiveFrom = DateTime.UtcNow.AddDays(-1),
            EffectiveTo = DateTime.UtcNow.AddDays(30)
        };

        var response = _mapper.Map<TenantQuotaResponse>(entity);
        response.Id.Should().Be(6);
        response.TenantId.Should().Be(1);
        response.Metric.Should().Be("users");
        response.Limit.Should().Be(100m);
        response.Unit.Should().Be("count");
        response.EffectiveFrom.Should().Be(entity.EffectiveFrom);
        response.EffectiveTo.Should().Be(entity.EffectiveTo);
    }

    [Fact]
    public void TenantUsageMetric_Mapping_Covers_All_Fields()
    {
        var entity = new TenantUsageMetric
        {
            Id = 7,
            TenantId = 1,
            Metric = "api.calls",
            Value = 250m,
            Unit = "count",
            PeriodStart = DateTime.UtcNow.AddDays(-7),
            PeriodEnd = DateTime.UtcNow,
            Source = "gateway"
        };

        var response = _mapper.Map<TenantUsageMetricResponse>(entity);
        response.Id.Should().Be(7);
        response.TenantId.Should().Be(1);
        response.Metric.Should().Be("api.calls");
        response.Value.Should().Be(250m);
        response.Unit.Should().Be("count");
        response.PeriodStart.Should().Be(entity.PeriodStart);
        response.PeriodEnd.Should().Be(entity.PeriodEnd);
        response.Source.Should().Be("gateway");
    }
}
