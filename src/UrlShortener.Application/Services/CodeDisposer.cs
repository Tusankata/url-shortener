using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using UrlShortener.Application.Common.Models.Abstractions;
using UrlShortener.Application.Common.Models.Enums;
using UrlShortener.Application.Entities;
using UrlShortener.Application.Infrastructure.Persistence;

namespace UrlShortener.Application.Services;

public class CodeDisposer : ICodeDisposer
{
    private readonly AppDbContext _context;

    public CodeDisposer(AppDbContext context) => _context = context;

    public async Task<int> DisposeCodes(CancellationToken cancellationToken)
    {
        var codesToRemove = await _context.ShortUrlCodes
        .Where(code => code.State == CodeState.Used)
            .ToListAsync(cancellationToken);

        _context.ShortUrlCodes.RemoveRange(codesToRemove);

        return await _context.SaveChangesAsync(cancellationToken);
    }
}
