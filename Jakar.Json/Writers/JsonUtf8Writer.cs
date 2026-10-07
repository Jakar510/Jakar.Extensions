// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary>
///     Writes UTF-8 JSON into a <see cref="ValueUtf8Builder"/>, or straight to a <see cref="Stream"/> in chunks. The output is byte for byte
///     <c>Encoding.UTF8.GetBytes</c> of <see cref="JsonWriter"/>'s (SPEC.md §5.1, I5).
/// </summary>
/// <remarks> Not reusable after <see cref="ToArray"/> or <see cref="Dispose"/>. Misuse throws <see cref="JsonWriteException"/>. </remarks>
public ref struct JsonUtf8Writer : IJsonWriter, IDisposable
{
    /// <summary> With a stream, the buffered output is flushed once it reaches this many bytes. </summary>
    public const int FLUSH_THRESHOLD = 16 * 1024;

    private          ValueUtf8Builder   __builder;
    private readonly Stream?            __stream;
    private readonly SearchValues<char> __escape;
    private readonly bool               __asciiOnly;
    private readonly bool               __indented;
    private readonly byte               __indentChar;
    private readonly int                __indentSize;
    private readonly int                __maxDepth;
    private          BitStack           __hasItems;
    private          BitStack           __isObject;
    private          int                __depth;
    private          bool               __afterName;
    private          bool               __rootWritten;


    public static bool IsUtf8 => true;

    public readonly int Depth => __depth;

    /// <summary> The JSON buffered so far (everything, unless writing to a stream). </summary>
    public readonly ReadOnlySpan<byte> Written => __builder.Values;

    public readonly bool IsComplete => __depth == 0 && __rootWritten;


    public JsonUtf8Writer( JsonWriterOptions options ) : this(Span<byte>.Empty, options) { }

    /// <param name="buffer"> Initial storage (typically <c>stackalloc byte[N]</c>); replaced by pooled arrays only if the output outgrows it. </param>
    /// <param name="options"> Layout, escaping and depth. </param>
    public JsonUtf8Writer( Span<byte> buffer, JsonWriterOptions options )
    {
        __builder    = new ValueUtf8Builder(buffer);
        __stream     = null;
        __escape     = JsonEscaper.For(options.Escaping);
        __asciiOnly  = options.Escaping == JsonEscaping.AsciiOnly;
        __indented   = options.Indented;
        __indentChar = (byte)options.IndentCharacter;
        __indentSize = options.IndentSize;
        __maxDepth   = options.MaxDepth;
    }

    /// <summary> Writes to <paramref name="stream"/> in <see cref="FLUSH_THRESHOLD"/>-byte chunks through a pooled buffer; call <see cref="Flush"/> at the end. </summary>
    public JsonUtf8Writer( Stream stream, JsonWriterOptions options ) : this(Span<byte>.Empty, options)
    {
        ArgumentNullException.ThrowIfNull(stream);
        __stream = stream;
    }


    // ─── Structure ───────────────────────────────────────────────────────────

    public void WriteStartObject()
    {
        BeforeValue();
        __builder.Append((byte)'{');
        Push(true);
    }

    public void WriteEndObject()
    {
        Pop(true, (byte)'}');
        MaybeFlush();
    }

    public void WriteStartArray()
    {
        BeforeValue();
        __builder.Append((byte)'[');
        Push(false);
    }

    public void WriteEndArray()
    {
        Pop(false, (byte)']');
        MaybeFlush();
    }


    public void WritePropertyName( JsonName name )
    {
        BeforeName();
        __builder.Append(name.Utf8);
        __afterName = true;
    }

    public void WritePropertyName( scoped ReadOnlySpan<char> name )
    {
        BeforeName();
        JsonEscaper.Write(ref __builder, name, __escape, __asciiOnly);
        __builder.Append(" : "u8);
        __afterName = true;
    }


    // ─── Values ──────────────────────────────────────────────────────────────

    public void WriteNull()
    {
        BeforeValue();
        __builder.Append("null"u8);
    }

    public void WriteBoolean( bool value )
    {
        BeforeValue();

        __builder.Append(value
                             ? "true"u8
                             : "false"u8);
    }

    public void WriteString( scoped ReadOnlySpan<char> value )
    {
        BeforeValue();
        JsonEscaper.Write(ref __builder, value, __escape, __asciiOnly);
        MaybeFlush();
    }

    public void WriteInteger<T>( T value )
        where T : IBinaryInteger<T>
    {
        BeforeValue();
        __builder.AppendUtf8Formattable(value, default, CultureInfo.InvariantCulture);
    }

    public void WriteFloat<T>( T value )
        where T : IFloatingPoint<T>
    {
        if ( !T.IsFinite(value) ) { throw new JsonWriteException(new JsonError(JsonErrorKind.NonFiniteNumber, __builder.Length, 0, 0), $"JSON has no {JsonNumbers.NonFiniteName(value)}; use NonFiniteFloats = AsString to write it as a string."); }

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
        __builder.Append(number); // ASCII
    }


    // ─── Output ──────────────────────────────────────────────────────────────

    /// <summary> The JSON as a new array (one allocation, at its exact length); the writer is disposed. </summary>
    public byte[] ToArray() => __builder.ToArray();

    /// <summary> The JSON decoded to a string; the writer is disposed. </summary>
    public override string ToString() => __builder.ToString();

    public readonly bool TryCopyTo( Span<byte> destination, out int bytesWritten )
    {
        bool copied = __builder.Values.TryCopyTo(destination);

        bytesWritten = copied
                           ? __builder.Length
                           : 0;

        return copied;
    }

    internal readonly bool IsStillIn( Span<byte> buffer ) => Unsafe.AreSame(ref MemoryMarshal.GetReference(__builder.RawBytes), ref MemoryMarshal.GetReference(buffer));

    /// <summary> Writes the buffered bytes to the stream (no-op without one). </summary>
    public void Flush()
    {
        if ( __stream is null || __builder.Length == 0 ) { return; }

        __stream.Write(__builder.Values);
        __builder.Reset();
    }

    public void Dispose() => __builder.Dispose();


    // ─── State ───────────────────────────────────────────────────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining)] private void MaybeFlush()
    {
        if ( __stream is not null && __builder.Length >= FLUSH_THRESHOLD ) { Flush(); }
    }

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
        if ( __hasItems.Get(__depth) ) { __builder.Append((byte)','); }
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

    private void Pop( bool isObject, byte close )
    {
        if ( __depth == 0 || __isObject.Get(__depth) != isObject ) { throw Misuse($"'{(char)close}' doesn't close the current container."); }

        if ( __afterName ) { throw Misuse("A property name needs a value before the object ends."); }

        if ( __indented && __hasItems.Get(__depth) ) { NewLine(__depth - 1); }

        __builder.Append(close);
        __depth--;
    }

    private void NewLine( int depth )
    {
        __builder.Append((byte)'\n');
        __builder.Append(__indentChar, depth * __indentSize);
    }

    private readonly JsonWriteException Misuse( string message ) => new(new JsonError(JsonErrorKind.UnexpectedToken, __builder.Length, 0, 0), message);
}
