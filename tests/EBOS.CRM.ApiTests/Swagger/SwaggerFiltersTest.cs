using EBOS.CRM.Api.Swagger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EBOS.CRM.ApiTests.Swagger;

public class SwaggerFiltersTest
{
    [Fact]
    public void DebugGroupNameOperationFilter_AddsExtension()
    {
        var operation = new OpenApiOperation();
        var ctx = BuildOperationContext("v1");

        new DebugGroupNameOperationFilter().Apply(operation, ctx);

        Assert.True(operation.Extensions.ContainsKey("x-groupName"));
    }

    [Fact]
    public void ErrorResponsesOperationFilter_AddsStandardResponses()
    {
        var operation = new OpenApiOperation();
        var ctx = BuildOperationContext("v1");

        new ErrorResponsesOperationFilter().Apply(operation, ctx);

        Assert.True(operation.Responses.ContainsKey("400"));
        Assert.True(operation.Responses.ContainsKey("404"));
        Assert.True(operation.Responses.ContainsKey("500"));
    }

    [Fact]
    public void PaginationOperationFilter_AddsHeaderAndBadRequestResponse()
    {
        var operation = new OpenApiOperation
        {
            Parameters =
            [
                new OpenApiParameter { Name = "pageNumber" },
                new OpenApiParameter { Name = "pageSize" }
            ],
            Responses = { ["200"] = new OpenApiResponse() }
        };
        var ctx = BuildOperationContext("v1");

        new PaginationOperationFilter().Apply(operation, ctx);

        Assert.Contains(operation.Responses["200"].Headers, h => h.Key == "X-Total-Count");
        Assert.True(operation.Responses.ContainsKey("400"));
    }

    [Fact]
    public void ValidationProblemDetailsOperationFilter_AddsExampleTo400()
    {
        var operation = new OpenApiOperation { Responses = { ["400"] = new OpenApiResponse() } };
        var ctx = BuildOperationContext("v1");

        new ValidationProblemDetailsOperationFilter().Apply(operation, ctx);

        Assert.True(operation.Responses["400"].Content.ContainsKey("application/problem+json"));
    }

    [Fact]
    public void ValidationProblemDetailsSchemaFilter_AppliesOnlyForValidationProblemDetails()
    {
        var filter = new ValidationProblemDetailsSchemaFilter();
        var schema = new OpenApiSchema();
        var ctx = BuildSchemaContext(typeof(ValidationProblemDetails));

        filter.Apply(schema, ctx);

        Assert.True(schema.Extensions.ContainsKey("x-errors-detailed"));
    }

    private static OperationFilterContext BuildOperationContext(string group)
    {
        var schemaGenerator = new SchemaGenerator(
            new SchemaGeneratorOptions(),
            new JsonSerializerDataContractResolver(new System.Text.Json.JsonSerializerOptions()));
        var schemaRepo = new SchemaRepository();
        var apiDesc = new ApiDescription { GroupName = group };
        var methodInfo = typeof(SwaggerFiltersTest).GetMethod(nameof(DummyMethod), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!;
        return new OperationFilterContext(apiDesc, schemaGenerator, schemaRepo, methodInfo);
    }

    private static SchemaFilterContext BuildSchemaContext(Type type)
    {
        var schemaGenerator = new SchemaGenerator(
            new SchemaGeneratorOptions(),
            new JsonSerializerDataContractResolver(new System.Text.Json.JsonSerializerOptions()));
        var schemaRepo = new SchemaRepository();
        return new SchemaFilterContext(type, schemaGenerator, schemaRepo);
    }

    private static void DummyMethod() { }
}
