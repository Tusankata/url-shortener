using Microsoft.EntityFrameworkCore.Diagnostics;
using UrlShortener.Application.Common.Models.Enums;
using UrlShortener.Application.ValueObjects;

namespace UrlShortener.Application.Entities;

public sealed class ShortUrlCode
{
    public int Id { get; init; }
    public byte[]? RowVersion { get; init; }
    public Code Code { get; init; }
    public CodeState State { get; private set; }

    private ShortUrlCode(Code code, CodeState state) => (Code, State) = (code, state);

#pragma warning disable CS8618
    private ShortUrlCode() { }
#pragma warning restore CS8618

    private static ShortUrlCode? TryCreate(Code code) =>
        code?.Value is null ? null : new ShortUrlCode(code, CodeState.Available);

    public static ShortUrlCode Create(Code code) =>
        TryCreate(code) ?? throw new ArgumentException("Provided code cannot be null.");

    public static IEnumerable<ShortUrlCode> CreateMany(IEnumerable<Code> codes) =>
        codes.Select(code => Create(code));

    public void MarkAsUsed() =>
        State = State switch
        {
            CodeState.Available => CodeState.Used,
            CodeState.Used => throw new InvalidOperationException("Code is already used."),
            _ => throw new InvalidOperationException("Code is in an invalid state.")
        };

    public override string ToString() => Code!.Value;
}
