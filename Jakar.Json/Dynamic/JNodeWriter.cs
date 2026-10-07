// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary> An <see cref="IJsonWriter"/> that builds a <see cref="JNode"/> tree instead of text (<see cref="JNode.FromModel{T}"/>). </summary>
public ref struct JNodeWriter : IJsonWriter
{
    private JNode?   __root;
    private bool     __rootWritten;
    private JNode[]? __stack;
    private int      __depth;
    private string?  __name;


    public static bool IsUtf8 => false;

    public readonly int Depth => __depth;

    /// <summary> The tree written so far; <see langword="null"/> for a JSON <c>null</c> root. </summary>
    public readonly JNode? Result => __root;


    public JNodeWriter() { }


    public void WriteStartObject() => Open(new JObjectNode());

    public void WriteStartArray() => Open(new JArrayNode());

    public void WriteEndObject() => Close(JNodeKind.Object);

    public void WriteEndArray() => Close(JNodeKind.Array);

    /// <summary> Decodes the pre-escaped name (<c>"name" : </c>) back to its text. </summary>
    public void WritePropertyName( JsonName name )
    {
        ReadOnlySpan<char> encoded = name.Utf16;
        int                end     = encoded.LastIndexOf('"');
        ReadOnlySpan<char> escaped = encoded[1..end];

        if ( escaped.IndexOf('\\') < 0 )
        {
            WritePropertyName(escaped);
            return;
        }

        ValueStringBuilder text = new(stackalloc char[128]);

        try
        {
            JsonLexer<char>.Unescape(escaped, ref text);
            WritePropertyName(text.Values);
        }
        finally { text.Dispose(); }
    }

    public void WritePropertyName( scoped ReadOnlySpan<char> name )
    {
        if ( __depth == 0 || __stack![__depth - 1] is not JObjectNode || __name is not null ) { throw Misuse("Property names belong inside an object, one per value."); }

        __name = new string(name);
    }

    public void WriteNull() => Add(null);

    public void WriteBoolean( bool value ) => Add(new JValueNode(value));

    public void WriteString( scoped ReadOnlySpan<char> value ) => Add(new JValueNode(new string(value)));

    public void WriteInteger<T>( T value )
        where T : IBinaryInteger<T>
    {
        JValueNode node;

        if ( T.CreateSaturating(long.CreateSaturating(value))         == value ) { node = new JValueNode(long.CreateTruncating(value)); }
        else if ( T.CreateSaturating(ulong.CreateSaturating(value))   == value ) { node = new JValueNode(ulong.CreateTruncating(value)); }
        else if ( T.CreateSaturating(Int128.CreateSaturating(value))  == value ) { node = new JValueNode(Int128.CreateTruncating(value)); }
        else if ( T.CreateSaturating(UInt128.CreateSaturating(value)) == value ) { node = new JValueNode(UInt128.CreateTruncating(value)); }
        else { node                                                                     = JValueNode.FromValidatedNumberText(value.ToString(null, CultureInfo.InvariantCulture)); } // BigInteger beyond 128 bits

        Add(node);
    }

    public void WriteFloat<T>( T value )
        where T : IFloatingPoint<T>
    {
        if ( !T.IsFinite(value) ) { throw new JsonWriteException(new JsonError(JsonErrorKind.NonFiniteNumber, 0, 0, 0), $"JSON has no {JsonNumbers.NonFiniteName(value)}; use NonFiniteFloats = AsString to write it as a string."); }

        JValueNode node;

        if ( typeof(T)      == typeof(double) ) { node  = new JValueNode(Unsafe.BitCast<T, double>(value)); }
        else if ( typeof(T) == typeof(float) ) { node   = new JValueNode(Unsafe.BitCast<T, float>(value)); }
        else if ( typeof(T) == typeof(decimal) ) { node = new JValueNode(Unsafe.BitCast<T, decimal>(value)); }
        else
        {
            Span<char> buffer = stackalloc char[64];
            JsonNumbers.TryFormatFloat(value, buffer, out int written);
            node = JValueNode.FromValidatedNumberText(new string(buffer[..written]));
        }

        Add(node);
    }

    public void WriteFormatted<T>( T value, scoped ReadOnlySpan<char> format = default )
        where T : ISpanFormattable => Add(new JValueNode(value.ToString(format.IsEmpty
                                                                            ? null
                                                                            : new string(format),
                                                                        CultureInfo.InvariantCulture)));

    public void WriteRawNumber( scoped ReadOnlySpan<char> number ) => Add(JValueNode.FromValidatedNumberText(new string(number)));


    private void Open( JNode container )
    {
        Add(container);

        if ( __stack is null || __depth == __stack.Length )
        {
            JNode[] larger = ArrayPool<JNode>.Shared.Rent(Math.Max(16, __depth * 2));

            if ( __stack is not null )
            {
                __stack.AsSpan(0, __depth).CopyTo(larger);
                ArrayPool<JNode>.Shared.Return(__stack, true);
            }

            __stack = larger;
        }

        __stack[__depth++] = container;
    }

    private void Close( JNodeKind kind )
    {
        if ( __depth == 0 || __stack![__depth - 1].Kind != kind || __name is not null ) { throw Misuse("Closing a container that isn't open."); }

        __stack[--__depth] = null!;

        if ( __depth == 0 )
        {
            ArrayPool<JNode>.Shared.Return(__stack, true);
            __stack = null;
        }
    }

    private void Add( JNode? node )
    {
        if ( __depth == 0 )
        {
            if ( __rootWritten ) { throw Misuse("A JSON document has exactly one root value."); }

            __root        = node;
            __rootWritten = true;
            return;
        }

        switch ( __stack![__depth - 1] )
        {
            case JArrayNode array:
                array.AddNew(node);
                return;

            case JObjectNode obj:
                if ( __name is null ) { throw Misuse("A value inside an object needs a property name first."); }

                obj[__name] = node; // a repeated name replaces the earlier value
                __name      = null;
                return;
        }
    }

    private static JsonWriteException Misuse( string message ) => new(new JsonError(JsonErrorKind.UnexpectedToken, 0, 0, 0), message);
}
