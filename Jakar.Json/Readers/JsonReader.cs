// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary>
///     A strict, forward-only JSON reader over UTF-16 (<c>JsonReader&lt;char&gt;</c>) or UTF-8 (<c>JsonReader&lt;byte&gt;</c>) input, built on <see cref="ValueSpanReader{T}"/>.
///     Nothing is allocated except strings you ask for; escaped strings are decoded into pooled scratch space that <see cref="Dispose"/> returns.
/// </summary>
/// <remarks>
///     <para> Every <c>Try*</c> member returns <see langword="false"/> on failure and records the first error in <see cref="Error"/>; nothing throws on bad input. </para>
///     <para> RFC 8259 by default: comments and trailing commas only when <see cref="JsonReaderOptions"/> allow them; a leading byte order mark is skipped; invalid UTF-8 is rejected up front. </para>
/// </remarks>
public ref struct JsonReader<TChar> : IJsonReader, IDisposable
    where TChar : unmanaged, IBinaryInteger<TChar>
{
    private          ValueSpanReader<TChar> __reader;
    private readonly int                    __maxDepth;
    private readonly int                    __maxStringLength;
    private readonly bool                   __comments;
    private readonly bool                   __trailingCommas;
    private          BitStack               __isObject;
    private          BitStack               __first;
    private          PathStack              __marks; // objects: offset of the current name's quote; arrays: the current index
    private          int                    __depth;
    private          JsonError              __error;
    private          ValueStringBuilder     __chars; // unescape scratch
    private          ValueUtf8Builder       __bytes; // unescape scratch for UTF-8 names and string spans


    public static bool IsUtf8 => typeof(TChar) == typeof(byte);

    public readonly int       Depth    => __depth;
    public readonly JsonError Error    => __error;
    public readonly int       Position => __reader.Position;


    public JsonReader( ReadOnlySpan<TChar> input, JsonReaderOptions options = default )
    {
        if ( typeof(TChar) != typeof(byte) && typeof(TChar) != typeof(char) ) { throw new NotSupportedException("JsonReader reads UTF-16 (char) or UTF-8 (byte) input."); }

        __reader          = new ValueSpanReader<TChar>(input);
        __maxDepth        = options.MaxDepth;
        __maxStringLength = options.MaxStringLength;
        __comments        = options.AllowComments;
        __trailingCommas  = options.AllowTrailingCommas;
        __chars           = new ValueStringBuilder(Span<char>.Empty);
        __bytes           = new ValueUtf8Builder(Span<byte>.Empty);

        if ( IsUtf8 && !Utf8.IsValid(MemoryMarshal.Cast<TChar, byte>(input)) )
        {
            __error = JsonLexer<TChar>.CreateError(JsonErrorKind.InvalidUtf8, JsonText.IndexOfInvalidUtf8(MemoryMarshal.Cast<TChar, byte>(input)), input);
            return;
        }

        __reader.TryReadExact(JsonLexer<TChar>.Literal("﻿"u8, "﻿")); // a leading byte order mark
    }


    // ─── Structure ───────────────────────────────────────────────────────────

    public JsonTokenKind PeekKind()
    {
        if ( !Begin() || !__reader.TryPeek(out TChar next) ) { return JsonTokenKind.None; }

        return JsonLexer<TChar>.U(next) switch
               {
                   '{'                        => JsonTokenKind.Object,
                   '['                        => JsonTokenKind.Array,
                   '"'                        => JsonTokenKind.String,
                   '-' or (>= '0' and <= '9') => JsonTokenKind.Number,
                   't'                        => JsonTokenKind.True,
                   'f'                        => JsonTokenKind.False,
                   'n'                        => JsonTokenKind.Null,
                   _                          => JsonTokenKind.None
               };
    }

    public bool TryReadStartObject() => TryOpen(true, '{');

    public bool TryReadStartArray() => TryOpen(false, '[');

    public bool TryReadProperty( out JsonSpan name, out bool end )
    {
        name = default;
        end  = false;
        if ( !Begin() ) { return false; }

        if ( __depth == 0 || !__isObject.Get(__depth) ) { return Fail(JsonErrorKind.UnexpectedToken); }

        if ( TryClose('}', out end) ) { return true; }

        if ( end ) { return false; } // a separator error was recorded

        if ( !__reader.IsNext(JsonLexer<TChar>.Char('"')) ) { return FailToken(); }

        if ( __depth <= PathStack.LENGTH ) { __marks[__depth - 1] = __reader.Position; }

        if ( !TryReadStringContent(out name) ) { return false; }

        JsonErrorKind trivia = JsonLexer<TChar>.SkipTrivia(ref __reader, __comments);
        if ( trivia != JsonErrorKind.None ) { return Fail(trivia); }

        return __reader.TryReadExact(JsonLexer<TChar>.Char(':')) || FailToken();
    }

    public bool TryReadNextElement( out bool end )
    {
        end = false;
        if ( !Begin() ) { return false; }

        if ( __depth == 0 || __isObject.Get(__depth) ) { return Fail(JsonErrorKind.UnexpectedToken); }

        bool first = __first.Get(__depth);
        if ( TryClose(']', out end) ) { return true; }

        if ( end ) { return false; }

        if ( __depth <= PathStack.LENGTH )
        {
            if ( first ) { __marks[__depth - 1] = 0; }
            else { __marks[__depth         - 1]++; }
        }

        return true;
    }


    // ─── Scalars ─────────────────────────────────────────────────────────────

    public bool TryReadNull() => Begin() && __reader.TryReadExact(JsonLexer<TChar>.Literal("null"u8, "null"));

    public bool TryReadBoolean( out bool value )
    {
        value = false;
        if ( !Begin() ) { return false; }

        if ( __reader.TryReadExact(JsonLexer<TChar>.Literal("true"u8, "true")) )
        {
            value = true;
            return true;
        }

        return __reader.TryReadExact(JsonLexer<TChar>.Literal("false"u8, "false")) || FailToken();
    }

    public bool TryReadInteger<T>( out T value, bool allowString = false )
        where T : struct, IBinaryInteger<T>
    {
        value = default;
        if ( !Begin() ) { return false; }

        int start = __reader.Position;

        if ( __reader.IsNext(JsonLexer<TChar>.Char('"')) )
        {
            if ( !allowString ) { return FailToken(); }

            if ( !TryReadStringContent(out JsonSpan text) ) { return false; }

            if ( text.IsUtf8
                     ? T.TryParse(text.Utf8,  NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value)
                     : T.TryParse(text.Utf16, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value) ) { return true; }

            return FailAt(start, JsonErrorKind.InvalidNumber);
        }

        if ( !TryReadNumberToken(out ReadOnlySpan<TChar> number) ) { return false; }

        if ( TryParse(number, NumberStyles.AllowLeadingSign, out value) ) { return true; }

        return FailAt(start,
                      JsonLexer<TChar>.HasFractionOrExponent(number)
                          ? JsonErrorKind.InvalidNumber
                          : JsonErrorKind.NumberOverflow);
    }

    public bool TryReadFloat<T>( out T value, JsonFloatRead mode = JsonFloatRead.Strict )
        where T : struct, IFloatingPoint<T>
    {
        value = default;
        if ( !Begin() ) { return false; }

        int start = __reader.Position;

        if ( __reader.IsNext(JsonLexer<TChar>.Char('"')) )
        {
            if ( ( mode & ( JsonFloatRead.AllowString | JsonFloatRead.AllowNonFinite ) ) == 0 ) { return FailToken(); }

            if ( !TryReadStringContent(out JsonSpan text) ) { return false; }

            if ( ( mode & JsonFloatRead.AllowNonFinite ) != 0 && JsonFloats.TryParseNonFinite(text, out value, out bool supported) ) { return supported || FailAt(start, JsonErrorKind.NonFiniteNumber); }

            if ( ( mode & JsonFloatRead.AllowString ) != 0 &&
                 ( text.IsUtf8
                       ? T.TryParse(text.Utf8,  NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                       : T.TryParse(text.Utf16, NumberStyles.Float, CultureInfo.InvariantCulture, out value) ) &&
                 !T.IsInfinity(value) ) { return true; }

            return FailAt(start, JsonErrorKind.InvalidNumber);
        }

        if ( !TryReadNumberToken(out ReadOnlySpan<TChar> number) ) { return false; }

        if ( TryParse(number, NumberStyles.Float, out value) && !T.IsInfinity(value) ) { return true; }

        return FailAt(start, JsonErrorKind.NumberOverflow);
    }

    public bool TryReadString( [NotNullWhen(true)] out string? value )
    {
        value = null;
        if ( !Begin() ) { return false; }

        if ( !__reader.IsNext(JsonLexer<TChar>.Char('"')) ) { return FailToken(); }

        JsonErrorKind error = JsonLexer<TChar>.ReadString(ref __reader, __maxStringLength, out int start, out int length, out bool escaped);
        if ( error != JsonErrorKind.None ) { return Fail(error); }

        ReadOnlySpan<TChar> raw = __reader.Slice(start, start + length);

        if ( !escaped )
        {
            value = IsUtf8
                        ? Encoding.UTF8.GetString(MemoryMarshal.Cast<TChar, byte>(raw))
                        : new string(MemoryMarshal.Cast<TChar, char>(raw));

            return true;
        }

        __chars.Reset();
        JsonLexer<TChar>.Unescape(raw, ref __chars);
        value = new string(__chars.Values);
        return true;
    }

    public bool TryReadStringSpan( out JsonSpan value )
    {
        value = default;
        if ( !Begin() ) { return false; }

        return __reader.IsNext(JsonLexer<TChar>.Char('"'))
                   ? TryReadStringContent(out value)
                   : FailToken();
    }

    public bool TryReadRawNumber( out JsonSpan number )
    {
        number = default;
        if ( !Begin() || !TryReadNumberToken(out ReadOnlySpan<TChar> raw) ) { return false; }

        number = Span(raw);
        return true;
    }


    // ─── Skip ────────────────────────────────────────────────────────────────

    /// <summary> Skips one value of any kind, validating it, without recursion: nesting is tracked in an inline bit stack. </summary>
    public bool TrySkipValue()
    {
        if ( !Begin() ) { return false; }

        BitStack isObject = default;
        int      depth    = 0;
        bool     value    = true;

        while ( true )
        {
            JsonErrorKind trivia = JsonLexer<TChar>.SkipTrivia(ref __reader, __comments);
            if ( trivia != JsonErrorKind.None ) { return Fail(trivia); }

            if ( value )
            {
                if ( !__reader.TryPeek(out TChar next) ) { return Fail(JsonErrorKind.UnexpectedEnd); }

                uint c = JsonLexer<TChar>.U(next);

                if ( c is '{' or '[' )
                {
                    if ( __depth + depth == __maxDepth ) { return Fail(JsonErrorKind.DepthExceeded); }

                    bool obj = c == '{';
                    __reader.Advance(1);
                    isObject.Set(++depth, obj);

                    trivia = JsonLexer<TChar>.SkipTrivia(ref __reader, __comments);
                    if ( trivia != JsonErrorKind.None ) { return Fail(trivia); }

                    if ( __reader.TryReadExact(JsonLexer<TChar>.Char(obj
                                                                         ? '}'
                                                                         : ']')) )
                    {
                        depth--;
                        value = false;
                        continue;
                    }

                    if ( obj && !TrySkipName() ) { return false; }

                    continue;
                }

                if ( !TrySkipScalar(c) ) { return false; }

                value = false;
                continue;
            }

            if ( depth == 0 ) { return true; }

            bool inObject = isObject.Get(depth);

            char close = inObject
                             ? '}'
                             : ']';

            if ( __reader.TryReadExact(JsonLexer<TChar>.Char(',')) )
            {
                trivia = JsonLexer<TChar>.SkipTrivia(ref __reader, __comments);
                if ( trivia != JsonErrorKind.None ) { return Fail(trivia); }

                if ( __trailingCommas && __reader.TryReadExact(JsonLexer<TChar>.Char(close)) )
                {
                    depth--;
                    continue;
                }

                if ( inObject && !TrySkipName() ) { return false; }

                value = true;
                continue;
            }

            if ( __reader.TryReadExact(JsonLexer<TChar>.Char(close)) )
            {
                depth--;
                continue;
            }

            return FailToken();
        }
    }

    private bool TrySkipName()
    {
        if ( !__reader.IsNext(JsonLexer<TChar>.Char('"')) ) { return FailToken(); }

        JsonErrorKind error = JsonLexer<TChar>.ReadString(ref __reader, __maxStringLength, out _, out _, out _);
        if ( error != JsonErrorKind.None ) { return Fail(error); }

        error = JsonLexer<TChar>.SkipTrivia(ref __reader, __comments);
        if ( error != JsonErrorKind.None ) { return Fail(error); }

        return __reader.TryReadExact(JsonLexer<TChar>.Char(':')) || FailToken();
    }

    private bool TrySkipScalar( uint c )
    {
        JsonErrorKind error;

        switch ( c )
        {
            case '"':
                error = JsonLexer<TChar>.ReadString(ref __reader, __maxStringLength, out _, out _, out _);
                return error == JsonErrorKind.None || Fail(error);

            case '-' or (>= '0' and <= '9'):
                error = JsonLexer<TChar>.ReadNumber(ref __reader);
                return error == JsonErrorKind.None || Fail(error);

            case 't' when __reader.TryReadExact(JsonLexer<TChar>.Literal("true"u8,  "true")):
            case 'f' when __reader.TryReadExact(JsonLexer<TChar>.Literal("false"u8, "false")):
            case 'n' when __reader.TryReadExact(JsonLexer<TChar>.Literal("null"u8,  "null")):
                return true;

            default:
                return FailToken();
        }
    }


    // ─── Checkpoints, end, errors ────────────────────────────────────────────

    public readonly JsonReaderCheckpoint Checkpoint()
    {
        int mark = __depth is > 0 and <= PathStack.LENGTH
                       ? __marks[__depth - 1]
                       : 0;

        return new JsonReaderCheckpoint(__reader.Position,
                                        __depth,
                                        ( __first.Get(__depth)
                                              ? 1
                                              : 0 ) |
                                        ( mark << 1 ));
    }

    /// <summary> Returns to <paramref name="checkpoint"/>, taken earlier in the same container (or an enclosing one this reader hasn't left). </summary>
    public void Rewind( in JsonReaderCheckpoint checkpoint )
    {
        __reader.Rewind(checkpoint.Position);
        __depth = checkpoint.Depth;
        __first.Set(__depth, ( checkpoint.State & 1 ) != 0);
        if ( __depth is > 0 and <= PathStack.LENGTH ) { __marks[__depth - 1] = checkpoint.State >> 1; }
    }

    /// <summary> After the root value: only trivia may remain. </summary>
    public bool TryReadEnd()
    {
        if ( !Begin() ) { return false; }

        return __reader.End || Fail(JsonErrorKind.TrailingData);
    }

    public bool Fail( JsonErrorKind kind )
    {
        if ( !__error.IsError ) { __error = JsonLexer<TChar>.CreateError(kind, __reader.Position, __reader.Span); }

        return false;
    }

    public readonly string GetPath()
    {
        ValueStringBuilder path = new(stackalloc char[128]);
        ValueStringBuilder text = new(stackalloc char[64]);
        path.Append('$');

        for ( int depth = 1; depth <= __depth && depth <= PathStack.LENGTH; depth++ )
        {
            bool started = !__first.Get(depth);
            if ( !started ) { continue; }

            if ( !__isObject.Get(depth) )
            {
                path.Append('[').AppendSpanFormattable(__marks[depth - 1], default, CultureInfo.InvariantCulture).Append(']');
                continue;
            }

            // Re-read the member name at its recorded quote (failure path only).
            ValueSpanReader<TChar> name = new(__reader.Span);
            name.Advance(__marks[depth - 1]);

            if ( JsonLexer<TChar>.ReadString(ref name, int.MaxValue, out int start, out int length, out _) != JsonErrorKind.None ) { break; }

            text.Reset();
            JsonLexer<TChar>.Unescape(__reader.Span.Slice(start, length), ref text);
            JsonPath.AppendMember(ref path, text.Values);
        }

        text.Dispose();

        if ( __depth > PathStack.LENGTH ) { path.Append("..."); }

        return path.ToString();
    }

    /// <summary> A <see cref="JsonReadException"/> for the recorded error, with its path. </summary>
    public readonly JsonReadException CreateException() => new(__error.IsError
                                                                   ? __error
                                                                   : new JsonError(JsonErrorKind.UnexpectedToken, __reader.Position, 0, 0),
                                                               GetPath());

    public void Dispose()
    {
        __chars.Dispose();
        __bytes.Dispose();
    }


    // ─── Helpers ─────────────────────────────────────────────────────────────

    /// <summary> Skips trivia before a token; <see langword="false"/> after an earlier error. </summary>
    private bool Begin()
    {
        if ( __error.IsError ) { return false; }

        JsonErrorKind trivia = JsonLexer<TChar>.SkipTrivia(ref __reader, __comments);
        return trivia == JsonErrorKind.None || Fail(trivia);
    }

    private bool TryOpen( bool isObject, char open )
    {
        if ( !Begin() ) { return false; }

        if ( !__reader.IsNext(JsonLexer<TChar>.Char(open)) ) { return FailToken(); }

        if ( __depth == __maxDepth ) { return Fail(JsonErrorKind.DepthExceeded); }

        __reader.Advance(1);
        __depth++;
        __isObject.Set(__depth, isObject);
        __first.Set(__depth, true);
        return true;
    }

    /// <summary> At a member or element boundary: reads the closer (<paramref name="end"/> = true, returns true) or the separator (returns false with end = false). On a separator error, returns false with end = true. </summary>
    private bool TryClose( char close, out bool end )
    {
        end = false;

        if ( __reader.TryReadExact(JsonLexer<TChar>.Char(close)) )
        {
            __depth--;
            end = true;
            return true;
        }

        if ( __first.Get(__depth) )
        {
            __first.Set(__depth, false);
            return false;
        }

        if ( !__reader.TryReadExact(JsonLexer<TChar>.Char(',')) )
        {
            FailToken();
            end = true;
            return false;
        }

        JsonErrorKind trivia = JsonLexer<TChar>.SkipTrivia(ref __reader, __comments);

        if ( trivia != JsonErrorKind.None )
        {
            Fail(trivia);
            end = true;
            return false;
        }

        if ( __trailingCommas && __reader.TryReadExact(JsonLexer<TChar>.Char(close)) )
        {
            __depth--;
            end = true;
            return true;
        }

        return false;
    }

    private bool TryReadStringContent( out JsonSpan value )
    {
        value = default;
        JsonErrorKind error = JsonLexer<TChar>.ReadString(ref __reader, __maxStringLength, out int start, out int length, out bool escaped);
        if ( error != JsonErrorKind.None ) { return Fail(error); }

        ReadOnlySpan<TChar> raw = __reader.Slice(start, start + length);

        if ( !escaped )
        {
            value = Span(raw);
            return true;
        }

        if ( IsUtf8 )
        {
            __bytes.Reset();
            JsonText.UnescapeToUtf8(MemoryMarshal.Cast<TChar, byte>(raw), ref __bytes);
            value = new JsonSpan(__bytes.Values);
            return true;
        }

        __chars.Reset();
        JsonLexer<TChar>.Unescape(raw, ref __chars);
        value = new JsonSpan(__chars.Values);
        return true;
    }

    private bool TryReadNumberToken( out ReadOnlySpan<TChar> number )
    {
        number = default;
        int start = __reader.Position;

        if ( !__reader.TryPeek(out TChar next) || JsonLexer<TChar>.U(next) is not ('-' or (>= '0' and <= '9')) ) { return FailToken(); }

        JsonErrorKind error = JsonLexer<TChar>.ReadNumber(ref __reader);
        if ( error != JsonErrorKind.None ) { return Fail(error); }

        number = __reader.Slice(start, __reader.Position);
        return true;
    }

    private static bool TryParse<T>( ReadOnlySpan<TChar> number, NumberStyles style, out T value )
        where T : struct, INumberBase<T> => IsUtf8
                                                ? T.TryParse(MemoryMarshal.Cast<TChar, byte>(number), style, CultureInfo.InvariantCulture, out value)
                                                : T.TryParse(MemoryMarshal.Cast<TChar, char>(number), style, CultureInfo.InvariantCulture, out value);

    private static JsonSpan Span( ReadOnlySpan<TChar> raw ) => IsUtf8
                                                                   ? new JsonSpan(MemoryMarshal.Cast<TChar, byte>(raw))
                                                                   : new JsonSpan(MemoryMarshal.Cast<TChar, char>(raw));

    private bool FailToken() => Fail(__reader.End
                                         ? JsonErrorKind.UnexpectedEnd
                                         : JsonErrorKind.UnexpectedToken);

    private bool FailAt( int position, JsonErrorKind kind )
    {
        __reader.Rewind(position);
        return Fail(kind);
    }
}



internal static class JsonFloats
{
    /// <summary> <c>NaN</c>, <c>Infinity</c>, <c>-Infinity</c>. <paramref name="supported"/> is <see langword="false"/> for types without them (<see cref="decimal"/>). </summary>
    public static bool TryParseNonFinite<T>( scoped in JsonSpan text, out T value, out bool supported )
        where T : struct, IFloatingPoint<T>
    {
        value     = default;
        supported = typeof(T) != typeof(decimal);

        double special;

        if ( text.Equals("NaN") ) { special            = double.NaN; }
        else if ( text.Equals("Infinity") ) { special  = double.PositiveInfinity; }
        else if ( text.Equals("-Infinity") ) { special = double.NegativeInfinity; }
        else { return false; }

        if ( supported ) { value = T.CreateTruncating(special); }

        return true;
    }
}



internal static class JsonPath
{
    private static readonly SearchValues<char> __identifier = SearchValues.Create("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_$");

    /// <summary> <c>.name</c>, or <c>['odd name']</c> for names that aren't identifiers. </summary>
    public static void AppendMember( ref ValueStringBuilder path, scoped ReadOnlySpan<char> name )
    {
        if ( name.Length > 0 && !char.IsAsciiDigit(name[0]) && !name.ContainsAnyExcept(__identifier) )
        {
            path.Append('.').Append(name);
            return;
        }

        path.Append("['");

        foreach ( char c in name )
        {
            if ( c is '\'' or '\\' ) { path.Append('\\'); }

            path.Append(c);
        }

        path.Append("']");
    }
}
