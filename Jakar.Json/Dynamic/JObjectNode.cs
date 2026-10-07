// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary>
///     A mutable JSON object: ordinal member names in insertion order (the order they are written in). Values may be <see langword="null"/> (JSON <c>null</c>).
/// </summary>
/// <remarks> Like Newtonsoft's <c>JObject</c>, the indexer returns <see langword="null"/> for a missing member instead of throwing, even through <see cref="IDictionary{TKey,TValue}"/>; use <see cref="ContainsKey"/> or <see cref="TryGetValue"/> to tell missing from JSON <c>null</c>. </remarks>
public sealed class JObjectNode : JNode, IDictionary<string, JNode?>, IReadOnlyDictionary<string, JNode?>
{
    private static readonly SearchValues<char> __identifierChars = SearchValues.Create("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_");

    private readonly OrderedDictionary<string, JNode?> __members;


    public JObjectNode() => __members = new OrderedDictionary<string, JNode?>(StringComparer.Ordinal);
    public JObjectNode( int capacity ) => __members = new OrderedDictionary<string, JNode?>(capacity, StringComparer.Ordinal);
    /// <exception cref="ArgumentException"> A name repeats. </exception>
    /// <exception cref="InvalidOperationException"> A value already has a parent. </exception>
    public JObjectNode( params ReadOnlySpan<KeyValuePair<string, JNode?>> members ) : this(members.Length)
    {
        foreach ( KeyValuePair<string, JNode?> member in members ) { Add(member.Key, member.Value); }
    }


    /// <exception cref="JsonReadException"> The JSON is malformed, or a member name repeats. </exception>
    /// <exception cref="FormatException"> The root isn't an object. </exception>
    public static new JObjectNode Parse( string json, JsonReaderOptions? options = null ) => JNode.Parse(json, options) as JObjectNode ?? throw new FormatException("The JSON root isn't an object.");

    /// <inheritdoc cref="Parse(string, JsonReaderOptions?)"/>
    public static new JObjectNode Parse( ReadOnlySpan<byte> utf8Json, JsonReaderOptions? options = null ) => JNode.Parse(utf8Json, options) as JObjectNode ?? throw new FormatException("The JSON root isn't an object.");


    public override JNodeKind Kind  => JNodeKind.Object;
    public          int       Count => __members.Count;

    public OrderedDictionary<string, JNode?>.KeyCollection   Keys   => __members.Keys;
    public OrderedDictionary<string, JNode?>.ValueCollection Values => __members.Values;

    ICollection<string> IDictionary<string, JNode?>.        Keys       => __members.Keys;
    ICollection<JNode?> IDictionary<string, JNode?>.        Values     => __members.Values;
    IEnumerable<string> IReadOnlyDictionary<string, JNode?>.Keys       => __members.Keys;
    IEnumerable<JNode?> IReadOnlyDictionary<string, JNode?>.Values     => __members.Values;
    bool ICollection<KeyValuePair<string, JNode?>>.         IsReadOnly => false;


    /// <summary> Get: the member's value, or <see langword="null"/> when it's missing. Set: adds or replaces the member (a new member goes last). </summary>
    /// <exception cref="InvalidOperationException"> <paramref name="value"/> already has a parent, or is this object or one of its ancestors. </exception>
    public override JNode? this[ string name ]
    {
        get => __members.TryGetValue(name, out JNode? value)
                   ? value
                   : null;
        set
        {
            ArgumentNullException.ThrowIfNull(name);

            if ( __members.TryGetValue(name, out JNode? old) )
            {
                if ( ReferenceEquals(old, value) ) { return; }

                Adopt(value); // validates before anything changes
                __members[name] = value;
                Orphan(old);
                return;
            }

            Adopt(value);
            __members.Add(name, value);
        }
    }


    /// <summary> The member at <paramref name="index"/>, in document order. </summary>
    public KeyValuePair<string, JNode?> GetAt( int index ) => __members.GetAt(index);

    public bool ContainsKey( string name ) => __members.ContainsKey(name);

    public bool TryGetValue( string name, [MaybeNullWhen(false)] out JNode? value ) => __members.TryGetValue(name, out value);

    /// <exception cref="ArgumentException"> The object already has a member named <paramref name="name"/>. </exception>
    /// <exception cref="InvalidOperationException"> <paramref name="value"/> already has a parent, or is this object or one of its ancestors. </exception>
    public void Add( string name, JNode? value )
    {
        if ( !TryAdd(name, value) ) { throw new ArgumentException($"The object already has a member '{name}'.", nameof(name)); }
    }

    /// <summary> <see langword="false"/> (and nothing changes) if the name is taken. </summary>
    /// <exception cref="InvalidOperationException"> <paramref name="value"/> already has a parent, or is this object or one of its ancestors. </exception>
    public bool TryAdd( string name, JNode? value )
    {
        ArgumentNullException.ThrowIfNull(name);
        if ( __members.ContainsKey(name) ) { return false; }

        Adopt(value);
        __members.Add(name, value);
        return true;
    }

    public bool Remove( string name ) => Remove(name, out _);

    public bool Remove( string name, out JNode? value )
    {
        if ( !__members.Remove(name, out value) ) { return false; }

        Orphan(value);
        return true;
    }

    public void Clear()
    {
        foreach ( KeyValuePair<string, JNode?> member in __members ) { Orphan(member.Value); }

        __members.Clear();
    }

    public OrderedDictionary<string, JNode?>.Enumerator                                 GetEnumerator() => __members.GetEnumerator();
    IEnumerator<KeyValuePair<string, JNode?>> IEnumerable<KeyValuePair<string, JNode?>>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.                                                            GetEnumerator() => GetEnumerator();


    void ICollection<KeyValuePair<string, JNode?>>.Add( KeyValuePair<string, JNode?>      member )                => Add(member.Key, member.Value);
    bool ICollection<KeyValuePair<string, JNode?>>.Contains( KeyValuePair<string, JNode?> member )                => __members.TryGetValue(member.Key, out JNode? value)                  && ReferenceEquals(value, member.Value);
    bool ICollection<KeyValuePair<string, JNode?>>.Remove( KeyValuePair<string, JNode?>   member )                => ( (ICollection<KeyValuePair<string, JNode?>>)this ).Contains(member) && Remove(member.Key);
    void ICollection<KeyValuePair<string, JNode?>>.CopyTo( KeyValuePair<string, JNode?>[] array, int arrayIndex ) => ( (ICollection<KeyValuePair<string, JNode?>>)__members ).CopyTo(array, arrayIndex);


    internal bool TryAddNew( string name, JNode? value )
    {
        if ( !__members.TryAdd(name, value) ) { return false; }

        AdoptNew(value);
        return true;
    }


    // ─── JNode ───────────────────────────────────────────────────────────────

    public override void WriteTo<TWriter>( ref TWriter writer )
    {
        RuntimeHelpers.EnsureSufficientExecutionStack();
        writer.WriteStartObject();

        foreach ( KeyValuePair<string, JNode?> member in __members )
        {
            writer.WritePropertyName(member.Key); // escaped by the writer
            Write(member.Value, ref writer);
        }

        writer.WriteEndObject();
    }

    public override JObjectNode DeepClone()
    {
        RuntimeHelpers.EnsureSufficientExecutionStack();
        JObjectNode clone = new(__members.Count);

        foreach ( KeyValuePair<string, JNode?> member in __members ) { clone.TryAddNew(member.Key, member.Value?.DeepClone()); }

        return clone;
    }

    /// <summary> Same names with deep-equal values, in any order. </summary>
    private protected override bool DeepEqualsCore( JNode other )
    {
        OrderedDictionary<string, JNode?> members = ( (JObjectNode)other ).__members;
        if ( members.Count != __members.Count ) { return false; }

        foreach ( KeyValuePair<string, JNode?> member in __members )
        {
            if ( !members.TryGetValue(member.Key, out JNode? value) || !DeepEquals(member.Value, value) ) { return false; }
        }

        return true;
    }

    private protected override void AppendSegment( JNode child, ref ValueStringBuilder builder )
    {
        foreach ( KeyValuePair<string, JNode?> member in __members )
        {
            if ( !ReferenceEquals(member.Value, child) ) { continue; }

            string name = member.Key;

            if ( name.Length > 0 && !char.IsAsciiDigit(name[0]) && !name.AsSpan().ContainsAnyExcept(__identifierChars) ) { builder.Append('.').Append(name); }
            else { builder.Append("['").Append(name.Replace("'", "\\'")).Append("']"); }

            return;
        }
    }

    private protected override void RemoveChild( JNode child )
    {
        for ( int i = 0; i < __members.Count; i++ )
        {
            if ( !ReferenceEquals(__members.GetAt(i).Value, child) ) { continue; }

            __members.RemoveAt(i); // keeps the remaining members in order
            Orphan(child);
            return;
        }
    }
}
