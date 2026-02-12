using System.Text;
using System.Text.Json;
using EBOS.CRM.Api.Extensions;
using EBOS.CRM.Api.Filters;
using EBOS.CRM.Api.HostedServices;
using EBOS.CRM.Api.Infrastructure;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Api.Services;
using EBOS.CRM.Application;
using EBOS.CRM.Application.Behavior;
using EBOS.CRM.Application.Options;
using EBOS.CRM.Application.Services.Commands;
using EBOS.CRM.Domain.Identity;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Infrastructure;
using EBOS.CRM.Infrastructure.Options;
using EBOS.CRM.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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
services.AddOptions<CommandExecutionOptions>()
    .Bind(builder.Configuration.GetSection(CommandExecutionOptions.SectionName));
services.AddOptions<CaseWorkflowOptions>()
    .Bind(builder.Configuration.GetSection(CaseWorkflowOptions.SectionName));

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
services.Configure<OidcOptions>(builder.Configuration.GetSection(OidcOptions.SectionName));
services.Configure<TenantResolutionOptions>(builder.Configuration.GetSection(TenantResolutionOptions.SectionName));
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

services.AddOptions<MultiTenantOptions>()
    .Bind(builder.Configuration.GetSection(MultiTenantOptions.SectionName))
    .Validate(options => options.Strategy != MultiTenantStrategy.Database ||
                         !string.IsNullOrWhiteSpace(options.ConnectionStringTemplate),
        "MultiTenant:ConnectionStringTemplate is required when Strategy is Database.")
    .Validate(options => options.Strategy != MultiTenantStrategy.Database ||
                         options.ConnectionStringTemplate!.Contains("{tenantId}", StringComparison.OrdinalIgnoreCase),
        "MultiTenant:ConnectionStringTemplate must include '{tenantId}'.")
    .Validate(options => options.Strategy != MultiTenantStrategy.Schema ||
                         !string.IsNullOrWhiteSpace(options.SchemaPrefix),
        "MultiTenant:SchemaPrefix is required when Strategy is Schema.")
    .ValidateOnStart();
services.AddLocalization();

var authOptions = builder.Configuration.GetSection(AuthenticationOptions.SectionName)
    .Get<AuthenticationOptions>() ?? new AuthenticationOptions();

services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // To enable EBOS.Auth, set UseAuthority=true and fill Authority/Audience in config.
        if (authOptions.UseAuthority && !string.IsNullOrWhiteSpace(authOptions.Authority))
        {
            options.Authority = authOptions.Authority;
        }

        if (authOptions.UseAuthority && !string.IsNullOrWhiteSpace(authOptions.MetadataAddress))
        {
            options.MetadataAddress = authOptions.MetadataAddress;
        }

        if (!string.IsNullOrWhiteSpace(authOptions.Audience))
        {
            options.Audience = authOptions.Audience;
        }

        options.RequireHttpsMetadata = authOptions.RequireHttpsMetadata;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = authOptions.ValidateIssuer,
            ValidateAudience = authOptions.ValidateAudience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = !string.IsNullOrWhiteSpace(authOptions.SigningKey)
                                       || (authOptions.UseAuthority &&
                                           (!string.IsNullOrWhiteSpace(authOptions.Authority) ||
                                            !string.IsNullOrWhiteSpace(authOptions.MetadataAddress))),
            NameClaimType = authOptions.NameClaimType,
            RoleClaimType = authOptions.RoleClaimType,
            ClockSkew = TimeSpan.FromSeconds(authOptions.ClockSkewSeconds)
        };

        if (!string.IsNullOrWhiteSpace(authOptions.ValidIssuer))
        {
            options.TokenValidationParameters.ValidIssuer = authOptions.ValidIssuer;
        }

        if (authOptions.ValidIssuers is { Length: > 0 })
        {
            options.TokenValidationParameters.ValidIssuers = authOptions.ValidIssuers;
        }

        if (authOptions.ValidAudiences is { Length: > 0 })
        {
            options.TokenValidationParameters.ValidAudiences = authOptions.ValidAudiences;
        }

        // For local dev without EBOS.Auth, keep UseAuthority=false and set SigningKey.
        if (!string.IsNullOrWhiteSpace(authOptions.SigningKey))
        {
            options.TokenValidationParameters.IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.SigningKey));
        }
    });

services.AddAuthorization(options =>
{
    options.AddPolicy("ApiUser", policy =>
        policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CountryRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CountryCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CountryUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CountryDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CountryPatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.StatusRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.StatusCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.StatusUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.StatusDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.StatusPatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.IdentificationTypeRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.IdentificationTypeCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.IdentificationTypeUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.IdentificationTypeDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.IdentificationTypePatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.AddressTypeRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.AddressTypeCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.AddressTypeUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.AddressTypeDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.AddressTypePatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.AddressRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.AddressCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.AddressUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.AddressDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.AddressPatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.BankInformationRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.BankInformationCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.BankInformationUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.BankInformationDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.BankInformationPatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.BranchOfficeRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.BranchOfficeCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.BranchOfficeUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.BranchOfficeDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.BranchOfficePatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.BranchOfficeAddressRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.BranchOfficeAddressCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.BranchOfficeAddressUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.BranchOfficeAddressDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.BranchOfficeAddressPatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CorporateCustomerRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CorporateCustomerCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CorporateCustomerUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CorporateCustomerDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CorporateCustomerPatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CreditAccountRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CreditAccountCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CreditAccountUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CreditAccountDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CreditAccountPatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CreditTransactionRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CreditTransactionCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CreditTransactionUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CreditTransactionDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CreditTransactionPatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CustomerRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CustomerCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CustomerUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CustomerDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CustomerPatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CustomerAddressRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CustomerAddressCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CustomerAddressUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CustomerAddressDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CustomerAddressPatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.IndividualCustomerRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.IndividualCustomerCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.IndividualCustomerUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.IndividualCustomerDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.IndividualCustomerPatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.TaxInformationRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.TaxInformationCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.TaxInformationUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.TaxInformationDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.TaxInformationPatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.TaxInformationAddressRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.TaxInformationAddressCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.TaxInformationAddressUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.TaxInformationAddressDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.TaxInformationAddressPatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.LeadRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.LeadCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.LeadUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.LeadDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.LeadPatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.OpportunityRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.OpportunityCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.OpportunityUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.OpportunityDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.OpportunityPatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.OpportunityStageRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.OpportunityStageCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.OpportunityStageUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.OpportunityStageDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.OpportunityStagePatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.QuoteRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.QuoteCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.QuoteUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.QuoteDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.QuotePatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CaseRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CaseCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CaseUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CaseDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CasePatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.SlaRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.SlaCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.SlaUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.SlaDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.SlaPatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.QueueRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.QueueCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.QueueUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.QueueDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.QueuePatch, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CaseActivityRead, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CaseActivityCreate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CaseActivityUpdate, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CaseActivityDelete, policy => policy.RequireAuthenticatedUser());
    options.AddPolicy(PolicyKeys.Crm.CaseActivityPatch, policy => policy.RequireAuthenticatedUser());
});

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
Console.WriteLine(@"=== ApiVersionDescriptions ===");
foreach (var desc in provider.ApiVersionDescriptions)
{
    Console.WriteLine($@"GroupName: {desc.GroupName} | ApiVersion: {desc.ApiVersion}");
}

var apiExplorer = app.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>();
Console.WriteLine(@"=== ApiDescriptions ===");
foreach (var group in apiExplorer.ApiDescriptionGroups.Items)
{
    foreach (var api in group.Items)
    {
        Console.WriteLine($@"Path: {api.RelativePath} | GroupName: {api.GroupName} | Controller: " +
                          $@"{api.ActionDescriptor.RouteValues["controller"]} | Action: " +
                          $@"{api.ActionDescriptor.RouteValues["action"]}");
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
app.UseTenantResolution();
app.UseTenantRequirement();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();

[System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public partial class Program
{
    // Exposed for WebApplicationFactory in integration tests.
}
