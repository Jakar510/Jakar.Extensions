// Jakar.Json
// 10/02/2026

namespace Jakar.Extensions;


/// <summary>
///     Serializes <typeparamref name="T"/> as a single JSON string through <see cref="ISpanFormattable"/> / <see cref="ISpanParsable{TSelf}"/> (invariant culture).
///     <code> [JsonConverter(typeof(SerializeAsStringJsonConverter&lt;AppVersion&gt;))] </code>
/// </summary>
/// <remarks>
///     AOT-safe: the type argument is closed at compile time, so the System.Text.Json source generator instantiates it directly. Also works as a dictionary key.
///     <para> Lenient on read, like the Newtonsoft converter it replaces: a blank string reads as <c> default </c>, and a JSON number is parsed from its text. </para>
/// </remarks>
public sealed class SerializeAsStringJsonConverter<T> : JsonConverter<T>
    where T : ISpanParsable<T>, ISpanFormattable
{
    private const int STACK_LIMIT = 256;

    public static readonly SerializeAsStringJsonConverter<T> Instance = new();


    public override T? Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options ) =>
        reader.TokenType switch
        {
            JsonTokenType.Null   => default,
            JsonTokenType.String => ParseString(ref reader, false),
            JsonTokenType.Number => ParseNumber(ref reader),
            _                    => throw new JsonException($"Unexpected {reader.TokenType} when reading {typeof(T).Name}; expected a JSON string.")
        };


    public override void Write( Utf8JsonWriter writer, T value, JsonSerializerOptions options )
    {
        if ( value is null )
        {
            writer.WriteNullValue();
            return;
        }

        Span<char> buffer = stackalloc char[STACK_LIMIT];

        if ( value.TryFormat(buffer, out int written, default, CultureInfo.InvariantCulture) )
        {
            writer.WriteStringValue(buffer[..written]);
            return;
        }

        writer.WriteStringValue(value.ToString(null, CultureInfo.InvariantCulture));
    }


    public override T ReadAsPropertyName( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options ) => ParseString(ref reader, true) ?? throw new JsonException($"A {typeof(T).Name} property name can't be blank.");


    public override void WriteAsPropertyName( Utf8JsonWriter writer, T value, JsonSerializerOptions options )
    {
        Span<char> buffer = stackalloc char[STACK_LIMIT];

        if ( value.TryFormat(buffer, out int written, default, CultureInfo.InvariantCulture) )
        {
            writer.WritePropertyName(buffer[..written]);
            return;
        }

        writer.WritePropertyName(value.ToString(null, CultureInfo.InvariantCulture));
    }


    private static T ParseNumber( ref Utf8JsonReader reader )
    {
        // Number tokens are plain ASCII (no escapes), so the raw bytes are the text.
        ReadOnlySpan<byte> utf8 = reader.HasValueSequence
                                      ? reader.ValueSequence.ToArray()
                                      : reader.ValueSpan;

        Span<char> text = stackalloc char[utf8.Length];
        for ( int i = 0; i < utf8.Length; i++ ) { text[i] = (char)utf8[i]; }

        return Parse(text);
    }


    /// <summary> <see cref="ISpanParsable{TSelf}.Parse(ReadOnlySpan{char}, IFormatProvider)"/>, not TryParse: a type's TryParse may validate more strictly than its Parse (e.g. Email). </summary>
    private static T Parse( ReadOnlySpan<char> text )
    {
        try { return T.Parse(text, CultureInfo.InvariantCulture); }
        catch ( Exception e ) when ( e is FormatException or ArgumentException or OverflowException ) { throw new JsonException($"'{text}' is not a valid {typeof(T).Name}.", e); }
    }


    private static T? ParseString( ref Utf8JsonReader reader, bool required )
    {
        int length = reader.HasValueSequence
                         ? checked((int)reader.ValueSequence.Length)
                         : reader.ValueSpan.Length; // UTF-16 chars ≤ UTF-8 bytes, and escaped length ≥ unescaped length

        char[]?    rented = null;
        Span<char> buffer = length <= STACK_LIMIT
                                ? stackalloc char[STACK_LIMIT]
                                : rented = ArrayPool<char>.Shared.Rent(length);

        try
        {
            int                written = reader.CopyString(buffer);
            ReadOnlySpan<char> text    = buffer[..written].Trim();

            if ( text.IsEmpty )
            {
                return required
                           ? throw new JsonException($"A {typeof(T).Name} can't be blank.")
                           : default;
            }

            return Parse(text);
        }
        finally
        {
            if ( rented is not null ) { ArrayPool<char>.Shared.Return(rented); }
        }
    }
}
