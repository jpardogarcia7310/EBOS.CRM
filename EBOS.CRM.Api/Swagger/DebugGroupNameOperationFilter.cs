using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EBOS.CRM.Api.Swagger;

public class DebugGroupNameOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var group = context.ApiDescription.GroupName ?? "NULL";
        operation.Extensions["x-groupName"] = new OpenApiString(group);
    }
}