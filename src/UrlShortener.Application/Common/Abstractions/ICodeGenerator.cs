namespace UrlShortener.Application.Common.Models.Abstractions;

public interface ICodeGenerator
{
    Task GenerateCodes(CancellationToken cancellationToken);
}
