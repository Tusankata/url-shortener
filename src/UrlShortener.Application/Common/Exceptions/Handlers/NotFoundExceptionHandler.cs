using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.Common.Abstractions;

namespace UrlShortener.Application.Common.Exceptions.Handlers;

public class NotFoundExceptionHandler : IExceptionHandler
{
    private readonly ILogger<NotFoundExceptionHandler> _logger;

    public NotFoundExceptionHandler(ILogger<NotFoundExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(HttpContext context, Exception exception)
    {
        var ex = (NotFoundException)exception;
        context.Response.StatusCode = 404;

        _logger.LogError(ex, "Resource not found: {@Message}", ex.Message ?? "");

        var errorResponse = new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            title = "Not Found",
            status = 404,
            detail = ex.Message,
            instance = context.Request.Path
        };

        var errorResponseJson = JsonSerializer.Serialize(errorResponse);

        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(errorResponseJson);
    }
}
