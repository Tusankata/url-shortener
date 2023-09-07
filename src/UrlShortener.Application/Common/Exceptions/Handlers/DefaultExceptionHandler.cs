using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.Common.Abstractions;

namespace UrlShortener.Application.Common.Exceptions.Handlers;

public class DefaultExceptionHandler : IExceptionHandler
{
    private readonly ILogger<DefaultExceptionHandler> _logger;

    public DefaultExceptionHandler(ILogger<DefaultExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = 500;

        _logger.LogError(exception, "An unexpected error occurred on the server.");

        var errorResponse = new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            title = "Internal Server Error",
            status = 500,
            detail = "An unexpected error occurred on the server.",
            instance = context.Request.Path
        };

        var errorResponseJson = JsonSerializer.Serialize(errorResponse);

        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsync(errorResponseJson);

    }
}