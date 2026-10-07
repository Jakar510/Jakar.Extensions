// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary> The kind of a <see cref="JNode"/>. JSON <c>null</c> has no node: it's a <see langword="null"/> reference. </summary>
public enum JNodeKind : byte
{
    Object,
    Array,
    String,
    Number,
    Boolean
}



/// <summary> A mutable JSON tree in the style of Newtonsoft's <c>JToken</c>: <see cref="JObjectNode"/>, <see cref="JArrayNode"/> or <see cref="JValueNode"/>. </summary>
/// <remarks>
///     <para> The hierarchy is closed (the constructor is <see langword="private protected"/>), so a switch over <see cref="Kind"/> is exhaustive. </para>
///     <para> A node has at most one <see cref="Parent"/>, so a tree can't contain a cycle. Not thread-safe for writes; concurrent reads of an unchanging tree are safe. </para>
/// </remarks>
public abstract partial class JNode
{
    private JNode? __parent;


    private protected JNode() { }


    public abstract JNodeKind Kind   { get; }
    public          JNode?    Parent => __parent;

    public JNode Root
    {
        get
        {
            JNode node = this;
            while ( node.__parent is not null ) { node = node.__parent; }

            return node;
        }
    }

    /// <summary> The node's location from the root, e.g. <c>$.lines[3].price</c> or <c>$['odd key']</c>. Diagnostic: O(depth × siblings). </summary>
    public string Path
    {
        get
        {
            ValueStringBuilder builder = new(stackalloc char[128]);
            AppendPath(ref builder);
            return builder.ToString();
        }
    }


    /// <summary> An object member (<see langword="null"/> when missing or JSON <c>null</c>). </summary>
    /// <exception cref="InvalidOperationException"> The node isn't an object. </exception>
    public virtual JNode? this[ string name ] { get => throw NotA("an object"); set => throw NotA("an object"); }

    /// <summary> An array element. </summary>
    /// <exception cref="InvalidOperationException"> The node isn't an array. </exception>
    public virtual JNode? this[ int index ] { get => throw NotA("an array"); set => throw NotA("an array"); }


    public JObjectNode AsObject() => this as JObjectNode ?? throw NotA("an object");
    public JArrayNode  AsArray()  => this as JArrayNode  ?? throw NotA("an array");
    public JValueNode  AsValue()  => this as JValueNode  ?? throw NotA("a value");


    // ─── Parse ───────────────────────────────────────────────────────────────

    /// <summary> <see langword="null"/> for a JSON <c>null</c> root. </summary>
    /// <exception cref="JsonReadException"> The JSON is malformed, or an object repeats a member name. </exception>
    public static JNode? Parse( string json, JsonReaderOptions? options = null )
    {
        using JsonTape tape = JsonTape.Parse(json, options);
        return FromTape(tape);
    }

    /// <inheritdoc cref="Parse(string, JsonReaderOptions?)"/>
    public static JNode? Parse( ReadOnlySpan<char> json, JsonReaderOptions? options = null )
    {
        using JsonTape tape = JsonTape.Parse(json, options);
        return FromTape(tape);
    }

    /// <inheritdoc cref="Parse(string, JsonReaderOptions?)"/>
    public static JNode? Parse( ReadOnlySpan<byte> utf8Json, JsonReaderOptions? options = null )
    {
        using JsonTape tape = JsonTape.Parse(utf8Json, options);
        return FromTape(tape);
    }

    /// <inheritdoc cref="Parse(string, JsonReaderOptions?)"/>
    public static JNode? Parse( Stream utf8Json, JsonReaderOptions? options = null )
    {
        using JsonTape tape = JsonTape.Parse(utf8Json, options);
        return FromTape(tape);
    }

    /// <inheritdoc cref="Parse(string, JsonReaderOptions?)"/>
    public static async ValueTask<JNode?> ParseAsync( Stream utf8Json, JsonReaderOptions? options = null, CancellationToken token = default )
    {
        using JsonTape tape = await JsonTape.ParseAsync(utf8Json, options, token).ConfigureAwait(false);
        return FromTape(tape);
    }

    /// <summary> Exception-free. <paramref name="node"/> is <see langword="null"/> for a JSON <c>null</c> root, as well as on failure: check the return value. </summary>
    public static bool TryParse( [NotNullWhen(true)] string? json, out JNode? node, out JsonError error, JsonReaderOptions? options = null )
    {
        node = null;
        if ( !JsonTape.TryParse(json, out JsonTape? tape, out error, options) ) { return false; }

        using ( tape ) { return TryFromItem(tape.Root, out node, out error); }
    }


    private static JNode? FromTape( JsonTape tape ) => TryFromItem(tape.Root, out JNode? node, out JsonError error)
                                                           ? node
                                                           : throw new JsonReadException(error);

    internal static bool TryFromItem( JsonItem item, out JNode? node, out JsonError error )
    {
        RuntimeHelpers.EnsureSufficientExecutionStack();
        error = default;

        switch ( item.Kind )
        {
            case JsonTokenKind.Object:
            {
                JObjectNode obj = new(item.GetPropertyCount());

                foreach ( JsonTapeProperty member in item.EnumerateObject() )
                {
                    if ( !TryFromItem(member.Value, out JNode? value, out error) )
                    {
                        node = null;
                        return false;
                    }

                    if ( !obj.TryAddNew(member.Name.GetString(), value) )
                    {
                        error = item.Tape.CreateError(JsonErrorKind.DuplicateMember, member.Name.Position); // DuplicateMembers = Error (§4.2)
                        node  = null;
                        return false;
                    }
                }

                node = obj;
                return true;
            }

            case JsonTokenKind.Array:
            {
                JArrayNode array = new(item.GetArrayLength());

                foreach ( JsonItem element in item.EnumerateArray() )
                {
                    if ( !TryFromItem(element, out JNode? value, out error) )
                    {
                        node = null;
                        return false;
                    }

                    array.AddNew(value);
                }

                node = array;
                return true;
            }

            case JsonTokenKind.String:
                node = new JValueNode(item.GetString());
                return true;

            case JsonTokenKind.Number:
                node = JValueNode.FromValidatedNumberText(item.GetRawText()); // keeps the text: 1.10 stays 1.10
                return true;

            case JsonTokenKind.True:
                node = new JValueNode(true);
                return true;

            case JsonTokenKind.False:
                node = new JValueNode(false);
                return true;

            default: // JsonTokenKind.Null
                node = null;
                return true;
        }
    }


    // ─── Write ───────────────────────────────────────────────────────────────

    /// <summary> Writes this node through any <see cref="IJsonWriter"/> (text, UTF-8, or another DOM). </summary>
    public abstract void WriteTo<TWriter>( ref TWriter writer )
        where TWriter : IJsonWriter, allows ref struct;

    public string ToJson( JsonWriterOptions? options = null )
    {
        JsonWriter writer = new(stackalloc char[512], options ?? JsonWriterOptions.Default); // §3.2: UTF-16 over ValueStringBuilder

        try
        {
            WriteTo(ref writer);
            return writer.ToString();
        }
        finally { writer.Dispose(); }
    }

    public byte[] ToJsonUtf8( JsonWriterOptions? options = null )
    {
        JsonUtf8Writer writer = new(stackalloc byte[512], options ?? JsonWriterOptions.Default); // §3.2: UTF-8 over ValueUtf8Builder

        try
        {
            WriteTo(ref writer);
            return writer.ToArray();
        }
        finally { writer.Dispose(); }
    }

    /// <summary> The node's JSON (compact). </summary>
    public override string ToString() => ToJson();

    /// <summary> Writes <paramref name="node"/>, or <c>null</c>. Generated code uses it for extension data. </summary>
    public static void Write<TWriter>( JNode? node, ref TWriter writer )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( node is null ) { writer.WriteNull(); }
        else { node.WriteTo(ref writer); }
    }


    // ─── Models ──────────────────────────────────────────────────────────────

    /// <summary> Builds a tree from a model through its generated <c>WriteJson</c>, with no intermediate text. </summary>
    public static JNode? FromModel<T>( T value )
        where T : IJsonSerializable<T>
    {
        JNodeWriter writer = new(); // §3.2 (P4)
        T.WriteJson(ref writer, in value);
        return writer.Result;
    }

    /// <summary> Reads a model from this tree through its generated <c>TryReadJson</c>, with no intermediate text. </summary>
    /// <exception cref="JsonReadException"> The tree doesn't match <typeparamref name="T"/>. </exception>
    public T ToModel<T>()
        where T : IJsonSerializable<T>
    {
        JNodeReader reader = new(this); // §3.2 (P4)

        return T.TryReadJson(ref reader, out T? value)
                   ? value
                   : throw new JsonReadException(reader.Error);
    }


    // ─── Clone / compare ─────────────────────────────────────────────────────

    /// <summary> A detached copy (no <see cref="Parent"/>) that can be added anywhere. </summary>
    public abstract JNode DeepClone();

    /// <summary> Structural equality: object members in any order, array elements in order, numbers by value (<c>1</c> == <c>1.0</c> == <c>1m</c>), strings ordinal. </summary>
    public static bool DeepEquals( JNode? left, JNode? right )
    {
        if ( ReferenceEquals(left, right) ) { return true; }

        if ( left is null || right is null || left.Kind != right.Kind ) { return false; }

        RuntimeHelpers.EnsureSufficientExecutionStack();
        return left.DeepEqualsCore(right);
    }

    /// <param name="other"> Same <see cref="Kind"/> as this node. </param>
    private protected abstract bool DeepEqualsCore( JNode other );


    // ─── Parent tracking ─────────────────────────────────────────────────────

    /// <summary>
    ///     Removes this node from its <see cref="Parent"/> (its array element or object member is removed) and returns it, ready to be added anywhere else.
    ///     Use it to keep a piece of a large document without keeping the rest alive: a parent link keeps the whole tree reachable. No-op for a root.
    /// </summary>
    /// <returns> This node, for chaining (<c>target["lines"] = source["lines"]!.Detach();</c>). </returns>
    public JNode Detach()
    {
        __parent?.RemoveChild(this); // clears __parent through Orphan
        return this;
    }

    /// <summary> Removes <paramref name="child"/> (by reference) from this container and orphans it. </summary>
    private protected abstract void RemoveChild( JNode child );


    /// <summary> Makes this node <paramref name="child"/>'s parent; throws if that would give it two parents or create a cycle. </summary>
    private protected void Adopt( JNode? child )
    {
        if ( child is null ) { return; }

        if ( child.__parent is not null ) { throw new InvalidOperationException($"The node already belongs to '{child.__parent.Path}'; add a DeepClone() of it instead."); }

        for ( JNode? ancestor = this; ancestor is not null; ancestor = ancestor.__parent )
        {
            if ( ReferenceEquals(ancestor, child) ) { throw new InvalidOperationException("A node can't be added to itself or to one of its descendants."); }
        }

        child.__parent = this;
    }

    /// <summary> For nodes created by this library a moment ago (parsing, cloning): no checks needed. </summary>
    private protected void AdoptNew( JNode? child )
    {
        if ( child is not null ) { child.__parent = this; }
    }

    private protected static void Orphan( JNode? child )
    {
        if ( child is not null ) { child.__parent = null; }
    }


    private void AppendPath( ref ValueStringBuilder builder )
    {
        if ( __parent is null )
        {
            builder.Append('$');
            return;
        }

        RuntimeHelpers.EnsureSufficientExecutionStack();
        __parent.AppendPath(ref builder);
        __parent.AppendSegment(this, ref builder);
    }

    /// <summary> Appends <paramref name="child"/>'s segment (<c>.name</c>, <c>['name']</c> or <c>[i]</c>). </summary>
    private protected abstract void AppendSegment( JNode child, ref ValueStringBuilder builder );

    private InvalidOperationException NotA( string expected ) => new($"The node is a JSON {Kind}, not {expected}.");


    // ─── Conversions: CLR → JNode ────────────────────────────────────────────

    public static implicit operator JNode( bool           value ) => new JValueNode(value);
    public static implicit operator JNode( sbyte          value ) => new JValueNode(value);
    public static implicit operator JNode( byte           value ) => new JValueNode((int)value); // byte and ushort would be ambiguous between int and UInt128
    public static implicit operator JNode( short          value ) => new JValueNode(value);
    public static implicit operator JNode( ushort         value ) => new JValueNode((int)value);
    public static implicit operator JNode( int            value ) => new JValueNode(value);
    public static implicit operator JNode( uint           value ) => new JValueNode(value);
    public static implicit operator JNode( long           value ) => new JValueNode(value);
    public static implicit operator JNode( ulong          value ) => new JValueNode(value);
    public static implicit operator JNode( Int128         value ) => new JValueNode(value);
    public static implicit operator JNode( UInt128        value ) => new JValueNode(value);
    public static implicit operator JNode( float          value ) => new JValueNode(value);
    public static implicit operator JNode( double         value ) => new JValueNode(value);
    public static implicit operator JNode( decimal        value ) => new JValueNode(value);
    public static implicit operator JNode( Guid           value ) => new JValueNode(value);
    public static implicit operator JNode( DateTime       value ) => new JValueNode(value);
    public static implicit operator JNode( DateTimeOffset value ) => new JValueNode(value);
    public static implicit operator JNode( DateOnly       value ) => new JValueNode(value);
    public static implicit operator JNode( TimeOnly       value ) => new JValueNode(value);
    public static implicit operator JNode( TimeSpan       value ) => new JValueNode(value);

    public static implicit operator JNode?( string? value ) => value is null
                                                                   ? null
                                                                   : new JValueNode(value);
    public static implicit operator JNode?( bool? value ) => value.HasValue
                                                                 ? new JValueNode(value.Value)
                                                                 : null;
    public static implicit operator JNode?( int? value ) => value.HasValue
                                                                ? new JValueNode(value.Value)
                                                                : null;
    public static implicit operator JNode?( long? value ) => value.HasValue
                                                                 ? new JValueNode(value.Value)
                                                                 : null;
    public static implicit operator JNode?( double? value ) => value.HasValue
                                                                   ? new JValueNode(value.Value)
                                                                   : null;
    public static implicit operator JNode?( decimal? value ) => value.HasValue
                                                                    ? new JValueNode(value.Value)
                                                                    : null;
    public static implicit operator JNode?( Guid? value ) => value.HasValue
                                                                 ? new JValueNode(value.Value)
                                                                 : null;
    public static implicit operator JNode?( DateTime? value ) => value.HasValue
                                                                     ? new JValueNode(value.Value)
                                                                     : null;
    public static implicit operator JNode?( DateTimeOffset? value ) => value.HasValue
                                                                           ? new JValueNode(value.Value)
                                                                           : null;


    // ─── Conversions: JNode → CLR ────────────────────────────────────────────

    public static explicit operator bool( JNode?           node ) => RequireValue(node, "Boolean").GetBoolean();
    public static explicit operator int( JNode?            node ) => RequireValue(node, "Int32").GetInt32();
    public static explicit operator long( JNode?           node ) => RequireValue(node, "Int64").GetInt64();
    public static explicit operator ulong( JNode?          node ) => RequireValue(node, "UInt64").GetUInt64();
    public static explicit operator float( JNode?          node ) => RequireValue(node, "Single").GetSingle();
    public static explicit operator double( JNode?         node ) => RequireValue(node, "Double").GetDouble();
    public static explicit operator decimal( JNode?        node ) => RequireValue(node, "Decimal").GetDecimal();
    public static explicit operator Guid( JNode?           node ) => RequireValue(node, "Guid").GetGuid();
    public static explicit operator DateTime( JNode?       node ) => RequireValue(node, "DateTime").GetDateTime();
    public static explicit operator DateTimeOffset( JNode? node ) => RequireValue(node, "DateTimeOffset").GetDateTimeOffset();

    public static explicit operator string?( JNode? node ) => node is null
                                                                  ? null
                                                                  : RequireValue(node, "String").GetString();
    public static explicit operator bool?( JNode? node ) => node is null
                                                                ? null
                                                                : (bool)node;
    public static explicit operator int?( JNode? node ) => node is null
                                                               ? null
                                                               : (int)node;
    public static explicit operator long?( JNode? node ) => node is null
                                                                ? null
                                                                : (long)node;
    public static explicit operator double?( JNode? node ) => node is null
                                                                  ? null
                                                                  : (double)node;
    public static explicit operator decimal?( JNode? node ) => node is null
                                                                   ? null
                                                                   : (decimal)node;
    public static explicit operator Guid?( JNode? node ) => node is null
                                                                ? null
                                                                : (Guid)node;
    public static explicit operator DateTime?( JNode? node ) => node is null
                                                                    ? null
                                                                    : (DateTime)node;
    public static explicit operator DateTimeOffset?( JNode? node ) => node is null
                                                                          ? null
                                                                          : (DateTimeOffset)node;

    private static JValueNode RequireValue( JNode? node, string target ) => node as JValueNode ??
                                                                            throw new InvalidCastException(node is null
                                                                                                               ? $"Can't convert JSON null to {target}."
                                                                                                               : $"Can't convert a JSON {node.Kind} to {target}.");
}
