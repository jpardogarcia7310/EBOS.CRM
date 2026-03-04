using System.Text.Json;
using EBOS.CRM.Api.Middleware;
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
            _ => throw new ValidationException(new[] { new ValidationFailure("Name", "Required") }),
            new Mock<ILogger<ErrorHandlingMiddleware>>().Object);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.Invoke(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }

    [Fact]
    public async Task Invoke_WhenKeyNotFoundException_Returns404()
    {
        var middleware = new ErrorHandlingMiddleware(
            _ => throw new KeyNotFoundException("missing"),
            new Mock<ILogger<ErrorHandlingMiddleware>>().Object);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.Invoke(context);

        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
    }

    [Fact]
    public async Task Invoke_WhenUnexpectedException_Returns500()
    {
        var middleware = new ErrorHandlingMiddleware(
            _ => throw new InvalidOperationException("boom"),
            new Mock<ILogger<ErrorHandlingMiddleware>>().Object);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.Invoke(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var payload = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body);
        Assert.NotNull(payload);
    }
}
