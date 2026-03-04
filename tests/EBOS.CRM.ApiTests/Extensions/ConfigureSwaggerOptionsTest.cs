using EBOS.CRM.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EBOS.CRM.ApiTests.Extensions;

public class ConfigureSwaggerOptionsTest
{
    [Fact]
    public void Configure_AddsDocsAndSecurity()
    {
        var provider = new FakeApiVersionDescriptionProvider(
            new ApiVersionDescription(new ApiVersion(1,0), "v1", false));

        var sut = new ConfigureSwaggerOptions(provider);
        var options = new SwaggerGenOptions();

        sut.Configure(options);

        Assert.True(options.SwaggerGeneratorOptions.SwaggerDocs.ContainsKey("v1"));
        Assert.Contains("Bearer", options.SwaggerGeneratorOptions.SecuritySchemes.Keys);
        Assert.Contains(options.OperationFilterDescriptors, x => x.Type.Name == "ErrorResponsesOperationFilter");
    }

    private sealed class FakeApiVersionDescriptionProvider : IApiVersionDescriptionProvider
    {
        public FakeApiVersionDescriptionProvider(params ApiVersionDescription[] descriptions)
        {
            ApiVersionDescriptions = descriptions;
        }

        public IReadOnlyList<ApiVersionDescription> ApiVersionDescriptions { get; }

        public bool IsDeprecated(Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor actionDescriptor, ApiVersion apiVersion) => false;
    }
}
