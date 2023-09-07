using UrlShortener.Application.Entities;
using UrlShortener.Application.Features.Common;

namespace UrlShortener.Application.Common.Extensions;

public static class ShortenedUrlExtensions
{
    public static UrlResponse ToResponse(this ShortenedUrl shortenedUrl) =>
        new(
            shortenedUrl.Id,
            shortenedUrl.ShortUrl,
            shortenedUrl.LongUrl,
            shortenedUrl.ShortCode,
            shortenedUrl.CreatedAt);
}
