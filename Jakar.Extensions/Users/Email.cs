// Jakar.Extensions :: Jakar.Extensions
// 04/27/2024  18:04

namespace Jakar.Extensions;


[Serializable]
[JsonConverter(typeof(SerializeAsStringJsonConverter<Email>))]
public readonly record struct Email( string Value ) : ISpanParsable<Email>, ISpanFormattable
{
    public static readonly          Email Empty = new(EMPTY);
    public static implicit operator string( Email email ) => email.Value;
    public static implicit operator Email( string value ) => new(value);


    public override string ToString()                                               => Value ?? EMPTY;
    public static   Email  Parse( string             s, IFormatProvider? provider ) => new(s);
    public static   Email  Parse( ReadOnlySpan<char> s, IFormatProvider? provider ) => new(s.ToString());


    /// <summary> <see langword="true"/> when <paramref name="s"/> is a valid email address. </summary>
    public static bool TryParse( [NotNullWhen(true)] string? s, IFormatProvider? provider, out Email result )
    {
        result = !string.IsNullOrWhiteSpace(s) && Regexes.Email.IsMatch(s)
                     ? new Email(s)
                     : Empty;

        return result != Empty;
    }
    public static bool TryParse( ReadOnlySpan<char> s, IFormatProvider? provider, out Email result ) => TryParse(s.ToString(), provider, out result);


    public string ToString( string? format, IFormatProvider? formatProvider ) => string.IsNullOrWhiteSpace(format)
                                                                                     ? ToString()
                                                                                     : $"{nameof(Value)}: {Value}";
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        string text = ToString(format.IsEmpty
                                   ? null
                                   : format.ToString(),
                               provider);

        charsWritten = 0;
        if ( !text.TryCopyTo(destination) ) { return false; }

        charsWritten = text.Length;
        return true;
    }
}
