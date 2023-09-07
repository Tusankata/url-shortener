using MongoDB.Driver;
using UrlShortener.Application.Common.Models.Abstractions;
using UrlShortener.Application.Entities;
using UrlShortener.Application.Infrastructure.Persistence;
using UrlShortener.Application.ValueObjects;

namespace UrlShortener.Application.Services;

public class CodeGenerator : ICodeGenerator
{
    private readonly AppDbContext _context;
    private readonly Random _random = new();
    private const int MaxCodeChars = 7;
    private const int MaxBatchSize = 50;
    private const string CharSet = "23456789abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";
    private readonly List<Task<string>> _tasks = new();

    public CodeGenerator(AppDbContext context) => _context = context;

    public async Task GenerateCodes(CancellationToken cancellationToken)
    {
        while (_tasks.Count < MaxBatchSize && !cancellationToken.IsCancellationRequested)
        {
            _tasks.Add(GenerateRandomCodeAsync(cancellationToken));
        }

        await Task.WhenAll(_tasks);

        var newCodes = _tasks.ConvertAll(task => task.Result);

        if (newCodes.Count >= MaxBatchSize)
        {
            var codeList = CreateShortUrlCodesFromBatch(newCodes);
            InsertCodesIntoDatabase(codeList);
        }

        await _context.SaveChangesAsync(cancellationToken);

        ResetTaskList();
    }

    private async Task<string> GenerateRandomCodeAsync(CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            var codeChars = new char[MaxCodeChars];
            for (int i = 0; i < MaxCodeChars; i++)
            {
                var randomIndex = _random.Next(CharSet.Length);
                codeChars[i] = CharSet[randomIndex];
            }
            return new string(codeChars);
        }, cancellationToken);
    }

    private static IEnumerable<ShortUrlCode> CreateShortUrlCodesFromBatch(IEnumerable<string> codes) =>
        ShortUrlCode.CreateMany(Code.CreateMany(codes)).ToList();

    private void InsertCodesIntoDatabase(IEnumerable<ShortUrlCode> codeList) =>
        _context.ShortUrlCodes.AddRange(codeList);

    private void ResetTaskList() => _tasks.Clear();
}
