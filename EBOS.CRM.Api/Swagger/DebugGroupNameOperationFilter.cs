using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace EBOS.CRM.Api.Swagger;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class DebugGroupNameOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var groupName = context.ApiDescription.GroupName ?? "NULL";
        operation.Extensions["x-groupName"] = new OpenApiString(groupName);
    }
}
