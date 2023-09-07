namespace UrlShortener.Application.Services;

public interface ICodeProcessor
{
    Task ProcessCodes(CancellationToken cancellationToken);
}