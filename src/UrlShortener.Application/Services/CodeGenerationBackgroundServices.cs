using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace UrlShortener.Application.Services;

public sealed class CodeGenerationBackgroundService : BackgroundService
{
    private readonly ILogger<CodeGenerationBackgroundService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public CodeGenerationBackgroundService(ILogger<CodeGenerationBackgroundService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var _generator = scope.ServiceProvider.GetRequiredService<ICodeProcessor>();
            await _generator.ProcessCodes(stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); 
        }
    }
}

