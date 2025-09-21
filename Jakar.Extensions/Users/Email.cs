// Jakar.Extensions :: Jakar.Extensions
// 04/27/2024  18:04

namespace Jakar.Extensions;


[Serializable, JsonConverter(typeof(EmailJsonConverter))]
public readonly record struct Email( string Value ) : IParsable<Email>
{
    public static readonly          Email Empty = new(string.Empty);
    public static implicit operator string( Email email ) => email.Value;
    public static implicit operator Email( string value ) => new(value);


    public override string ToString()                                   => Value;
    public static   Email  Parse( string s, IFormatProvider? provider ) => new(s);
    public static bool TryParse( [NotNullWhen(true)] string? s, IFormatProvider? provider, out Email result )
    {
        result = !string.IsNullOrWhiteSpace(s) && Validate.Re.Email.IsMatch(s)
                     ? new Email(s)
                     : Empty;

        return result == Empty;
    }
}




public sealed class EmailJsonConverter : SerializeAsStringJsonConverter<EmailJsonConverter, Email>
{
    public override Email Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        string? value = reader.GetString();

        return string.IsNullOrWhiteSpace(value)
                   ? Email.Empty
                   : new Email(value);
    }
    public override void Write( Utf8JsonWriter writer, Email value, JsonSerializerOptions options ) => writer.WriteStringValue(value.ToString());
}
