using EBOS.CRM.Api.Extensions;
using EBOS.CRM.Api.Validation;
using EBOS.CRM.Application.Behavior;
using EBOS.CRM.Application.Features.Countries.Commands.AddCountry;
using EBOS.CRM.Application.Mappings;
using EBOS.CRM.Domain.Interfaces.Repositories;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.Infrastructure.Repositories.Concrete;
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
var configuration = builder.Configuration;

// DbContext
services.AddDbContext<CrmDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("CrmDb")));

// Application layer registrations
services.AddAutoMapper(typeof(CountryMapping).Assembly);
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddCountryCommand).Assembly));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Infrastructure
services.AddScoped<ICountryRepository, CountryRepository>();

// Register FluentValidation validators (from Application assembly)
services.AddValidatorsFromAssembly(typeof(AddCountryCommand).Assembly);

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

// 🔧 ApiVersioning
SwaggerConfig.ApiVersioning(services);

// Swagger / OpenAPI
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
// Configuración de Swagger vía DI (evita BuildServiceProvider)
services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

// Global JSON options
services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});


var app = builder.Build();
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
        // Simplificación del loop con Select (SonarQube)
        foreach (var group in provider.ApiVersionDescriptions.Select(d => d.GroupName))
        {
            options.SwaggerEndpoint($"/swagger/{group}/swagger.json",
                                    $"EBOS.CRM API {group.ToUpperInvariant()}");
        }
        // 🔧 Mostrar tags separados por versión
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