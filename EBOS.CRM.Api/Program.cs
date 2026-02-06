using System.Text.Json;
using EBOS.CRM.Api.Extensions;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Api.Services;
using EBOS.CRM.Api.Infrastructure;
using EBOS.CRM.Api.HostedServices;
using EBOS.CRM.Api.Filters;
using EBOS.CRM.Application;
using EBOS.CRM.Application.Behavior;
using EBOS.CRM.Application.Options;
using EBOS.CRM.Infrastructure;
using EBOS.CRM.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
// Short aliases
var services = builder.Services;

#if DEBUG
// --- DIAGNOSTIC BLOCK ---
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);
// --- END OF DIAGNOSTIC BLOCK ---
#endif

// Application layer registrations
services.AddApplication();
builder.Services.AddApplicationMappings();
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TenantIsolationBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Infrastructure
services.AddInfrastructure(builder.Configuration);

services.AddHttpContextAccessor();
services.AddScoped<ICurrentUserContext, HttpContextCurrentUserContext>();
services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<ICurrentUserContext>());
services.AddSingleton<ProblemDetailsFactory, CrmProblemDetailsFactory>();
if (builder.Environment.IsDevelopment())
{
    services.AddHostedService<LookupSeedHostedService>();
}
services.Configure<PaginationOptions>(builder.Configuration.GetSection("Pagination"));
services.AddOptions<TenantIsolationOptions>()
    .Bind(builder.Configuration.GetSection(TenantIsolationOptions.SectionName))
    .Validate(options => options.MinTraversalDepth is >= 1 and <= 50,
        "TenantIsolation:MinTraversalDepth must be between 1 and 50.")
    .Validate(options => options.MaxTraversalDepth is >= 1 and <= 50,
        "TenantIsolation:MaxTraversalDepth must be between 1 and 50.")
    .Validate(options => options.MinTraversalDepth <= options.MaxTraversalDepth,
        "TenantIsolation:MinTraversalDepth must be <= TenantIsolation:MaxTraversalDepth.")
    .Validate(options =>
            options.TraversalDepth >= options.MinTraversalDepth &&
            options.TraversalDepth <= options.MaxTraversalDepth,
        "TenantIsolation:TraversalDepth must be within the configured min/max range.")
    .ValidateOnStart();
services.AddLocalization();

// Register FluentValidation validators (from Application assembly)
builder.Services.AddValidatorsFromAssembly(typeof(IAssemblyMarker).Assembly);

// Controllers + JSON options and register the filter globally
services
    .AddControllers(options =>
    {
        options.Filters.Add<PaginationValidationFilter>();
    })
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Configure consistent ModelState -> ValidationProblemDetails mapping
services.Configure<ApiBehaviorOptions>(ApiBehaviorConfig.Configure);

// API Versioning
SwaggerConfig.ApiVersioning(services);

// Swagger / OpenAPI
services.AddEndpointsApiExplorer();
// ⚠️ IMPORTANT: SwaggerGen must come AFTER ApiVersioning + ApiExplorer
services.AddSwaggerGen();
// Register the configuration that creates a SwaggerDoc per version and filters by GroupName
services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

var app = builder.Build();

#if DEBUG
// --- DIAGNOSTIC BLOCK: List versions and API descriptions in the console ---
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
Console.WriteLine("=== ApiVersionDescriptions ===");
foreach (var desc in provider.ApiVersionDescriptions)
{
    Console.WriteLine($"GroupName: {desc.GroupName} | ApiVersion: {desc.ApiVersion}");
}

var apiExplorer = app.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>();
Console.WriteLine("=== ApiDescriptions ===");
foreach (var group in apiExplorer.ApiDescriptionGroups.Items)
{
    foreach (var api in group.Items)
    {
        Console.WriteLine($"Path: {api.RelativePath} | GroupName: {api.GroupName} | Controller: " +
                          $"{api.ActionDescriptor.RouteValues["controller"]} | Action: " +
                          $"{api.ActionDescriptor.RouteValues["action"]}");
    }
}
// --- END OF DIAGNOSTIC BLOCK ---
#endif

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
var cancellationToken = app.Lifetime.ApplicationStopping;

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var group in descriptionProvider.ApiVersionDescriptions.Select(d => d.GroupName))
        {
            options.SwaggerEndpoint($"/swagger/{group}/swagger.json",
                                    $"EBOS.CRM API {group.ToUpperInvariant()}");
        }
        // Show tags separated by version
        options.DefaultModelsExpandDepth(-1); // Optional: Hides default models
        options.DisplayOperationId();         // Optional: Displays operationId
    });
}

try
{
    if (db.Database.IsRelational())
    {
        var canConnect = await db.Database.CanConnectAsync(cancellationToken).ConfigureAwait(false);
        if (canConnect)
        {
            await db.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            await CrmDbContextSeed.SeedAsync(db, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
            logger.LogWarning("Database unavailable. Skipping migrations and seed.");
        }
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
    logger.LogWarning(ex, "Database init failed. Skipping migrations and seed.");
}

// Middleware pipeline
app.UseCorrelationId();
app.UseApiErrorHandling();
app.UseTenantRequirement();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();

public partial class Program
{
    // Exposed for WebApplicationFactory in integration tests.
}
