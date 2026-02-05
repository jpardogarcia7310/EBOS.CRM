using System.Text;
using System.Text.Json;
using EBOS.CRM.Api.Extensions;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Api.Services;
using EBOS.CRM.Api.Security;
using EBOS.CRM.Api.Authentication;
using EBOS.CRM.Application;
using EBOS.CRM.Application.Behavior;
using EBOS.CRM.Infrastructure;
using EBOS.CRM.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
<<<<<<< features/Foundation/Security-&-IAM/API
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
=======
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Security.Claims;
>>>>>>> develop-Security&IAM

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
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Infrastructure
services.AddInfrastructure(builder.Configuration);

services.AddHttpContextAccessor();
services.AddScoped<ICurrentUserContext, HttpContextCurrentUserContext>();
services.Configure<PaginationOptions>(builder.Configuration.GetSection("Pagination"));
services.Configure<OidcOptions>(builder.Configuration.GetSection(OidcOptions.SectionName));
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
});

// Register FluentValidation validators (from Application assembly)
builder.Services.AddValidatorsFromAssembly(typeof(IAssemblyMarker).Assembly);

// Controllers + JSON options and register the filter globally
services
    .AddControllers(options =>
    {
<<<<<<< features/Foundation/Security-&-IAM/API
        if (authOptions.Enabled)
        {
            options.Filters.Add(new AuthorizeFilter("ApiUser"));
        }
=======
        options.Filters.Add<PolicyAuthorizationFilter>();
>>>>>>> develop-Security&IAM
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

services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var oidcOptions = builder.Configuration.GetSection(OidcOptions.SectionName).Get<OidcOptions>() ?? new OidcOptions();

        if (!string.IsNullOrWhiteSpace(oidcOptions.Authority))
        {
            options.Authority = oidcOptions.Authority;
        }

        if (!string.IsNullOrWhiteSpace(oidcOptions.MetadataAddress))
        {
            options.MetadataAddress = oidcOptions.MetadataAddress;
        }

        if (!string.IsNullOrWhiteSpace(oidcOptions.Audience))
        {
            options.Audience = oidcOptions.Audience;
        }

        options.RequireHttpsMetadata = oidcOptions.RequireHttpsMetadata;
        if (oidcOptions.BackchannelTimeoutSeconds > 0)
        {
            options.BackchannelTimeout = TimeSpan.FromSeconds(oidcOptions.BackchannelTimeoutSeconds);
        }
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = (oidcOptions.ValidIssuers?.Length ?? 0) > 0 || !string.IsNullOrWhiteSpace(oidcOptions.Authority),
            ValidateAudience = (oidcOptions.ValidAudiences?.Length ?? 0) > 0 || !string.IsNullOrWhiteSpace(oidcOptions.Audience),
            ValidIssuers = oidcOptions.ValidIssuers,
            ValidAudiences = oidcOptions.ValidAudiences,
            ClockSkew = TimeSpan.FromSeconds(oidcOptions.ClockSkewSeconds),
            RoleClaimType = ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                if (context.Principal?.Identity is ClaimsIdentity identity)
                {
                    ClaimsMapping.MapClaimValues(identity, oidcOptions.RoleClaimType, ClaimTypes.Role);
                    ClaimsMapping.MapClaimValues(identity, oidcOptions.PermissionClaimType, "permission");
                }

                return Task.CompletedTask;
            }
        };
    });
services.AddAuthorization();

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

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();

public partial class Program
{
    // Exposed for WebApplicationFactory in integration tests.
}
