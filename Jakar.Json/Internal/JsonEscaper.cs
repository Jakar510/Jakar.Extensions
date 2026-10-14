// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary>
///     Canonical string escaping (SPEC.md §5.2): <c>"</c>, <c>\</c> and U+0000–U+001F always, with the short forms <c>\b \f \n \r \t</c> and otherwise <c>\u00XX</c> (uppercase hex);
///     <c>/</c> never; lone surrogates as <c>\uXXXX</c>, so any .NET string round-trips. <see cref="JsonEscaping.AsciiOnly"/> and <see cref="JsonEscaping.HtmlSafe"/> add to the set.
/// </summary>
internal static class JsonEscaper
{
    private const string HEX = "0123456789ABCDEF";

    // Every set includes the surrogate range: a valid pair is copied as is, a lone surrogate is escaped.
    private static readonly SearchValues<char> __minimal   = SearchValues.Create(Build(static c => c < 0x20 || c is '"' or '\\'                                     || char.IsSurrogate(c)));
    private static readonly SearchValues<char> __asciiOnly = SearchValues.Create(Build(static c => c < 0x20 || c is '"' or '\\'                                     || c >= 0x7F));
    private static readonly SearchValues<char> __htmlSafe  = SearchValues.Create(Build(static c => c < 0x20 || c is '"' or '\\' or '<' or '>' or '&' or '\'' or '+' || char.IsSurrogate(c)));


    // UTF-8 stops for text formatted straight to bytes: any non-ASCII byte as well, so only plain ASCII passes (the UTF-16 path handles the rest).
    private static readonly SearchValues<byte> __minimalUtf8  = SearchValues.Create(BuildUtf8(static c => c < 0x20 || c is '"' or '\\'                                     || c >= 0x7F));
    private static readonly SearchValues<byte> __htmlSafeUtf8 = SearchValues.Create(BuildUtf8(static c => c < 0x20 || c is '"' or '\\' or '<' or '>' or '&' or '\'' or '+' || c >= 0x7F));


    /// <summary> The bytes that keep ASCII text from being written as is, under <paramref name="escaping"/>. </summary>
    public static SearchValues<byte> ForUtf8( JsonEscaping escaping ) => escaping == JsonEscaping.HtmlSafe
                                                                             ? __htmlSafeUtf8
                                                                             : __minimalUtf8;

    private static byte[] BuildUtf8( Func<int, bool> include )
    {
        List<byte> bytes = [];

        for ( int c = 0; c <= byte.MaxValue; c++ )
        {
            if ( include(c) ) { bytes.Add((byte)c); }
        }

        return [.. bytes];
    }


    public static SearchValues<char> For( JsonEscaping escaping ) => escaping switch
                                                                     {
                                                                         JsonEscaping.AsciiOnly => __asciiOnly,
                                                                         JsonEscaping.HtmlSafe  => __htmlSafe,
                                                                         _                      => __minimal
                                                                     };


    /// <summary> Writes <c>"value"</c>, escaped. Runs that need no escaping are bulk-copied. </summary>
    public static void Write( ref ValueStringBuilder builder, scoped ReadOnlySpan<char> value, SearchValues<char> stops, bool asciiOnly )
    {
        builder.EnsureCapacity(builder.Length + value.Length + 2);
        builder.Append('"');

        while ( true )
        {
            int index = value.IndexOfAny(stops);

            if ( index < 0 )
            {
                builder.Append(value);
                break;
            }

            builder.Append(value[..index]);
            int consumed = Escape(ref builder, value[index..], asciiOnly);
            value = value[( index + consumed )..];
        }

        builder.Append('"');
    }

    /// <inheritdoc cref="Write(ref ValueStringBuilder, ReadOnlySpan{char}, SearchValues{char}, bool)"/>
    public static void Write( ref ValueUtf8Builder builder, scoped ReadOnlySpan<char> value, SearchValues<char> stops, bool asciiOnly )
    {
        builder.EnsureCapacity(builder.Length + value.Length + 2);
        builder.Append((byte)'"');

        while ( true )
        {
            int index = value.IndexOfAny(stops);

            if ( index < 0 )
            {
                builder.Append(value); // transcoded; contains no lone surrogates (they're all stops)
                break;
            }

            builder.Append(value[..index]);
            int consumed = Escape(ref builder, value[index..], asciiOnly);
            value = value[( index + consumed )..];
        }

        builder.Append((byte)'"');
    }


    /// <summary> Escapes (or copies, for a valid surrogate pair) the char at the start of <paramref name="rest"/>; returns how many chars it consumed. </summary>
    private static int Escape( ref ValueStringBuilder builder, scoped ReadOnlySpan<char> rest, bool asciiOnly )
    {
        char c = rest[0];

        if ( char.IsHighSurrogate(c) && rest.Length > 1 && char.IsLowSurrogate(rest[1]) )
        {
            if ( asciiOnly )
            {
                AppendUnicode(ref builder, c);
                AppendUnicode(ref builder, rest[1]);
            }
            else { builder.Append(rest[..2]); }

            return 2;
        }

        switch ( c )
        {
            case '"':
                builder.Append("\\\"");
                break;

            case '\\':
                builder.Append("\\\\");
                break;

            case '\b':
                builder.Append("\\b");
                break;

            case '\f':
                builder.Append("\\f");
                break;

            case '\n':
                builder.Append("\\n");
                break;

            case '\r':
                builder.Append("\\r");
                break;

            case '\t':
                builder.Append("\\t");
                break;

            default:
                AppendUnicode(ref builder, c);
                break;
        }

        return 1;
    }

    private static int Escape( ref ValueUtf8Builder builder, scoped ReadOnlySpan<char> rest, bool asciiOnly )
    {
        char c = rest[0];

        if ( char.IsHighSurrogate(c) && rest.Length > 1 && char.IsLowSurrogate(rest[1]) )
        {
            if ( asciiOnly )
            {
                AppendUnicode(ref builder, c);
                AppendUnicode(ref builder, rest[1]);
            }
            else { builder.Append(rest[..2]); }

            return 2;
        }

        switch ( c )
        {
            case '"':
                builder.Append("\\\""u8);
                break;

            case '\\':
                builder.Append("\\\\"u8);
                break;

            case '\b':
                builder.Append("\\b"u8);
                break;

            case '\f':
                builder.Append("\\f"u8);
                break;

            case '\n':
                builder.Append("\\n"u8);
                break;

            case '\r':
                builder.Append("\\r"u8);
                break;

            case '\t':
                builder.Append("\\t"u8);
                break;

            default:
                AppendUnicode(ref builder, c);
                break;
        }

        return 1;
    }

    private static void AppendUnicode( ref ValueStringBuilder builder, char c )
    {
        Span<char> escape = ['\\', 'u', HEX[c >> 12], HEX[( c >> 8 ) & 0xF], HEX[( c >> 4 ) & 0xF], HEX[c & 0xF]];
        builder.Append(escape);
    }

    private static void AppendUnicode( ref ValueUtf8Builder builder, char c )
    {
        Span<byte> escape = [(byte)'\\', (byte)'u', (byte)HEX[c >> 12], (byte)HEX[( c >> 8 ) & 0xF], (byte)HEX[( c >> 4 ) & 0xF], (byte)HEX[c & 0xF]];
        builder.Append(escape);
    }


    private static string Build( Func<char, bool> include )
    {
        StringBuilder chars = new();

        for ( int c = 0; c <= char.MaxValue; c++ )
        {
            if ( include((char)c) ) { chars.Append((char)c); }
        }

        return chars.ToString();
    }
}



/// <summary> Canonical number text (SPEC.md §5.2). </summary>
internal static class JsonNumbers
{
    /// <summary> Formats <paramref name="value"/> shortest-round-trippable into <paramref name="destination"/>, with the canonical exponent (<c>1e+20</c>, <c>1e-7</c>). </summary>
    public static bool TryFormatFloat<T, TChar>( T value, Span<TChar> destination, out int written )
        where T : IFloatingPoint<T>
        where TChar : unmanaged, IBinaryInteger<TChar>
    {
        if ( typeof(T) == typeof(decimal) && TryFormatDecimal(Unsafe.As<T, decimal>(ref value), destination, out written) ) { return true; }

        ReadOnlySpan<char> format = typeof(T) == typeof(decimal)
                                        ? default
                                        : "R"; // decimal keeps its scale (1.10 stays 1.10) and never uses an exponent

        bool formatted = typeof(TChar) == typeof(byte)
                             ? value.TryFormat(MemoryMarshal.Cast<TChar, byte>(destination), out written, format, CultureInfo.InvariantCulture)
                             : value.TryFormat(MemoryMarshal.Cast<TChar, char>(destination), out written, format, CultureInfo.InvariantCulture);

        if ( formatted ) { written = CanonicalizeExponent(destination[..written]); }

        return formatted;
    }

    /// <summary>
    ///     A <see cref="decimal"/> whose 96-bit mantissa fits a <see cref="ulong"/>, written as the BCL writes it with the invariant culture and no format: the digits, with a point
    ///     <c>scale</c> digits from the right (<c>1.10</c> keeps its zero, <c>0.005</c> gets its leading zeros). <see langword="false"/> (nothing written) for anything else, including zero
    ///     (the BCL decides its sign) and a destination that's too small: the caller falls back to <see cref="decimal.TryFormat(Span{char}, out int, ReadOnlySpan{char}, IFormatProvider?)"/>.
    /// </summary>
    private static bool TryFormatDecimal<TChar>( decimal value, Span<TChar> destination, out int written )
        where TChar : unmanaged, IBinaryInteger<TChar>
    {
        written = 0;

        Span<int> bits = stackalloc int[4];
        decimal.GetBits(value, bits);
        if ( bits[2] != 0 ) { return false; }

        ulong mantissa = (uint)bits[1] * ( 1UL << 32 ) + (uint)bits[0];
        if ( mantissa == 0 ) { return false; }

        int  scale    = ( bits[3] >> 16 ) & 0xFF;
        bool negative = bits[3] < 0;

        Span<byte> digits = stackalloc byte[20]; // ulong.MaxValue has 20 digits; filled from the right
        int        count  = 0;

        while ( mantissa != 0 )
        {
            ( mantissa, ulong digit ) = Math.DivRem(mantissa, 10UL);
            digits[19 - count++] = (byte)( '0' + digit );
        }

        ReadOnlySpan<byte> text = digits[( 20 - count )..];

        int length = ( negative ? 1 : 0 ) + ( scale == 0
                                                  ? count
                                                  : count > scale
                                                      ? count + 1
                                                      : 2 + scale );

        if ( destination.Length < length ) { return false; }

        int at = 0;
        if ( negative ) { destination[at++] = TChar.CreateTruncating('-'); }

        if ( scale == 0 )
        {
            for ( int i = 0; i < count; i++ ) { destination[at++] = TChar.CreateTruncating(text[i]); }
        }
        else if ( count > scale )
        {
            int whole = count - scale;
            for ( int i = 0; i < whole; i++ ) { destination[at++] = TChar.CreateTruncating(text[i]); }

            destination[at++] = TChar.CreateTruncating('.');
            for ( int i = whole; i < count; i++ ) { destination[at++] = TChar.CreateTruncating(text[i]); }
        }
        else
        {
            destination[at++] = TChar.CreateTruncating('0');
            destination[at++] = TChar.CreateTruncating('.');
            for ( int i = count; i < scale; i++ ) { destination[at++] = TChar.CreateTruncating('0'); }

            for ( int i = 0; i < count; i++ ) { destination[at++] = TChar.CreateTruncating(text[i]); }
        }

        written = at;
        return true;
    }


    /// <summary> <c>1E+20</c> → <c>1e+20</c>, <c>1E-07</c> → <c>1e-7</c>: lowercase <c>e</c>, explicit sign, no zero padding. Returns the new length. </summary>
    public static int CanonicalizeExponent<TChar>( Span<TChar> text )
        where TChar : unmanaged, IBinaryInteger<TChar>
    {
        int e = text.IndexOfAny(TChar.CreateTruncating('E'), TChar.CreateTruncating('e'));
        if ( e < 0 ) { return text.Length; }

        text[e] = TChar.CreateTruncating('e');

        int digits = e + 1; // .NET's "R" always writes the exponent's sign
        if ( digits < text.Length && uint.CreateTruncating(text[digits]) is '+' or '-' ) { digits++; }

        int zeros = 0;
        while ( digits + zeros < text.Length - 1 && uint.CreateTruncating(text[digits + zeros]) == '0' ) { zeros++; }

        if ( zeros == 0 ) { return text.Length; }

        text[( digits + zeros )..].CopyTo(text[digits..]);
        return text.Length - zeros;
    }


    public static string NonFiniteName<T>( T value )
        where T : IFloatingPoint<T> => T.IsNaN(value)
                                           ? "NaN"
                                           : T.IsNegative(value)
                                               ? "-Infinity"
                                               : "Infinity";

    /// <summary> ±(2^53 − 1): the integers JavaScript's numbers hold exactly (<c>LargeIntegers = String</c>). </summary>
    public static bool IsSafeInteger<T>( T value )
        where T : IBinaryInteger<T>
    {
        const long MAX_SAFE = ( 1L << 53 ) - 1;
        return T.CreateSaturating(MAX_SAFE) >= value && T.CreateSaturating(-MAX_SAFE) <= value;
    }
}
