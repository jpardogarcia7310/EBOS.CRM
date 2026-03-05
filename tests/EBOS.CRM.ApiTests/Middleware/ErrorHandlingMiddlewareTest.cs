using System.Text.Json;
using EBOS.CRM.Api.Middleware;
using EBOS.CRM.Domain.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace EBOS.CRM.ApiTests.Middleware;

public class ErrorHandlingMiddlewareTest
{
    [Fact]
    public async Task Invoke_WhenValidationException_Returns400()
    {
        var middleware = new ErrorHandlingMiddleware(
            _ => throw new ValidationException([new ValidationFailure("Name", "Required")]),
            new Mock<ILogger<ErrorHandlingMiddleware>>().Object);

        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        await middleware.Invoke(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }

    [Fact]
    public async Task Invoke_WhenKeyNotFoundException_Returns404()
    {
        var middleware = new ErrorHandlingMiddleware(
            _ => throw new KeyNotFoundException("missing"),
            new Mock<ILogger<ErrorHandlingMiddleware>>().Object);

        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        await middleware.Invoke(context);

        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
    }

    [Fact]
    public async Task Invoke_WhenUnexpectedException_Returns500()
    {
        var middleware = new ErrorHandlingMiddleware(
            _ => throw new InvalidOperationException("boom"),
            new Mock<ILogger<ErrorHandlingMiddleware>>().Object);

        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        await middleware.Invoke(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var payload = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body);
        Assert.NotNull(payload);
    }

    [Fact]
    public async Task Invoke_WhenDomainValidationException_Returns400_WithTaxonomyMetadata()
    {
        var middleware = new ErrorHandlingMiddleware(
            _ => throw new DomainValidationException("invalid aggregate input"),
            new Mock<ILogger<ErrorHandlingMiddleware>>().Object);

        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        await middleware.Invoke(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var json = await JsonDocument.ParseAsync(context.Response.Body);
        Assert.Equal("DOMAIN_VALIDATION", json.RootElement.GetProperty("code").GetString());
        Assert.False(json.RootElement.GetProperty("retryable").GetBoolean());
    }

    [Fact]
    public async Task Invoke_WhenDomainConflictException_Returns409()
    {
        var middleware = new ErrorHandlingMiddleware(
            _ => throw new DomainConflictException("state conflict", retryable: true),
            new Mock<ILogger<ErrorHandlingMiddleware>>().Object);

        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        await middleware.Invoke(context);

        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
    }

    [Fact]
    public async Task Invoke_WhenDomainRuleViolationException_Returns422()
    {
        var middleware = new ErrorHandlingMiddleware(
            _ => throw new DomainRuleViolationException("invariant broken"),
            new Mock<ILogger<ErrorHandlingMiddleware>>().Object);

        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        await middleware.Invoke(context);

        Assert.Equal(StatusCodes.Status422UnprocessableEntity, context.Response.StatusCode);
    }

    [Fact]
    public async Task Invoke_WhenTransientDomainFailureException_Returns503_WithRetryableTrue()
    {
        var middleware = new ErrorHandlingMiddleware(
            _ => throw new TransientDomainFailureException("domain service temporarily unavailable"),
            new Mock<ILogger<ErrorHandlingMiddleware>>().Object);

        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        await middleware.Invoke(context);

        Assert.Equal(StatusCodes.Status503ServiceUnavailable, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var json = await JsonDocument.ParseAsync(context.Response.Body);
        Assert.True(json.RootElement.GetProperty("retryable").GetBoolean());
    }
}
