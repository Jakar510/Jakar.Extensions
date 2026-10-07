// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary>
///     Token-level JSON scanning, shared by <see cref="JsonReader{TChar}"/> and <see cref="JsonTape"/>, for <see cref="char"/> (UTF-16) or <see cref="byte"/> (UTF-8) input.
///     Generic over the code unit; the JIT specializes it, and <c>typeof(TChar) == typeof(byte)</c> checks fold to constants.
/// </summary>
internal static class JsonLexer<TChar>
    where TChar : unmanaged, IBinaryInteger<TChar>
{
    public static readonly SearchValues<TChar> Whitespace = Create(" \t\r\n");
    public static readonly SearchValues<TChar> Digits     = Create("0123456789");
    public static readonly SearchValues<TChar> StringStops = Create(string.Create(34,
                                                                                  0,
                                                                                  static ( span, _ ) =>
                                                                                  {
                                                                                      span[0] = '"';
                                                                                      span[1] = '\\';
                                                                                      for ( int i = 0; i < 32; i++ ) { span[i + 2] = (char)i; } // raw control chars are invalid in strings
                                                                                  }));
    /// <summary> The surrogate range, as <see cref="SearchValues{T}"/>: <c>IndexOfAnyInRange</c> allocates until tier-1 code replaces it. </summary>
    public static readonly SearchValues<char> Surrogates = SearchValues.Create(string.Create(0x800,
                                                                                             0,
                                                                                             static ( span, _ ) =>
                                                                                             {
                                                                                                 for ( int i = 0; i < span.Length; i++ ) { span[i] = (char)( 0xD800 + i ); }
                                                                                             }));


    public static bool IsUtf8 { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => typeof(TChar) == typeof(byte); }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint U( TChar value ) => uint.CreateTruncating(value);

    /// <summary> The same ASCII literal in this encoding; the JIT keeps only the matching arm. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ReadOnlySpan<TChar> Literal( ReadOnlySpan<byte> utf8, ReadOnlySpan<char> utf16 ) => IsUtf8
                                                                                                                                                             ? MemoryMarshal.Cast<byte, TChar>(utf8)
                                                                                                                                                             : MemoryMarshal.Cast<char, TChar>(utf16);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TChar Char( char c ) => TChar.CreateTruncating(c);


    /// <summary> Whitespace, and <c>//</c> / <c>/* */</c> comments when they're allowed. </summary>
    public static JsonErrorKind SkipTrivia( ref ValueSpanReader<TChar> reader, bool comments )
    {
        while ( true )
        {
            reader.SkipWhile(Whitespace);
            if ( !comments || !reader.IsNext(Char('/')) ) { return JsonErrorKind.None; }

            if ( reader.TryReadExact(Literal("//"u8, "//")) )
            {
                if ( !reader.TryReadTo(Char('\n'), out _) ) { reader.Advance(reader.RemainingCount); }
            }
            else if ( reader.TryReadExact(Literal("/*"u8, "/*")) )
            {
                int end = reader.Remaining.IndexOf(Literal("*/"u8, "*/"));

                if ( end < 0 )
                {
                    reader.Advance(reader.RemainingCount);
                    return JsonErrorKind.UnexpectedEnd;
                }

                reader.Advance(end + 2);
            }
            else { return JsonErrorKind.UnexpectedToken; }
        }
    }


    /// <summary> Reads a string from its opening quote. Escapes are validated, not decoded. <paramref name="start"/> / <paramref name="length"/> are the content, without quotes. </summary>
    public static JsonErrorKind ReadString( ref ValueSpanReader<TChar> reader, int maxLength, out int start, out int length, out bool escaped )
    {
        reader.Advance(1); // the opening quote
        start   = reader.Position;
        length  = 0;
        escaped = false;

        while ( true )
        {
            int stop = reader.IndexOfAny(StringStops); // vectorized: everything up to the next quote, backslash or control char is plain text

            if ( stop < 0 )
            {
                reader.Advance(reader.RemainingCount);
                return JsonErrorKind.UnexpectedEnd;
            }

            reader.Advance(stop);
            uint c = U(reader.Peek());

            if ( c == '"' )
            {
                length = reader.Position - start;

                if ( length > maxLength )
                {
                    reader.Rewind(start);
                    return JsonErrorKind.DocumentTooLarge;
                }

                reader.Advance(1);

                // Raw (unescaped) lone surrogates are invalid UTF-16; escaped ones (\uD800) are legal JSON and round-trip any .NET string (§5.2).
                if ( !IsUtf8 && HasLoneSurrogate(MemoryMarshal.Cast<TChar, char>(reader.Slice(start, start + length))) )
                {
                    reader.Rewind(start);
                    return JsonErrorKind.InvalidString;
                }

                return JsonErrorKind.None;
            }

            if ( c != '\\' ) { return JsonErrorKind.InvalidString; } // a raw control character

            escaped = true;
            reader.Advance(1);
            if ( !reader.TryRead(out TChar escape) ) { return JsonErrorKind.UnexpectedEnd; }

            switch ( U(escape) )
            {
                case '"' or '\\' or '/' or 'b' or 'f' or 'n' or 'r' or 't':
                    continue;

                case 'u' when reader.TryRead(4, out ReadOnlySpan<TChar> hex) && IsHex(hex):
                    continue;

                default:
                    return JsonErrorKind.InvalidEscape;
            }
        }
    }


    /// <summary> <c>-? (0 | [1-9][0-9]*) (\.[0-9]+)? ([eE][+-]?[0-9]+)?</c>: no <c>+1</c>, <c>01</c>, <c>.5</c>, <c>1.</c>, hex, NaN or Infinity. </summary>
    public static JsonErrorKind ReadNumber( ref ValueSpanReader<TChar> reader )
    {
        reader.TryReadExact(Char('-'));

        if ( reader.TryReadExact(Char('0')) )
        {
            if ( reader.TryPeek(out TChar digit) && U(digit) - '0' <= 9 ) { return JsonErrorKind.InvalidNumber; } // a leading zero
        }
        else if ( reader.SkipWhile(Digits) == 0 )
        {
            return reader.End
                       ? JsonErrorKind.UnexpectedEnd
                       : JsonErrorKind.InvalidNumber;
        }

        if ( reader.TryReadExact(Char('.')) && reader.SkipWhile(Digits) == 0 )
        {
            return reader.End
                       ? JsonErrorKind.UnexpectedEnd
                       : JsonErrorKind.InvalidNumber;
        }

        if ( reader.TryPeek(out TChar e) && ( U(e) | 0x20 ) == 'e' )
        {
            reader.Advance(1);
            if ( !reader.TryReadExact(Char('+')) ) { reader.TryReadExact(Char('-')); }

            if ( reader.SkipWhile(Digits) == 0 )
            {
                return reader.End
                           ? JsonErrorKind.UnexpectedEnd
                           : JsonErrorKind.InvalidNumber;
            }
        }

        return JsonErrorKind.None;
    }

    /// <summary> Whether <paramref name="text"/> is exactly one JSON number. </summary>
    public static bool IsNumber( ReadOnlySpan<TChar> text )
    {
        ValueSpanReader<TChar> reader = new(text);
        return !text.IsEmpty && ReadNumber(ref reader) == JsonErrorKind.None && reader.End;
    }

    /// <summary> Whether a number token has a fraction or exponent. </summary>
    public static bool HasFractionOrExponent( ReadOnlySpan<TChar> number ) => number.IndexOfAny(Char('.'), Char('e'), Char('E')) >= 0;


    /// <summary> Decodes a validated string body (escapes, and UTF-8 → UTF-16) into <paramref name="builder"/>. Escaped lone surrogates are kept as is. </summary>
    public static void Unescape( ReadOnlySpan<TChar> source, ref ValueStringBuilder builder )
    {
        TChar backslash = Char('\\');

        while ( true )
        {
            int escape = source.IndexOf(backslash);

            AppendRun(escape < 0
                          ? source
                          : source[..escape],
                      ref builder);

            if ( escape < 0 ) { return; }

            uint c = U(source[escape + 1]);

            if ( c == 'u' )
            {
                builder.Append(HexChar(source.Slice(escape + 2, 4)));
                source = source[( escape + 6 )..];
                continue;
            }

            builder.Append(Unescaped(c));
            source = source[( escape + 2 )..];
        }
    }

    private static void AppendRun( ReadOnlySpan<TChar> run, ref ValueStringBuilder builder )
    {
        if ( !IsUtf8 )
        {
            builder.Append(MemoryMarshal.Cast<TChar, char>(run));
            return;
        }

        builder.EnsureCapacity(builder.Length + run.Length); // UTF-8 → UTF-16 never needs more chars than there are bytes
        Utf8.ToUtf16(MemoryMarshal.Cast<TChar, byte>(run), builder.Next, out _, out int written);
        builder.Length += written;
    }

    public static char HexChar( ReadOnlySpan<TChar> hex ) => (char)( HexValue(U(hex[0])) << 12 | HexValue(U(hex[1])) << 8 | HexValue(U(hex[2])) << 4 | HexValue(U(hex[3])) );

    public static char Unescaped( uint c ) => c switch
                                              {
                                                  'b' => '\b',
                                                  'f' => '\f',
                                                  'n' => '\n',
                                                  'r' => '\r',
                                                  't' => '\t',
                                                  _   => (char)c // '"', '\\', '/'
                                              };


    /// <summary> Line and column of <paramref name="position"/>: the failure path only (columns count code units). </summary>
    public static JsonError CreateError( JsonErrorKind kind, int position, ReadOnlySpan<TChar> input )
    {
        ReadOnlySpan<TChar> before  = input[..Math.Clamp(position, 0, input.Length)];
        TChar               newLine = Char('\n');
        return new JsonError(kind, position, before.Count(newLine) + 1, before.Length - before.LastIndexOf(newLine));
    }


    private static SearchValues<TChar> Create( params ReadOnlySpan<char> ascii )
    {
        if ( !IsUtf8 ) { return (SearchValues<TChar>)(object)SearchValues.Create(ascii); }

        Span<byte> bytes = stackalloc byte[ascii.Length];
        for ( int i = 0; i < ascii.Length; i++ ) { bytes[i] = (byte)ascii[i]; }

        return (SearchValues<TChar>)(object)SearchValues.Create(bytes);
    }

    private static bool IsHex( ReadOnlySpan<TChar> hex )
    {
        foreach ( TChar c in hex )
        {
            if ( HexValue(U(c)) < 0 ) { return false; }
        }

        return true;
    }

    public static int HexValue( uint c )
    {
        if ( c - '0' <= 9 ) { return (int)( c - '0' ); }

        uint lower = c | 0x20;

        return lower - 'a' <= 5
                   ? (int)( lower - 'a' + 10 )
                   : -1;
    }


    public static bool HasLoneSurrogate( ReadOnlySpan<char> text )
    {
        int index = text.IndexOfAny(Surrogates);

        while ( index >= 0 )
        {
            if ( !char.IsHighSurrogate(text[index]) || index + 1 >= text.Length || !char.IsLowSurrogate(text[index + 1]) ) { return true; }

            text  = text[( index + 2 )..];
            index = text.IndexOfAny(Surrogates);
        }

        return false;
    }
}



internal static partial class JsonText
{
    /// <summary> Failure path only: the offset of the first invalid UTF-8 sequence. </summary>
    public static int IndexOfInvalidUtf8( ReadOnlySpan<byte> input )
    {
        int position = input.IndexOfAnyExceptInRange((byte)0x00, (byte)0x7F); // skip the ASCII prefix
        if ( position < 0 ) { return input.Length; }

        while ( position < input.Length )
        {
            if ( Rune.DecodeFromUtf8(input[position..], out _, out int consumed) != OperationStatus.Done ) { return position; }

            position += consumed;
        }

        return position;
    }


    /// <summary> Decodes a validated UTF-8 string body into UTF-8 (escapes resolved). Escaped lone surrogates can't be UTF-8, so they become U+FFFD. </summary>
    public static void UnescapeToUtf8( ReadOnlySpan<byte> source, ref ValueUtf8Builder builder )
    {
        Span<byte> rune = stackalloc byte[4];

        while ( true )
        {
            int escape = source.IndexOf((byte)'\\');

            builder.Append(escape < 0
                               ? source
                               : source[..escape]);

            if ( escape < 0 ) { return; }

            uint c = source[escape + 1];

            if ( c != 'u' )
            {
                builder.Append((byte)JsonLexer<byte>.Unescaped(c));
                source = source[( escape + 2 )..];
                continue;
            }

            char high = JsonLexer<byte>.HexChar(source.Slice(escape + 2, 4));
            source = source[( escape + 6 )..];

            // A surrogate pair is two escapes: 😀.
            if ( char.IsHighSurrogate(high) && source.Length >= 6 && source[0] == '\\' && source[1] == 'u' )
            {
                char low = JsonLexer<byte>.HexChar(source.Slice(2, 4));

                if ( char.IsLowSurrogate(low) )
                {
                    builder.Append(rune[..new Rune(high, low).EncodeToUtf8(rune)]);
                    source = source[6..];
                    continue;
                }
            }

            builder.Append(high); // transcodes; a lone surrogate becomes U+FFFD
        }
    }
}
