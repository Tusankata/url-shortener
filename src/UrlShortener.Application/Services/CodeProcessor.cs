using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.Common.Models.Abstractions;
using UrlShortener.Application.Common.Models.Enums;
using UrlShortener.Application.Infrastructure.Persistence;

namespace UrlShortener.Application.Services;

public class CodeProcessor : ICodeProcessor
{
    private readonly ICodeDisposer _codeDisposer;
    private readonly ICodeGenerator _codeGenerator;

    private const int _availableCodesLimit = 10;
    private const int _usedCodesLimit = 30;

    private readonly AppDbContext _context;
    private readonly ILogger<CodeProcessor> _logger;

    public CodeProcessor(ICodeDisposer codeDisposer, ICodeGenerator codeGenerator, AppDbContext context, ILogger<CodeProcessor> logger)
    {
        _codeDisposer = codeDisposer;
        _codeGenerator = codeGenerator;
        _context = context;
        _logger = logger;
    }

    public async Task ProcessCodes(CancellationToken cancellationToken)
    {
        if (await AvailableCodesReachedLimit())
        {
            _logger.LogInformation("Available codes reached limit. Generating new codes...");
            await _codeGenerator.GenerateCodes(cancellationToken);
        }
        else if (await UsedCodesReachedLimit())
        {
            _logger.LogInformation("Used codes reached limit. Disposing codes...");
            await _codeDisposer.DisposeCodes(cancellationToken);
        }
    }

    private async Task<bool> UsedCodesReachedLimit()
    {
        return await _context.ShortUrlCodes
            .Where(code => code.State == CodeState.Used)
            .CountAsync() >= _usedCodesLimit;
    }

    private async Task<bool> AvailableCodesReachedLimit()
    {
        return await _context.ShortUrlCodes
            .Where(code => code.State == CodeState.Available)
            .CountAsync() <= _availableCodesLimit;
    }
}
