// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary>
///     An <see cref="IJsonReader"/> over a <see cref="JsonTape"/>: generated model code reads straight from the index, without re-tokenizing (<see cref="JsonItem.Deserialize{T}"/>).
///     The tape is already validated, so the only failures are type mismatches.
/// </summary>
public ref struct JsonTapeReader : IJsonReader, IDisposable
{
    private readonly JsonTape           __tape;
    private          int                __next; // the entry of the next value (or member name) to read
    private          int                __depth;
    private          int[]?             __frames; // per depth: [container entry, remaining children, current child entry]
    private          JsonError          __error;
    private          ValueStringBuilder __chars;
    private          ValueUtf8Builder   __bytes;


    /// <summary> <see langword="false"/>: a tape may hold either encoding, so check <see cref="JsonSpan.IsUtf8"/> on each span. </summary>
    public static bool IsUtf8 => false;

    public readonly int       Depth => __depth;
    public readonly JsonError Error => __error;


    /// <param name="tape"> A parsed, undisposed tape. </param>
    /// <param name="index"> The entry of the value to read (0 for the root). </param>
    public JsonTapeReader( JsonTape tape, int index )
    {
        ArgumentNullException.ThrowIfNull(tape);
        __tape   = tape;
        __next   = index;
        __frames = ArrayPool<int>.Shared.Rent(( tape.MaxDepth + 1 ) * 3);
        __chars  = new ValueStringBuilder(Span<char>.Empty);
        __bytes  = new ValueUtf8Builder(Span<byte>.Empty);
    }


    private readonly ref JsonTape.Entry Current => ref __tape.Entries[__next];


    public JsonTokenKind PeekKind() => __error.IsError || __next >= __tape.Count
                                           ? JsonTokenKind.None
                                           : Current.Kind;

    public bool TryReadStartObject() => TryOpen(JsonTokenKind.Object);

    public bool TryReadStartArray() => TryOpen(JsonTokenKind.Array);

    public bool TryReadProperty( out JsonSpan name, out bool end )
    {
        name = default;
        end  = false;
        if ( __error.IsError ) { return false; }

        if ( __depth == 0 || __tape.Entries[Frame(0)].Kind != JsonTokenKind.Object ) { return Fail(JsonErrorKind.UnexpectedToken); }

        if ( TryClose(out end) ) { return true; }

        ref readonly JsonTape.Entry entry = ref Current; // the name
        Frame(2) = __next;
        name     = Text(in entry);
        __next++;
        return true;
    }

    public bool TryReadNextElement( out bool end )
    {
        end = false;
        if ( __error.IsError ) { return false; }

        if ( __depth == 0 || __tape.Entries[Frame(0)].Kind != JsonTokenKind.Array ) { return Fail(JsonErrorKind.UnexpectedToken); }

        if ( TryClose(out end) ) { return true; }

        Frame(2) = __next;
        return true;
    }

    public bool TryReadNull()
    {
        if ( PeekKind() != JsonTokenKind.Null ) { return false; }

        __next++;
        return true;
    }

    public bool TryReadBoolean( out bool value )
    {
        value = false;

        switch ( PeekKind() )
        {
            case JsonTokenKind.True:
                value = true;
                __next++;
                return true;

            case JsonTokenKind.False:
                __next++;
                return true;

            default:
                return Fail(JsonErrorKind.UnexpectedToken);
        }
    }

    public bool TryReadInteger<T>( out T value, bool allowString = false )
        where T : struct, IBinaryInteger<T>
    {
        value = default;
        JsonTokenKind kind = PeekKind();

        if ( kind == JsonTokenKind.String && allowString )
        {
            JsonSpan text = Text(in Current);

            if ( !( text.IsUtf8
                        ? T.TryParse(text.Utf8,  NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value)
                        : T.TryParse(text.Utf16, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value) ) ) { return Fail(JsonErrorKind.InvalidNumber); }

            __next++;
            return true;
        }

        if ( kind != JsonTokenKind.Number ) { return Fail(JsonErrorKind.UnexpectedToken); }

        ref readonly JsonTape.Entry entry = ref Current;

        bool parsed = __tape.IsUtf8
                          ? T.TryParse(__tape.InputUtf8.Slice(entry.Start, entry.Length),  NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value)
                          : T.TryParse(__tape.InputUtf16.Slice(entry.Start, entry.Length), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value);

        if ( !parsed ) { return Fail(JsonErrorKind.NumberOverflow); }

        __next++;
        return true;
    }

    public bool TryReadFloat<T>( out T value, JsonFloatRead mode = JsonFloatRead.Strict )
        where T : struct, IFloatingPoint<T>
    {
        value = default;
        JsonTokenKind kind = PeekKind();

        if ( kind == JsonTokenKind.String && mode != JsonFloatRead.Strict )
        {
            JsonSpan text = Text(in Current);

            if ( ( mode & JsonFloatRead.AllowNonFinite ) != 0 && JsonFloats.TryParseNonFinite(text, out value, out bool supported) )
            {
                if ( !supported ) { return Fail(JsonErrorKind.NonFiniteNumber); }

                __next++;
                return true;
            }

            if ( ( mode & JsonFloatRead.AllowString ) == 0 ||
                 !( text.IsUtf8
                        ? T.TryParse(text.Utf8,  NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                        : T.TryParse(text.Utf16, NumberStyles.Float, CultureInfo.InvariantCulture, out value) ) ||
                 T.IsInfinity(value) ) { return Fail(JsonErrorKind.InvalidNumber); }

            __next++;
            return true;
        }

        if ( kind != JsonTokenKind.Number ) { return Fail(JsonErrorKind.UnexpectedToken); }

        ref readonly JsonTape.Entry entry = ref Current;

        bool parsed = __tape.IsUtf8
                          ? T.TryParse(__tape.InputUtf8.Slice(entry.Start, entry.Length),  NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                          : T.TryParse(__tape.InputUtf16.Slice(entry.Start, entry.Length), NumberStyles.Float, CultureInfo.InvariantCulture, out value);

        if ( !parsed || T.IsInfinity(value) ) { return Fail(JsonErrorKind.NumberOverflow); }

        __next++;
        return true;
    }

    public bool TryReadString( [NotNullWhen(true)] out string? value )
    {
        value = null;
        if ( PeekKind() != JsonTokenKind.String ) { return Fail(JsonErrorKind.UnexpectedToken); }

        value = __tape.GetString(in Current);
        __next++;
        return true;
    }

    public bool TryReadStringSpan( out JsonSpan value )
    {
        value = default;
        if ( PeekKind() != JsonTokenKind.String ) { return Fail(JsonErrorKind.UnexpectedToken); }

        value = Text(in Current);
        __next++;
        return true;
    }

    public bool TryReadRawNumber( out JsonSpan number )
    {
        number = default;
        if ( PeekKind() != JsonTokenKind.Number ) { return Fail(JsonErrorKind.UnexpectedToken); }

        ref readonly JsonTape.Entry entry = ref Current;

        number = __tape.IsUtf8
                     ? new JsonSpan(__tape.InputUtf8.Slice(entry.Start, entry.Length))
                     : new JsonSpan(__tape.InputUtf16.Slice(entry.Start, entry.Length));

        __next++;
        return true;
    }

    public bool TrySkipValue()
    {
        if ( PeekKind() == JsonTokenKind.None ) { return Fail(JsonErrorKind.UnexpectedEnd); }

        __next = Current.Next;
        return true;
    }

    public readonly JsonReaderCheckpoint Checkpoint() => new(__next,
                                                             __depth,
                                                             __depth > 0
                                                                 ? __frames![( __depth - 1 ) * 3 + 1]
                                                                 : 0);

    public void Rewind( in JsonReaderCheckpoint checkpoint )
    {
        __next  = checkpoint.Position;
        __depth = checkpoint.Depth;
        if ( __depth > 0 ) { Frame(1) = checkpoint.State; }
    }

    public bool Fail( JsonErrorKind kind )
    {
        if ( !__error.IsError )
        {
            int position = __next < __tape.Count
                               ? __tape.Entries[__next].Start
                               : __tape.Length;

            __error = __tape.CreateError(kind, position);
        }

        return false;
    }

    public readonly string GetPath()
    {
        ValueStringBuilder path = new(stackalloc char[128]);
        path.Append('$');

        for ( int depth = 1; depth <= __depth; depth++ )
        {
            int container = __frames![( depth - 1 ) * 3];
            int child     = __frames[( depth - 1 ) * 3 + 2];
            if ( child <= container ) { continue; } // nothing read yet at this level

            JsonTape.Entry[] entries = __tape.Entries;

            if ( entries[container].Kind == JsonTokenKind.Object )
            {
                JsonPath.AppendMember(ref path, __tape.GetString(in entries[child]));
                continue;
            }

            int index = 0;
            for ( int i = container + 1; i < child; i = entries[i].Next ) { index++; }

            path.Append('[').AppendSpanFormattable(index, default, CultureInfo.InvariantCulture).Append(']');
        }

        return path.ToString();
    }

    public void Dispose()
    {
        if ( __frames is not null ) { ArrayPool<int>.Shared.Return(__frames); }

        __frames = null;
        __chars.Dispose();
        __bytes.Dispose();
    }


    // ─── Helpers ─────────────────────────────────────────────────────────────

    private readonly ref int Frame( int slot ) => ref __frames![( __depth - 1 ) * 3 + slot];

    private bool TryOpen( JsonTokenKind kind )
    {
        if ( PeekKind() != kind ) { return Fail(JsonErrorKind.UnexpectedToken); }

        __depth++;
        Frame(0) = __next;
        Frame(1) = Current.Count;
        Frame(2) = __next;
        __next++;
        return true;
    }

    /// <summary> True (and <paramref name="end"/>) when the container has no children left; otherwise counts one off. </summary>
    private bool TryClose( out bool end )
    {
        ref int remaining = ref Frame(1);

        if ( remaining == 0 )
        {
            __next = __tape.Entries[Frame(0)].Next; // also skips anything the caller left unread
            __depth--;
            end = true;
            return true;
        }

        remaining--;
        end = false;
        return false;
    }

    /// <summary> A string or name entry's text: a slice of the input, or decoded into scratch space when escaped. </summary>
    private JsonSpan Text( in JsonTape.Entry entry )
    {
        if ( !entry.Escaped )
        {
            return __tape.IsUtf8
                       ? new JsonSpan(__tape.InputUtf8.Slice(entry.Start, entry.Length))
                       : new JsonSpan(__tape.InputUtf16.Slice(entry.Start, entry.Length));
        }

        if ( __tape.IsUtf8 )
        {
            __bytes.Reset();
            JsonText.UnescapeToUtf8(__tape.InputUtf8.Slice(entry.Start, entry.Length), ref __bytes);
            return new JsonSpan(__bytes.Values);
        }

        __chars.Reset();
        JsonLexer<char>.Unescape(__tape.InputUtf16.Slice(entry.Start, entry.Length), ref __chars);
        return new JsonSpan(__chars.Values);
    }
}
