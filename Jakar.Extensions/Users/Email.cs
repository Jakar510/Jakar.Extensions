// Jakar.Extensions :: Jakar.Extensions
// 04/27/2024  18:04

namespace Jakar.Extensions;


[Serializable]
[JsonConverter(typeof(EmailJsonConverter))]
public readonly record struct Email( string Value ) : IParsable<Email>, IFormattable
{
    public static readonly          Email Empty = new(EMPTY);
    public static implicit operator string( Email email ) => email.Value;
    public static implicit operator Email( string value ) => new(value);


    public override string ToString()                                   => Value;
    public static   Email  Parse( string s, IFormatProvider? provider ) => new(s);
    public static bool TryParse( [NotNullWhen(true)] string? s, IFormatProvider? provider, out Email result )
    {
        result = !string.IsNullOrWhiteSpace(s) && Regexes.Email.IsMatch(s)
                     ? new Email(s)
                     : Empty;

        return result == Empty;
    }
    public string ToString( string? format, IFormatProvider? formatProvider ) => string.IsNullOrWhiteSpace(format)
                                                                                     ? Value
                                                                                     : $"{nameof(Value)}: {Value}";
}



public sealed class EmailJsonConverter : SerializeAsStringJsonConverter<EmailJsonConverter, Email>;
