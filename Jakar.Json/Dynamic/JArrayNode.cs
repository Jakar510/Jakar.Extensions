// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary> A mutable JSON array. Elements may be <see langword="null"/> (JSON <c>null</c>). </summary>
public sealed class JArrayNode : JNode, IList<JNode?>, IReadOnlyList<JNode?>
{
    private readonly List<JNode?> __items;


    public JArrayNode() => __items = [];
    public JArrayNode( int capacity ) => __items = new List<JNode?>(capacity);
    /// <exception cref="InvalidOperationException"> An item already has a parent. </exception>
    public JArrayNode( params ReadOnlySpan<JNode?> items ) : this(items.Length)
    {
        foreach ( JNode? item in items ) { Add(item); }
    }


    /// <exception cref="JsonReadException"> The JSON is malformed. </exception>
    /// <exception cref="FormatException"> The root isn't an array. </exception>
    public static new JArrayNode Parse( string json, JsonReaderOptions? options = null ) => JNode.Parse(json, options) as JArrayNode ?? throw new FormatException("The JSON root isn't an array.");

    /// <inheritdoc cref="Parse(string, JsonReaderOptions?)"/>
    public static new JArrayNode Parse( ReadOnlySpan<byte> utf8Json, JsonReaderOptions? options = null ) => JNode.Parse(utf8Json, options) as JArrayNode ?? throw new FormatException("The JSON root isn't an array.");


    public override JNodeKind Kind       => JNodeKind.Array;
    public          int       Count      => __items.Count;
    bool ICollection<JNode?>. IsReadOnly => false;


    /// <exception cref="InvalidOperationException"> <paramref name="value"/> already has a parent, or is this array or one of its ancestors. </exception>
    public override JNode? this[ int index ]
    {
        get => __items[index];
        set
        {
            JNode? old = __items[index];
            if ( ReferenceEquals(old, value) ) { return; }

            Adopt(value); // validates before anything changes
            __items[index] = value;
            Orphan(old);
        }
    }


    /// <inheritdoc cref="this[int]"/>
    public void Add( JNode? item )
    {
        Adopt(item);
        __items.Add(item);
    }

    /// <inheritdoc cref="this[int]"/>
    public void Insert( int index, JNode? item )
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)index, (uint)__items.Count, nameof(index));
        Adopt(item);
        __items.Insert(index, item);
    }

    public void RemoveAt( int index )
    {
        JNode? old = __items[index];
        __items.RemoveAt(index);
        Orphan(old);
    }

    /// <summary> Removes <paramref name="item"/> (by reference; <see langword="null"/> removes the first JSON <c>null</c>). </summary>
    public bool Remove( JNode? item )
    {
        int index = IndexOf(item);
        if ( index < 0 ) { return false; }

        RemoveAt(index);
        return true;
    }

    public void Clear()
    {
        foreach ( JNode? item in __items ) { Orphan(item); }

        __items.Clear();
    }

    /// <summary> By reference: nodes don't override <see cref="object.Equals(object)"/> (use <see cref="JNode.DeepEquals"/> for structure). </summary>
    public int  IndexOf( JNode?  item ) => __items.IndexOf(item);
    public bool Contains( JNode? item ) => __items.IndexOf(item) >= 0;

    public void CopyTo( JNode?[] array, int arrayIndex ) => __items.CopyTo(array, arrayIndex);

    public List<JNode?>.Enumerator          GetEnumerator() => __items.GetEnumerator();
    IEnumerator<JNode?> IEnumerable<JNode?>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.                GetEnumerator() => GetEnumerator();


    internal void AddNew( JNode? item )
    {
        AdoptNew(item);
        __items.Add(item);
    }


    // ─── JNode ───────────────────────────────────────────────────────────────

    public override void WriteTo<TWriter>( ref TWriter writer )
    {
        RuntimeHelpers.EnsureSufficientExecutionStack();
        writer.WriteStartArray();

        foreach ( JNode? item in __items ) { Write(item, ref writer); }

        writer.WriteEndArray();
    }

    public override JArrayNode DeepClone()
    {
        RuntimeHelpers.EnsureSufficientExecutionStack();
        JArrayNode clone = new(__items.Count);

        foreach ( JNode? item in __items ) { clone.AddNew(item?.DeepClone()); }

        return clone;
    }

    private protected override bool DeepEqualsCore( JNode other )
    {
        List<JNode?> items = ( (JArrayNode)other ).__items;
        if ( items.Count != __items.Count ) { return false; }

        for ( int i = 0; i < __items.Count; i++ )
        {
            if ( !DeepEquals(__items[i], items[i]) ) { return false; }
        }

        return true;
    }

    private protected override void AppendSegment( JNode child, ref ValueStringBuilder builder ) => builder.Append('[').AppendSpanFormattable(IndexOf(child), default, CultureInfo.InvariantCulture).Append(']');

    private protected override void RemoveChild( JNode child ) => RemoveAt(IndexOf(child)); // a child is always present: its Parent is this array
}
