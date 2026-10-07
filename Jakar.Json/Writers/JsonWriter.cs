// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary>
///     Writes UTF-16 JSON into a <see cref="ValueStringBuilder"/>: from a caller buffer (<c>new JsonWriter(stackalloc char[512], options)</c>) or pooled arrays.
///     Separators, the <c>" : "</c> name separator and indentation are inserted for you; nothing is allocated except <see cref="ToString"/>.
/// </summary>
/// <remarks> Not reusable after <see cref="ToString"/> or <see cref="Dispose"/>. Misuse (a value with no name inside an object, two roots, too deep) throws <see cref="JsonWriteException"/>. </remarks>
public ref struct JsonWriter : IJsonWriter, IDisposable
{
    private          ValueStringBuilder __builder;
    private readonly SearchValues<char> __escape;
    private readonly bool               __asciiOnly;
    private readonly bool               __indented;
    private readonly char               __indentChar;
    private readonly int                __indentSize;
    private readonly int                __maxDepth;
    private          BitStack           __hasItems;
    private          BitStack           __isObject;
    private          int                __depth;
    private          bool               __afterName;
    private          bool               __rootWritten;


    public static bool IsUtf8 => false;

    public readonly int Depth => __depth;

    /// <summary> The JSON written so far. </summary>
    public readonly ReadOnlySpan<char> Written => __builder.Values;

    /// <summary> A complete root value has been written. </summary>
    public readonly bool IsComplete => __depth == 0 && __rootWritten;


    public JsonWriter( JsonWriterOptions options ) : this(Span<char>.Empty, options) { }

    /// <param name="buffer"> Initial storage (typically <c>stackalloc char[N]</c>); replaced by pooled arrays only if the output outgrows it. </param>
    /// <param name="options"> Layout, escaping and depth. </param>
    public JsonWriter( Span<char> buffer, JsonWriterOptions options )
    {
        __builder    = new ValueStringBuilder(buffer);
        __escape     = JsonEscaper.For(options.Escaping);
        __asciiOnly  = options.Escaping == JsonEscaping.AsciiOnly;
        __indented   = options.Indented;
        __indentChar = options.IndentCharacter;
        __indentSize = options.IndentSize;
        __maxDepth   = options.MaxDepth;
    }


    // ─── Structure ───────────────────────────────────────────────────────────

    public void WriteStartObject()
    {
        BeforeValue();
        __builder.Append('{');
        Push(true);
    }

    public void WriteEndObject() => Pop(true, '}');

    public void WriteStartArray()
    {
        BeforeValue();
        __builder.Append('[');
        Push(false);
    }

    public void WriteEndArray() => Pop(false, ']');


    public void WritePropertyName( JsonName name )
    {
        BeforeName();
        __builder.Append(name.Utf16);
        __afterName = true;
    }

    public void WritePropertyName( scoped ReadOnlySpan<char> name )
    {
        BeforeName();
        JsonEscaper.Write(ref __builder, name, __escape, __asciiOnly);
        __builder.Append(" : ");
        __afterName = true;
    }


    // ─── Values ──────────────────────────────────────────────────────────────

    public void WriteNull()
    {
        BeforeValue();
        __builder.Append("null");
    }

    public void WriteBoolean( bool value )
    {
        BeforeValue();

        __builder.Append(value
                             ? "true"
                             : "false");
    }

    public void WriteString( scoped ReadOnlySpan<char> value )
    {
        BeforeValue();
        JsonEscaper.Write(ref __builder, value, __escape, __asciiOnly);
    }

    public void WriteInteger<T>( T value )
        where T : IBinaryInteger<T>
    {
        BeforeValue();
        __builder.AppendSpanFormattable(value, default, CultureInfo.InvariantCulture);
    }

    public void WriteFloat<T>( T value )
        where T : IFloatingPoint<T>
    {
        if ( !T.IsFinite(value) ) { throw NonFinite(JsonNumbers.NonFiniteName(value)); }

        BeforeValue();

        int written;
        while ( !JsonNumbers.TryFormatFloat(value, __builder.Next, out written) ) { __builder.EnsureCapacity(__builder.Capacity * 2 + 32); }

        __builder.Length += written;
    }

    public void WriteFormatted<T>( T value, scoped ReadOnlySpan<char> format = default )
        where T : ISpanFormattable
    {
        BeforeValue();
        Span<char> buffer = stackalloc char[128];

        if ( value.TryFormat(buffer, out int written, format, CultureInfo.InvariantCulture) )
        {
            JsonEscaper.Write(ref __builder, buffer[..written], __escape, __asciiOnly);
            return;
        }

        ValueStringBuilder text = new(512);

        try
        {
            text.AppendSpanFormattable(value, format, CultureInfo.InvariantCulture);
            JsonEscaper.Write(ref __builder, text.Values, __escape, __asciiOnly);
        }
        finally { text.Dispose(); }
    }

    public void WriteRawNumber( scoped ReadOnlySpan<char> number )
    {
        BeforeValue();
        __builder.Append(number);
    }


    // ─── Output ──────────────────────────────────────────────────────────────

    /// <summary> The JSON as a string (exactly one allocation, at its exact length); the writer is disposed. </summary>
    public override string ToString() => __builder.ToString();

    /// <summary> Copies the JSON to <paramref name="destination"/>; <see langword="false"/> (and 0) if it doesn't fit. </summary>
    public readonly bool TryCopyTo( Span<char> destination, out int charsWritten )
    {
        bool copied = __builder.Values.TryCopyTo(destination);

        charsWritten = copied
                           ? __builder.Length
                           : 0;

        return copied;
    }

    /// <summary> Whether the output still lives entirely in <paramref name="buffer"/> (the writer never outgrew the caller's storage). </summary>
    internal readonly bool IsStillIn( Span<char> buffer ) => Unsafe.AreSame(ref MemoryMarshal.GetReference(__builder.RawChars), ref MemoryMarshal.GetReference(buffer));

    public void Dispose() => __builder.Dispose();


    // ─── State ───────────────────────────────────────────────────────────────

    private void BeforeValue()
    {
        if ( __afterName )
        {
            __afterName = false;
            return;
        }

        if ( __depth == 0 )
        {
            if ( __rootWritten ) { throw Misuse("A JSON document has exactly one root value."); }

            __rootWritten = true;
            return;
        }

        if ( __isObject.Get(__depth) ) { throw Misuse("A value inside an object needs a property name first."); }

        Separate();
    }

    private void BeforeName()
    {
        if ( __depth == 0 || !__isObject.Get(__depth) ) { throw Misuse("Property names belong inside an object."); }

        if ( __afterName ) { throw Misuse("A property name needs a value before the next name."); }

        Separate();
    }

    private void Separate()
    {
        if ( __hasItems.Get(__depth) ) { __builder.Append(','); }
        else { __hasItems.Set(__depth, true); }

        if ( __indented ) { NewLine(__depth); }
    }

    private void Push( bool isObject )
    {
        if ( __depth == __maxDepth ) { throw new JsonWriteException(new JsonError(JsonErrorKind.DepthExceeded, __builder.Length, 0, 0), $"The JSON is nested more than {__maxDepth} levels deep (a cyclic object graph?)."); }

        __depth++;
        __hasItems.Set(__depth, false);
        __isObject.Set(__depth, isObject);
    }

    private void Pop( bool isObject, char close )
    {
        if ( __depth == 0 || __isObject.Get(__depth) != isObject ) { throw Misuse($"'{close}' doesn't close the current container."); }

        if ( __afterName ) { throw Misuse("A property name needs a value before the object ends."); }

        if ( __indented && __hasItems.Get(__depth) ) { NewLine(__depth - 1); }

        __builder.Append(close);
        __depth--;
    }

    private void NewLine( int depth )
    {
        __builder.Append('\n');
        __builder.Append(__indentChar, depth * __indentSize);
    }

    private readonly JsonWriteException Misuse( string message ) => new(new JsonError(JsonErrorKind.UnexpectedToken, __builder.Length, 0, 0), message);

    private readonly JsonWriteException NonFinite( string value ) => new(new JsonError(JsonErrorKind.NonFiniteNumber, __builder.Length, 0, 0), $"JSON has no {value}; use NonFiniteFloats = AsString to write it as a string.");
}
