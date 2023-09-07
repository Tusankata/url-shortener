using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.Common.Abstractions;

namespace UrlShortener.Application.Common.Exceptions.Handlers;

public class ExceptionHandlerFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ExceptionHandlerFactory> _logger;
    public ExceptionHandlerFactory(IServiceProvider serviceProvider, ILogger<ExceptionHandlerFactory> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public IExceptionHandler GetHandler(Exception ex)
    {
        _logger.LogInformation("Getting handler for exception {@Exception}", ex);

        var handlerType = ex switch
        {
            NotFoundException => typeof(NotFoundExceptionHandler),
            _ => typeof(DefaultExceptionHandler)
        };

        _logger.LogInformation("Handler type is {@HandlerType}", handlerType);

        return (IExceptionHandler)_serviceProvider.GetRequiredService(handlerType);
    }
}
