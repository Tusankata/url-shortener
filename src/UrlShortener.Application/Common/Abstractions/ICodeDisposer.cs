using MongoDB.Driver;

namespace UrlShortener.Application.Common.Models.Abstractions;

public interface ICodeDisposer
{
    Task<int> DisposeCodes(CancellationToken cancellationToken);
}
