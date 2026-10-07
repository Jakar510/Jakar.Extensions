// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary> The kind of a JSON value or token. </summary>
public enum JsonTokenKind : byte
{
    None,
    Object,
    Array,
    String,
    Number,
    True,
    False,
    Null,
    PropertyName
}



/// <summary>
///     A member name, already escaped, quoted and followed by the name separator <c>" : "</c>, in both encodings (SPEC.md §3.3).
///     Generated code keeps one per member as compile-time constants, so writing a name is a single copy.
/// </summary>
public readonly ref struct JsonName
{
    public readonly ReadOnlySpan<char> Utf16;
    public readonly ReadOnlySpan<byte> Utf8;

    /// <param name="utf16"> e.g. <c>"\"total\" : "</c> </param>
    /// <param name="utf8"> e.g. <c>"\"total\" : "u8</c> </param>
    public JsonName( ReadOnlySpan<char> utf16, ReadOnlySpan<byte> utf8 )
    {
        Utf16 = utf16;
        Utf8  = utf8;
    }
}



/// <summary> Text read from JSON (a string's content or a property name), unescaped, in the reader's encoding: <see cref="Utf8"/> when <see cref="IJsonReader.IsUtf8"/>, else <see cref="Utf16"/>. Valid until the next read. </summary>
public readonly ref struct JsonSpan
{
    public readonly ReadOnlySpan<char> Utf16;
    public readonly ReadOnlySpan<byte> Utf8;
    public readonly bool               IsUtf8;

    public JsonSpan( ReadOnlySpan<char> utf16 )
    {
        Utf16  = utf16;
        Utf8   = default;
        IsUtf8 = false;
    }

    public JsonSpan( ReadOnlySpan<byte> utf8 )
    {
        Utf16  = default;
        Utf8   = utf8;
        IsUtf8 = true;
    }


    /// <summary> The length in the span's own units (chars or bytes). </summary>
    public int Length => IsUtf8
                             ? Utf8.Length
                             : Utf16.Length;

    public bool IsEmpty => Length == 0;


    /// <summary> Ordinal comparison with <paramref name="text"/>. </summary>
    public bool Equals( scoped ReadOnlySpan<char> text )
    {
        if ( !IsUtf8 ) { return Utf16.SequenceEqual(text); }

        if ( text.Length > Utf8.Length ) { return false; } // UTF-8 never has fewer bytes than UTF-16 has chars

        char[]? rented = null;

        Span<char> buffer = Utf8.Length <= 256
                                ? stackalloc char[256]
                                : rented = ArrayPool<char>.Shared.Rent(Utf8.Length);

        try { return System.Text.Unicode.Utf8.ToUtf16(Utf8, buffer, out _, out int written) == OperationStatus.Done && buffer[..written].SequenceEqual(text); }
        finally
        {
            if ( rented is not null ) { ArrayPool<char>.Shared.Return(rented); }
        }
    }

    /// <summary> Decodes into <paramref name="destination"/> (at least <see cref="Length"/> chars always suffices). </summary>
    public int CopyTo( Span<char> destination )
    {
        if ( !IsUtf8 )
        {
            Utf16.CopyTo(destination);
            return Utf16.Length;
        }

        System.Text.Unicode.Utf8.ToUtf16(Utf8, destination, out _, out int written);
        return written;
    }

    public override string ToString() => IsUtf8
                                             ? Encoding.UTF8.GetString(Utf8)
                                             : new string(Utf16);
}



/// <summary> A reader position to come back to (<see cref="IJsonReader.Rewind"/>), e.g. to look ahead for a polymorphic discriminator. </summary>
public readonly struct JsonReaderCheckpoint( int position, int depth, int state )
{
    internal readonly int Position = position;
    internal readonly int Depth    = depth;
    internal readonly int State    = state;
}



/// <summary>
///     Writes JSON tokens. Implementations are <see langword="ref struct"/>s; generated code is generic over them (<c>where TWriter : IJsonWriter, allows ref struct</c>),
///     so the JIT specializes it per encoding with no interface dispatch.
/// </summary>
/// <remarks> Writers insert separators and indentation themselves. Misuse (a value with no name inside an object, too deep, NaN) throws <see cref="JsonWriteException"/>. </remarks>
public interface IJsonWriter
{
    /// <summary> Constant per implementation; branches on it are removed by the JIT. </summary>
    static abstract bool IsUtf8 { get; }

    int Depth { get; }

    void WriteStartObject();
    void WriteEndObject();
    void WriteStartArray();
    void WriteEndArray();

    /// <summary> A pre-escaped, pre-encoded member name (<see cref="JsonName"/>). </summary>
    void WritePropertyName( JsonName                  name );
    /// <summary> A runtime name (dictionary keys, extension data); escaped by the writer. </summary>
    void WritePropertyName( scoped ReadOnlySpan<char> name );

    void WriteNull();
    void WriteBoolean( bool                     value );
    void WriteString( scoped ReadOnlySpan<char> value );

    /// <summary> Any integer type, invariant. </summary>
    void WriteInteger<T>( T value )
        where T : IBinaryInteger<T>;

    /// <summary> Any floating-point type (incl. <see cref="decimal"/>): shortest round-trippable, canonical exponent (§5.2). </summary>
    /// <exception cref="JsonWriteException"> NaN or ±Infinity. </exception>
    void WriteFloat<T>( T value )
        where T : IFloatingPoint<T>;

    /// <summary> <paramref name="value"/> formatted with invariant culture, written as an escaped JSON string. </summary>
    void WriteFormatted<T>( T value, scoped ReadOnlySpan<char> format = default )
        where T : ISpanFormattable;

    /// <summary> A number already validated against the JSON grammar, written verbatim (the DOM keeps number text this way). </summary>
    void WriteRawNumber( scoped ReadOnlySpan<char> number );
}



/// <summary>
///     Reads JSON tokens, strictly and without allocating except for strings asked for (SPEC.md §3.5). Every <c>Try*</c> member returns <see langword="false"/> on failure and records
///     the first error in <see cref="Error"/> instead of throwing; the public helpers turn that into a <see cref="JsonReadException"/>.
/// </summary>
public interface IJsonReader
{
    /// <summary> <see langword="true"/> when every <see cref="JsonSpan"/> this reader hands out is UTF-8. A hint for specialization: code that matches names must still check <see cref="JsonSpan.IsUtf8"/> (a <see cref="JsonTapeReader"/> may hold either encoding). </summary>
    static abstract bool IsUtf8 { get; }

    int       Depth { get; }
    JsonError Error { get; }

    /// <summary> The next value's kind, without consuming it (<see cref="JsonTokenKind.None"/> at the end of input or after an error). </summary>
    JsonTokenKind PeekKind();

    bool TryReadStartObject();
    /// <summary> Reads the next member's name, or the end of the object (<paramref name="end"/>; the <c>}</c> is consumed). Call after <see cref="TryReadStartObject"/> and after each member value. </summary>
    bool TryReadProperty( out JsonSpan name, out bool end );

    bool TryReadStartArray();
    /// <summary> Moves to the next element, or reads the end of the array (<paramref name="end"/>; the <c>]</c> is consumed). Call before each element. </summary>
    bool TryReadNextElement( out bool end );

    /// <summary> Consumes a <c>null</c> if that's the next value; otherwise consumes nothing and returns <see langword="false"/> (not an error). </summary>
    bool TryReadNull();
    bool TryReadBoolean( out bool value );
    /// <param name="value"> The number; <see langword="default"/> on failure. </param>
    /// <param name="allowString"> Also accept the number as a JSON string (<c>NumbersFromStrings = Allow</c>). </param>
    bool TryReadInteger<T>( out T value, bool allowString = false )
        where T : struct, IBinaryInteger<T>;
    bool TryReadFloat<T>( out T value, JsonFloatRead mode = JsonFloatRead.Strict )
        where T : struct, IFloatingPoint<T>;
    /// <summary> The only allocating read: the string itself, at its exact length. </summary>
    bool TryReadString( [NotNullWhen(true)] out string?  value );
    /// <summary> The string's content without allocating; valid until the next read. </summary>
    bool TryReadStringSpan( out                 JsonSpan value );
    /// <summary> A number token's text, verbatim (validated). </summary>
    bool TryReadRawNumber( out                  JsonSpan number );
    /// <summary> Skips any value, depth-checked. </summary>
    bool TrySkipValue();

    JsonReaderCheckpoint Checkpoint();
    void                 Rewind( in JsonReaderCheckpoint checkpoint );

    /// <summary> Records <paramref name="kind"/> at the current position (the first error wins) and returns <see langword="false"/>. </summary>
    bool Fail( JsonErrorKind kind );

    /// <summary> The JSON path of the current position, e.g. <c>$.lines[3].price</c>. Allocates: for error messages. </summary>
    string GetPath();
}



[Flags]
public enum JsonFloatRead : byte
{
    Strict         = 0,
    /// <summary> Accept the number as a JSON string (<c>NumbersFromStrings = Allow</c>). </summary>
    AllowString    = 1,
    /// <summary> Accept <c>"NaN"</c>, <c>"Infinity"</c> and <c>"-Infinity"</c> (<c>NonFiniteFloats = AsString</c>). </summary>
    AllowNonFinite = 2
}



/// <summary>
///     A type that reads and writes itself as JSON through source-generated code. Don't implement it by hand: put <see cref="GenerateJsonAttribute"/> on a
///     <see langword="partial"/> type. Generic code reaches everything through the type parameter (<c>T.TryReadJson(ref reader, out T value)</c>), with no reflection.
/// </summary>
public interface IJsonSerializable<TSelf> : ISpanFormattable, IUtf8SpanFormattable, ISpanParsable<TSelf>, IUtf8SpanParsable<TSelf>
    where TSelf : IJsonSerializable<TSelf>
{
    static abstract void WriteJson<TWriter>( ref TWriter writer, scoped in TSelf value )
        where TWriter : IJsonWriter, allows ref struct;

    static abstract bool TryReadJson<TReader>( ref TReader reader, [MaybeNullWhen(false)] out TSelf value )
        where TReader : IJsonReader, allows ref struct;

    /// <summary> The type's resolved writer settings (indentation, escaping, depth). </summary>
    static abstract JsonWriterOptions DefaultWriterOptions { get; }

    /// <summary> The type's resolved reader settings (comments, trailing commas, depth). </summary>
    static abstract JsonReaderOptions DefaultReaderOptions { get; }
}



/// <summary>
///     Reads and writes a <typeparamref name="T"/>, statically: generated code calls <c>TConverter.Write(ref writer, value)</c>, so there's no instance and no reflection.
///     Implement it on a <see langword="struct"/> so the JIT specializes generic code for it. Use with <c>[JsonMember(Converter = typeof(MyConverter))]</c>.
/// </summary>
public interface IJsonConverter<T>
{
    static abstract void Write<TWriter>( ref TWriter writer, scoped in T value )
        where TWriter : IJsonWriter, allows ref struct;

    static abstract bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out T value )
        where TReader : IJsonReader, allows ref struct;
}



/// <summary> Reads and writes <typeparamref name="T"/> as an object member name (dictionary keys). </summary>
public interface IJsonKeyConverter<T>
{
    static abstract void WriteKey<TWriter>( ref TWriter writer, T key )
        where TWriter : IJsonWriter, allows ref struct;

    static abstract bool TryParseKey( scoped ReadOnlySpan<char> name, [MaybeNullWhen(false)] out T key );
}
