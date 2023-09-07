using UrlShortener.Application.Common.Exceptions.Handlers;

namespace UrlShortener.Api.Properties.Middlewares;

public class GlobalExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly ExceptionHandlerFactory _exceptionHandlerFactory;

    public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger, ExceptionHandlerFactory exceptionHandlerFactory)
    {
        _logger = logger;
        _exceptionHandlerFactory = exceptionHandlerFactory;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            var handler = _exceptionHandlerFactory.GetHandler(ex);
            await handler.HandleAsync(context, ex);
        }
    }
}

internal static class GlobalExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}
