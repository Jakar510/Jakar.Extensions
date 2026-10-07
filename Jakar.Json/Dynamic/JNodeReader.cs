// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary> An <see cref="IJsonReader"/> over a <see cref="JNode"/> tree, so generated model code reads straight from the DOM (<see cref="JNode.ToModel{T}"/>). </summary>
public ref struct JNodeReader : IJsonReader, IDisposable
{
    private readonly JNode?             __root;
    private          JNode?[]?          __containers; // per depth: the container being read
    private          int[]?             __indices;    // per depth: the next child index
    private          int                __depth;
    private          JNode?             __pending; // the next value
    private          bool               __hasPending;
    private          JsonError          __error;
    private          ValueStringBuilder __chars;


    public static bool IsUtf8 => false;

    public readonly int       Depth => __depth;
    public readonly JsonError Error => __error;


    /// <param name="root"> The value to read; <see langword="null"/> reads as JSON <c>null</c>. </param>
    public JNodeReader( JNode? root )
    {
        __root       = root;
        __pending    = root;
        __hasPending = true;
        __containers = null;
        __indices    = null;
        __chars      = new ValueStringBuilder(Span<char>.Empty);
    }


    public JsonTokenKind PeekKind()
    {
        if ( __error.IsError || !__hasPending ) { return JsonTokenKind.None; }

        return __pending switch
               {
                   null                                     => JsonTokenKind.Null,
                   JObjectNode                              => JsonTokenKind.Object,
                   JArrayNode                               => JsonTokenKind.Array,
                   JValueNode { Kind: JNodeKind.String }    => JsonTokenKind.String,
                   JValueNode { Kind: JNodeKind.Number }    => JsonTokenKind.Number,
                   JValueNode value when value.GetBoolean() => JsonTokenKind.True,
                   _                                        => JsonTokenKind.False
               };
    }

    public bool TryReadStartObject()
    {
        if ( PeekKind() != JsonTokenKind.Object ) { return Fail(JsonErrorKind.UnexpectedToken); }

        Push(__pending!);
        return true;
    }

    public bool TryReadStartArray()
    {
        if ( PeekKind() != JsonTokenKind.Array ) { return Fail(JsonErrorKind.UnexpectedToken); }

        Push(__pending!);
        return true;
    }

    public bool TryReadProperty( out JsonSpan name, out bool end )
    {
        name = default;
        end  = false;
        if ( __error.IsError ) { return false; }

        if ( __depth == 0 || __containers![__depth - 1] is not JObjectNode obj ) { return Fail(JsonErrorKind.UnexpectedToken); }

        ref int index = ref __indices![__depth - 1];

        if ( index >= obj.Count )
        {
            Pop();
            end = true;
            return true;
        }

        KeyValuePair<string, JNode?> member = obj.GetAt(index++);
        name         = new JsonSpan(member.Key);
        __pending    = member.Value;
        __hasPending = true;
        return true;
    }

    public bool TryReadNextElement( out bool end )
    {
        end = false;
        if ( __error.IsError ) { return false; }

        if ( __depth == 0 || __containers![__depth - 1] is not JArrayNode array ) { return Fail(JsonErrorKind.UnexpectedToken); }

        ref int index = ref __indices![__depth - 1];

        if ( index >= array.Count )
        {
            Pop();
            end = true;
            return true;
        }

        __pending    = array[index++];
        __hasPending = true;
        return true;
    }

    public bool TryReadNull()
    {
        if ( PeekKind() != JsonTokenKind.Null ) { return false; }

        Consume();
        return true;
    }

    public bool TryReadBoolean( out bool value )
    {
        value = false;
        JsonTokenKind kind = PeekKind();
        if ( kind is not (JsonTokenKind.True or JsonTokenKind.False) ) { return Fail(JsonErrorKind.UnexpectedToken); }

        value = kind == JsonTokenKind.True;
        Consume();
        return true;
    }

    public bool TryReadInteger<T>( out T value, bool allowString = false )
        where T : struct, IBinaryInteger<T>
    {
        value = default;

        switch ( __pending )
        {
            case JValueNode { Kind: JNodeKind.Number } number when __hasPending:
                if ( !number.TryGetInteger(out value, out JsonErrorKind error) ) { return Fail(error); }

                Consume();
                return true;

            case JValueNode { Kind: JNodeKind.String } text when __hasPending && allowString:
                if ( !T.TryParse(text.GetString(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value) ) { return Fail(JsonErrorKind.InvalidNumber); }

                Consume();
                return true;

            default:
                return Fail(JsonErrorKind.UnexpectedToken);
        }
    }

    public bool TryReadFloat<T>( out T value, JsonFloatRead mode = JsonFloatRead.Strict )
        where T : struct, IFloatingPoint<T>
    {
        value = default;

        switch ( __pending )
        {
            case JValueNode { Kind: JNodeKind.Number } number when __hasPending:
                if ( !number.TryGetFloat(out value, out JsonErrorKind error) ) { return Fail(error); }

                Consume();
                return true;

            case JValueNode { Kind: JNodeKind.String } text when __hasPending && mode != JsonFloatRead.Strict:
                JsonSpan span = new(text.GetString());

                if ( ( mode & JsonFloatRead.AllowNonFinite ) != 0 && JsonFloats.TryParseNonFinite(span, out value, out bool supported) )
                {
                    if ( !supported ) { return Fail(JsonErrorKind.NonFiniteNumber); }

                    Consume();
                    return true;
                }

                if ( ( mode & JsonFloatRead.AllowString ) == 0 || !T.TryParse(span.Utf16, NumberStyles.Float, CultureInfo.InvariantCulture, out value) || T.IsInfinity(value) ) { return Fail(JsonErrorKind.InvalidNumber); }

                Consume();
                return true;

            default:
                return Fail(JsonErrorKind.UnexpectedToken);
        }
    }

    public bool TryReadString( [NotNullWhen(true)] out string? value )
    {
        value = null;
        if ( !__hasPending || __pending is not JValueNode { Kind: JNodeKind.String } text ) { return Fail(JsonErrorKind.UnexpectedToken); }

        value = text.GetString();
        Consume();
        return true;
    }

    public bool TryReadStringSpan( out JsonSpan value )
    {
        value = default;
        if ( !TryReadString(out string? text) ) { return false; }

        value = new JsonSpan(text);
        return true;
    }

    public bool TryReadRawNumber( out JsonSpan number )
    {
        number = default;
        if ( !__hasPending || __pending is not JValueNode { Kind: JNodeKind.Number } value ) { return Fail(JsonErrorKind.UnexpectedToken); }

        __chars.Reset();
        value.AppendNumberText(ref __chars);
        number = new JsonSpan(__chars.Values);
        Consume();
        return true;
    }

    public bool TrySkipValue()
    {
        if ( !__hasPending || __error.IsError ) { return Fail(JsonErrorKind.UnexpectedEnd); }

        Consume();
        return true;
    }

    public readonly JsonReaderCheckpoint Checkpoint() => new(__hasPending
                                                                 ? 1
                                                                 : 0,
                                                             __depth,
                                                             __depth > 0
                                                                 ? __indices![__depth - 1]
                                                                 : 0);

    public void Rewind( in JsonReaderCheckpoint checkpoint )
    {
        while ( __depth > checkpoint.Depth ) { Pop(); }

        if ( __depth > 0 ) { __indices![__depth - 1] = checkpoint.State; }

        __hasPending = checkpoint.Position == 1;
        if ( !__hasPending ) { return; }

        // The pending value was the child before the saved index (or the root).
        __pending = __depth == 0
                        ? __root
                        : __containers![__depth - 1] switch
                          {
                              JObjectNode obj  => obj.GetAt(checkpoint.State - 1).Value,
                              JArrayNode array => array[checkpoint.State     - 1],
                              _                => null
                          };
    }

    public bool Fail( JsonErrorKind kind )
    {
        if ( !__error.IsError ) { __error = new JsonError(kind, 0, 0, 0); }

        return false;
    }

    public readonly string GetPath()
    {
        ValueStringBuilder path = new(stackalloc char[128]);
        path.Append('$');

        for ( int depth = 0; depth < __depth; depth++ )
        {
            int read = __indices![depth];
            if ( read == 0 ) { continue; }

            switch ( __containers![depth] )
            {
                case JObjectNode obj:
                    JsonPath.AppendMember(ref path, obj.GetAt(read - 1).Key);
                    break;

                case JArrayNode:
                    path.Append('[').AppendSpanFormattable(read - 1, default, CultureInfo.InvariantCulture).Append(']');
                    break;
            }
        }

        return path.ToString();
    }

    public void Dispose()
    {
        if ( __containers is not null ) { ArrayPool<JNode?>.Shared.Return(__containers, true); }

        if ( __indices is not null ) { ArrayPool<int>.Shared.Return(__indices); }

        __containers = null;
        __indices    = null;
        __chars.Dispose();
    }


    private void Consume()
    {
        __pending    = null;
        __hasPending = false;
    }

    private void Push( JNode container )
    {
        if ( __containers is null || __depth == __containers.Length )
        {
            JNode?[] containers = ArrayPool<JNode?>.Shared.Rent(Math.Max(16, __depth * 2));
            int[]    indices    = ArrayPool<int>.Shared.Rent(containers.Length);

            if ( __containers is not null )
            {
                __containers.AsSpan(0, __depth).CopyTo(containers);
                __indices.AsSpan(0, __depth).CopyTo(indices);
                ArrayPool<JNode?>.Shared.Return(__containers, true);
                ArrayPool<int>.Shared.Return(__indices!);
            }

            __containers = containers;
            __indices    = indices;
        }

        __containers[__depth] = container;
        __indices![__depth]   = 0;
        __depth++;
        Consume();
    }

    private void Pop()
    {
        __depth--;
        __containers![__depth] = null;
    }
}
