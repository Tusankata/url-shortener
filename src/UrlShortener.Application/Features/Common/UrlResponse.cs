namespace UrlShortener.Application.Features.Common;

public record UrlResponse(
    int UrlId,
    string ShortUrl,
    string OriginalUrl,
    string ShortCode,
    DateTime CreatedAt);