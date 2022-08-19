using System.Collections;



namespace Jakar.Database;


[Serializable]
[DataBaseType(DbType.String)]
public sealed class IDCollection<T, TID> : IReadOnlyCollection<TID>, ICollectionAlerts, ISpanFormattable where T : IUniqueID<TID>
                                                                                                         where TID : IComparable<TID>, IEquatable<TID>
{
    public const     char         SEPARATOR = ',';
    private readonly HashSet<TID> _items;


    public int Count => _items.Count;


    public IDCollection() : this(EqualityComparer<TID>.Default) { }
    public IDCollection( IEqualityComparer<TID>? comparer ) : this(new HashSet<TID>(comparer)) { }
    public IDCollection( int                     capacity ) : this(new HashSet<TID>(capacity)) { }
    public IDCollection( int                     capacity, IEqualityComparer<TID>? comparer ) : this(new HashSet<TID>(capacity, comparer)) { }
    public IDCollection( IEnumerable<T>          collection ) : this(new HashSet<TID>(collection.Select(x => x.ID))) { }
    internal IDCollection( HashSet<TID>          set ) => _items = set;


    private void Add( IEnumerable<TID>? value )
    {
        if ( value is null ) { return; }

        foreach ( TID id in value ) { Add(id); }
    }
    private bool Add( TID id )
    {
        OnPropertyChanging(nameof(Count));
        bool result = _items.Add(id);
        Added(id);
        OnPropertyChanged(nameof(Count));
        return result;
    }


    public void Add( IEnumerable<T>? value ) => Add(value?.Select(x => x.ID));
    public bool Add( T               value ) => Add(value.ID);
    public void Add( IDCollection<T, TID>? value )
    {
        if ( value is null ) { return; }

        foreach ( TID id in value ) { Add(id); }
    }
    private void Added( TID id ) => OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, id));


    public void Clear()
    {
        _items.Clear();
        Cleared();
    }
    private void Cleared() => OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    private bool Contains( TID id ) => _items.Contains(id);
    public bool Contains( T    value ) => Contains(value.ID);


    /// <summary>
    /// </summary>
    /// <typeparamref name = "T" />
    /// <param name = "items" > </param>
    /// <returns>
    ///     Returns a IDCollection if
    ///     <paramref name = "items" />
    ///     is not null.
    ///     <para>
    ///         Otherwise returns
    ///         <see langword = "null" />
    ///     </para>
    /// </returns>
    public static IDCollection<T, TID>? Create( [NotNullIfNotNull("items")] ICollection<T>? items )
    {
        if ( items is null ) { return null; }

        var collection = new IDCollection<T, TID>(items.Count);

        foreach ( T value in items ) { collection.Add(value); }

        return collection;
    }
    public static IDCollection<T, TID>? Create( [NotNullIfNotNull("jsonOrCsv")] string? jsonOrCsv )
    {
        if ( string.IsNullOrWhiteSpace(jsonOrCsv) ) { return null; }

        var items = jsonOrCsv.Replace("\"", string.Empty)
                             .FromJson<List<TID>>();

        var collection = new IDCollection<T, TID>(items.Count)
                         {
                             items
                         };


        return collection;
    }
    // public static IDCollection<T, TID> Create( in ReadOnlySpan<char> span )
    // {
    //     if ( span.IsEmpty ) { return new IDCollection<T, TID>(); }
    //
    //     if ( !span.Contains(SEPARATOR) ) { throw new ArgumentException($"{nameof(span)} doesn't contain a '{SEPARATOR}'"); }
    //
    //     ReadOnlySpan<char> value = span.Trim()
    //                                    .TrimStart('[')
    //                                    .TrimEnd(']');
    //
    //     var collection = new IDCollection<T, TID>();
    //
    //     foreach ( ReadOnlySpan<char> section in value.SplitOn(SEPARATOR) ) { collection.Add(long.Parse(section)); }
    //
    //     return collection;
    // }


    public void Init( string? json )
    {
        Clear();
        if ( string.IsNullOrWhiteSpace(json) ) { return; }

        foreach ( TID n in json.FromJson<List<TID>>() ) { Add(n); }
    }
    private void OnChanged( NotifyCollectionChangedEventArgs    e ) => CollectionChanged?.Invoke(this, e);
    private void OnPropertyChanged( [CallerMemberName]  string? propertyName = default ) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    private void OnPropertyChanging( [CallerMemberName] string? propertyName = default ) => PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));


    // public static implicit operator IDCollection<T, TID>( ReadOnlySpan<char> span ) => Create(span);

    public static implicit operator IDCollection<T, TID>?( string? jsonOrCsv ) => Create(jsonOrCsv);
    private bool Remove( TID id )
    {
        OnPropertyChanging(nameof(Count));
        bool result = _items.Remove(id);
        Removed(id);
        OnPropertyChanged(nameof(Count));
        return result;
    }
    public bool Remove( T     value ) => Remove(value.ID);
    private void Removed( TID id ) => OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, id));
    public string ToCsv() => $"\"{string.Join(SEPARATOR, this)}\"";


    public string ToJson() => JsonNet.ToJson(this);
    public string ToPrettyJson() => JsonNet.ToPrettyJson(this);

    /// <summary> Gets the JSON representation of this collection </summary>
    public override string ToString() => ToJson();
    public string ToString( ReadOnlySpan<char> format, IFormatProvider? _ )
    {
        if ( format.IsEmpty ) { return ToString(); }

        Span<char> span = stackalloc char[format.Length];
        format.ToUpperInvariant(span);

        if ( span.SequenceEqual("CSV") ) { return ToCsv(); }

        if ( span.SequenceEqual("JSON") ) { return ToJson(); }

        return ToString();
    }


    void ICollectionAlerts.SendOnChanged( NotifyCollectionChangedEventArgs e ) => CollectionChanged?.Invoke(this, e);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    public IEnumerator<TID> GetEnumerator() => _items.GetEnumerator();
    public string ToString( string? format, IFormatProvider? formatProvider )
    {
        ReadOnlySpan<char> span = format;
        return ToString(span, formatProvider);
    }


    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    public event PropertyChangedEventHandler?         PropertyChanged;
    public event PropertyChangingEventHandler?        PropertyChanging;
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        ReadOnlySpan<char> result = ToString(format, provider);
        result.CopyTo(destination);
        charsWritten = result.Length;
        return true;
    }
}
