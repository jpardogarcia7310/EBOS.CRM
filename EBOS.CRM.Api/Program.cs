using EBOS.CRM.Api.Extensions;
using EBOS.CRM.Api.Validation;
using EBOS.CRM.Application;
using EBOS.CRM.Application.Behavior;
using EBOS.CRM.Infrastructure;
using EBOS.CRM.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
// Short aliases
var services = builder.Services;

#if DEBUG
// --- BLOQUE DE DIAGNÓSTICO ---
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
// --- FIN BLOQUE DE DIAGNÓSTICO ---
#endif

// Application layer registrations
services.AddApplication();
builder.Services.AddApplicattionMappings();
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Infrastructure
services.AddInfrastructure(builder.Configuration);

// Register FluentValidation validators (from Application assembly)
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Si quieres escanear TODOS los ensamblados cargados:
builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()); 

// Register the action filter that runs FluentValidation for MVC model binding
services.AddScoped<FluentValidationActionFilter>();

// Controllers + JSON options and register the filter globally
services
    .AddControllers(options =>
    {
        options.Filters.Add<FluentValidationActionFilter>();
    })
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Configure consistent ModelState -> ValidationProblemDetails mapping
services.Configure<ApiBehaviorOptions>(ApiBehaviorConfig.Configure);

// ApiVersioning
SwaggerConfig.ApiVersioning(services);

// Swagger / OpenAPI
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

// Registrar la configuración que crea un SwaggerDoc por versión y filtra por GroupName
services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

// Global JSON options
services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});


var app = builder.Build();

#if DEBUG
// --- BLOQUE DE DIAGNÓSTICO: listar versiones y ApiDescriptions en consola ---
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
Console.WriteLine("=== ApiVersionDescriptions ===");
foreach (var desc in provider.ApiVersionDescriptions)
{
    Console.WriteLine($"GroupName: {desc.GroupName} | ApiVersion: {desc.ApiVersion}");
}

var apiExplorer = app.Services.GetRequiredService<Microsoft.AspNetCore.Mvc.ApiExplorer.IApiDescriptionGroupCollectionProvider>();
Console.WriteLine("=== ApiDescriptions ===");
foreach (var group in apiExplorer.ApiDescriptionGroups.Items)
{
    foreach (var api in group.Items)
    {
        Console.WriteLine($"Path: {api.RelativePath} | GroupName: {api.GroupName} | Controller: {api.ActionDescriptor.RouteValues["controller"]} | Action: {api.ActionDescriptor.RouteValues["action"]}");
    }
}
// --- FIN BLOQUE DE DIAGNÓSTICO ---
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
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var group in provider.ApiVersionDescriptions.Select(d => d.GroupName))
        {
            options.SwaggerEndpoint($"/swagger/{group}/swagger.json",
                                    $"EBOS.CRM API {group.ToUpperInvariant()}");
        }
        // Mostrar tags separados por versión
        options.DefaultModelsExpandDepth(-1); // opcional: oculta modelos por defecto
        options.DisplayOperationId();         // opcional: muestra operationId
    });

    await db.Database.MigrateAsync(cancellationToken);
}
await CrmDbContextSeed.SeedAsync(db, cancellationToken);

// Middleware pipeline
app.UseApiErrorHandling();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();

public partial class Program
{
    // Evita que el analizador sugiera instanciación; mantiene la clase usable por WebApplicationFactory
    protected Program() { }
}