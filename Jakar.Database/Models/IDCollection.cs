namespace Jakar.Database;


[Serializable]
[DataBaseType( DbType.String )]
public sealed class IDCollection<T> : CollectionAlerts<long>, IReadOnlyCollection<long>, ISpanFormattable where T : IUniqueID<long>
{
    private readonly HashSet<long> _items;


    public override int Count => _items.Count;


    public IDCollection() : this( EqualityComparer<long>.Default ) { }
    public IDCollection( IEqualityComparer<long>? comparer ) : this( new HashSet<long>( comparer ) ) { }
    public IDCollection( int                      capacity ) : this( new HashSet<long>( capacity ) ) { }
    public IDCollection( int                      capacity, IEqualityComparer<long>? comparer ) : this( new HashSet<long>( capacity, comparer ) ) { }
    public IDCollection( IEnumerable<T>           collection ) : this( new HashSet<long>( collection.Select( x => x.ID ) ) ) { }
    internal IDCollection( HashSet<long>          set ) => _items = set;
    public const char SEPARATOR = ',';


    /// <summary> </summary>
    /// <typeparamref name="T"/>
    /// <param name="items"> </param>
    /// <returns>
    ///     Returns a IDCollection if <paramref name="items"/> is not null.
    ///     <para> Otherwise returns <see langword="null"/> </para>
    /// </returns>
    public static IDCollection<T>? Create( [NotNullIfNotNull( "items" )] ICollection<T>? items )
    {
        if ( items is null ) { return null; }

        var collection = new IDCollection<T>( items.Count );

        foreach ( T value in items ) { collection.Add( value ); }

        return collection;
    }
    public static IDCollection<T>? Create( [NotNullIfNotNull( "jsonOrCsv" )] string? jsonOrCsv )
    {
        if ( string.IsNullOrWhiteSpace( jsonOrCsv ) ) { return null; }

        var items = jsonOrCsv.Replace( "\"", string.Empty )
                             .FromJson<List<long>>();

        var collection = new IDCollection<T>( items.Count )
                         {
                             items,
                         };


        return collection;
    }


    // public static implicit operator IDCollection<T, long>( ReadOnlySpan<char> span ) => Create(span);
    public static implicit operator IDCollection<T>?( string? jsonOrCsv ) => Create( jsonOrCsv );
    internal bool Add( long id )
    {
        OnPropertyChanging( nameof(Count) );
        bool result = _items.Add( id );
        Added( id );
        OnPropertyChanged( nameof(Count) );
        return result;
    }
    public bool Add( T          value ) => Add( value.ID );
    private bool Contains( long id ) => _items.Contains( id );
    public bool Contains( T     value ) => Contains( value.ID );
    private bool Remove( long id )
    {
        OnPropertyChanging( nameof(Count) );
        bool result = _items.Remove( id );
        Removed( id );
        OnPropertyChanged( nameof(Count) );
        return result;
    }
    public bool Remove( T value ) => Remove( value.ID );
    public string ToCsv() => $"\"{string.Join( SEPARATOR, this )}\"";


    public string ToJson() => JsonNet.ToJson( this );
    public string ToPrettyJson() => JsonNet.ToPrettyJson( this );

    /// <summary> Gets the JSON representation of this collection </summary>
    public override string ToString() => ToJson();
    public string ToString( ReadOnlySpan<char> format, IFormatProvider? _ )
    {
        if ( format.IsEmpty ) { return ToString(); }

        Span<char> span = stackalloc char[format.Length];
        format.ToUpperInvariant( span );

        if ( span.SequenceEqual( "CSV" ) ) { return ToCsv(); }

        if ( span.SequenceEqual( "JSON" ) ) { return ToJson(); }

        return ToString();
    }

    internal void Add( IEnumerable<long>? value )
    {
        if ( value is null ) { return; }

        foreach ( long id in value ) { Add( id ); }
    }


    public void Add( IEnumerable<T>? value ) => Add( value?.Select( x => x.ID ) );
    public void Add( IDCollection<T>? value )
    {
        if ( value is null ) { return; }

        foreach ( long id in value ) { Add( id ); }
    }
    private void Added( long id ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, id ) );


    public void Clear()
    {
        _items.Clear();
        Cleared();
    }
    private void Cleared() => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );

    // public static IDCollection<T, long> Create( in ReadOnlySpan<char> span )
    // {
    //     if ( span.IsEmpty ) { return new IDCollection<T, long>(); }
    //
    //     if ( !span.Contains(SEPARATOR) ) { throw new ArgumentException($"{nameof(span)} doesn't contain a '{SEPARATOR}'"); }
    //
    //     ReadOnlySpan<char> value = span.Trim()
    //                                    .TrimStart('[')
    //                                    .TrimEnd(']');
    //
    //     var collection = new IDCollection<T, long>();
    //
    //     foreach ( ReadOnlySpan<char> section in value.SplitOn(SEPARATOR) ) { collection.Add(long.Parse(section)); }
    //
    //     return collection;
    // }


    public void Init( string? json )
    {
        Clear();
        if ( string.IsNullOrWhiteSpace( json ) ) { return; }

        foreach ( long n in json.FromJson<List<long>>() ) { Add( n ); }
    }
    private void Removed( long id ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, id ) );


    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    public IEnumerator<long> GetEnumerator() => _items.GetEnumerator();
    public string ToString( string? format, IFormatProvider? formatProvider )
    {
        ReadOnlySpan<char> span = format;
        return ToString( span, formatProvider );
    }


    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        ReadOnlySpan<char> result = ToString( format, provider );
        result.CopyTo( destination );
        charsWritten = result.Length;
        return true;
    }
}
