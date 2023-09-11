using UrlShortener.Application.Models;

namespace UrlShortener.Application.ValueObjects;

public sealed class Code : ValueObject
{
    public string Value { get; init; }

#pragma warning disable CS8618
    private Code() { }
#pragma warning restore CS8618

    private Code(string value) => Value = value;

    public static Code Create(string value) =>
        TryCreate(value) ?? throw new ArgumentException("Value cannot be null.");

    public static Code Create(Code code) =>
        TryCreate(code.Value) ?? throw new ArgumentException("Value cannot be null.");

    private static Code? TryCreate(string value) =>
        value is null
            ? null
            : new Code(value);

    public static IEnumerable<Code> CreateMany(IEnumerable<string> values) =>
        values.Select(value => Create(value));

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}

