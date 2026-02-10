using EBOS.CRM.Application.Features.EBOS.AddressesType.Query.GetAddressTypeById;
using EBOS.CRM.Application.Features.EBOS.Countries.Queries.GetCountryById;
using EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery;
using EBOS.CRM.Application.Features.EBOS.Statuses.Queries.GetStatusById;
using EBOS.CRM.Application.Features.EBOS.TenantConfiguration.Queries.GetAllTenantConfigurations;
using EBOS.CRM.Application.Features.EBOS.TenantConfiguration.Queries.GetTenantConfigurationById;
using EBOS.CRM.Application.Features.EBOS.TenantQuota.Queries.GetAllTenantQuotas;
using EBOS.CRM.Application.Features.EBOS.TenantQuota.Queries.GetTenantQuotaById;
using EBOS.CRM.Application.Features.EBOS.TenantUsageMetric.Queries.GetAllTenantUsageMetrics;
using EBOS.CRM.Application.Features.EBOS.TenantUsageMetric.Queries.GetTenantUsageMetricById;
using FluentAssertions;

namespace EBOS.CRM.ApiTests.Application.Validators;

public class EbosValidatorTests
{
    [Fact]
    public void GetAddressTypeById_Validates_Id()
    {
        var validator = new GetAddressTypeByIdQueryValidator();
        validator.Validate(new GetAddressTypeByIdQuery(1)).IsValid.Should().BeTrue();
        validator.Validate(new GetAddressTypeByIdQuery(0)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void GetCountryById_Validates_Id()
    {
        var validator = new GetCountryByIdQueryValidator();
        validator.Validate(new GetCountryByIdQuery(1)).IsValid.Should().BeTrue();
        validator.Validate(new GetCountryByIdQuery(0)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void GetIdentificationTypeById_Validates_Id()
    {
        var validator = new GetIdentificationTypeByIdQueryValidator();
        validator.Validate(new GetIdentificationTypeByIdQuery(1)).IsValid.Should().BeTrue();
        validator.Validate(new GetIdentificationTypeByIdQuery(0)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void GetStatusById_Validates_Id()
    {
        var validator = new GetStatusByIdQueryValidator();
        validator.Validate(new GetStatusByIdQuery(1)).IsValid.Should().BeTrue();
        validator.Validate(new GetStatusByIdQuery(0)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void GetTenantConfigurationById_Validates_Id()
    {
        var validator = new GetTenantConfigurationByIdQueryValidator();
        validator.Validate(new GetTenantConfigurationByIdQuery(1)).IsValid.Should().BeTrue();
        validator.Validate(new GetTenantConfigurationByIdQuery(0)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void GetTenantQuotaById_Validates_Id()
    {
        var validator = new GetTenantQuotaByIdQueryValidator();
        validator.Validate(new GetTenantQuotaByIdQuery(1)).IsValid.Should().BeTrue();
        validator.Validate(new GetTenantQuotaByIdQuery(0)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void GetTenantUsageMetricById_Validates_Id()
    {
        var validator = new GetTenantUsageMetricByIdQueryValidator();
        validator.Validate(new GetTenantUsageMetricByIdQuery(1)).IsValid.Should().BeTrue();
        validator.Validate(new GetTenantUsageMetricByIdQuery(0)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void GetAllTenantConfigurations_Validates_Paging()
    {
        var validator = new GetAllTenantConfigurationsQueryValidator();
        validator.Validate(new GetAllTenantConfigurationsQuery(1, 10)).IsValid.Should().BeTrue();
        validator.Validate(new GetAllTenantConfigurationsQuery(0, 10)).IsValid.Should().BeFalse();
        validator.Validate(new GetAllTenantConfigurationsQuery(1, 0)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void GetAllTenantQuotas_Validates_Paging()
    {
        var validator = new GetAllTenantQuotasQueryValidator();
        validator.Validate(new GetAllTenantQuotasQuery(1, 10)).IsValid.Should().BeTrue();
        validator.Validate(new GetAllTenantQuotasQuery(0, 10)).IsValid.Should().BeFalse();
        validator.Validate(new GetAllTenantQuotasQuery(1, 0)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void GetAllTenantUsageMetrics_Validates_Paging()
    {
        var validator = new GetAllTenantUsageMetricsQueryValidator();
        validator.Validate(new GetAllTenantUsageMetricsQuery(1, 10)).IsValid.Should().BeTrue();
        validator.Validate(new GetAllTenantUsageMetricsQuery(0, 10)).IsValid.Should().BeFalse();
        validator.Validate(new GetAllTenantUsageMetricsQuery(1, 0)).IsValid.Should().BeFalse();
    }
}
