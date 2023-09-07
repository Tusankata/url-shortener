using MongoDB.Bson;

namespace UrlShortener.Application.Entities;

public sealed class ShortenedUrl
{
    public int Id { get; init; }
    public string LongUrl { get;  init;}
    public string ShortUrl { get; init; }
    public string ShortCode { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public int Clicks { get; private set; } = 0;

    private ShortenedUrl(string longUrl, string shortUrl, string shortCode) =>
        (LongUrl, ShortUrl, ShortCode) = (longUrl, shortUrl, shortCode);

    private ShortenedUrl() : this(string.Empty, string.Empty, string.Empty) { }

    private static ShortenedUrl? TryCreate(string longUrl, string shortUrl, string shortCode) =>
        string.IsNullOrWhiteSpace(longUrl) || string.IsNullOrWhiteSpace(shortUrl) || string.IsNullOrWhiteSpace(shortCode)
            ? null
            : new ShortenedUrl(longUrl, shortUrl, shortCode);

   public static ShortenedUrl Create(string longUrl, string shortUrl, string shortCode) =>
        TryCreate(longUrl, shortUrl, shortCode) ?? throw new ArgumentException("Provided values cannot be null or empty.");

    public void IncrementClicks() => Clicks++;
    public override string ToString() => ShortUrl;
}
