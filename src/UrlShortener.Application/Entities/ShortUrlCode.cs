using UrlShortener.Application.Common.Models.Enums;
using UrlShortener.Application.ValueObjects;

namespace UrlShortener.Application.Entities;

public sealed record ShortUrlCode(Code? Code, CodeState State)
{
    public int Id { get; init; }
    public byte[]? RowVersion { get; init; }

    private ShortUrlCode(Code code) : this(code, CodeState.Available) { }
    private ShortUrlCode() : this(default, default) { }

    private static ShortUrlCode? TryCreate(Code code) =>
        code?.Value is null ? null : new ShortUrlCode(code);

    public static ShortUrlCode Create(Code code) =>
        TryCreate(code) ?? throw new ArgumentException("Provided code cannot be null.");

    public static IEnumerable<ShortUrlCode> CreateMany(IEnumerable<Code> codes) =>
        codes.Select(code => Create(code));

    public ShortUrlCode MarkAsUsed() =>
        State == CodeState.Available
            ? this with { State = CodeState.Used }
            : throw new InvalidOperationException("State is already at used.");

    public override string ToString() => Code!.Value;
}
