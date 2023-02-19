namespace Jakar.Database;


[Serializable]
[DataBaseType( DbType.String )]
public sealed class IDCollection<T> : CollectionAlerts<string>, IReadOnlyCollection<string>, ISpanFormattable where T : IUniqueID<string>
{
    private readonly HashSet<string> _items;


    public override int Count => _items.Count;


    public IDCollection() : this( EqualityComparer<string>.Default ) { }
    public IDCollection( IEqualityComparer<string>? comparer ) : this( new HashSet<string>( comparer ) ) { }
    public IDCollection( int                      capacity ) : this( new HashSet<string>( capacity ) ) { }
    public IDCollection( int                      capacity, IEqualityComparer<string>? comparer ) : this( new HashSet<string>( capacity, comparer ) ) { }
    public IDCollection( IEnumerable<T>           collection ) : this( new HashSet<string>( collection.Select( x => x.ID ) ) ) { }
    internal IDCollection( HashSet<string>          set ) => _items = set;
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
                             .FromJson<List<string>>();

        var collection = new IDCollection<T>( items.Count )
                         {
                             items,
                         };


        return collection;
    }


    // public static implicit operator IDCollection<T, string>( ReadOnlySpan<char> span ) => Create(span);
    public static implicit operator IDCollection<T>?( string? jsonOrCsv ) => Create( jsonOrCsv );
    internal bool Add( string id )
    {
        OnPropertyChanging( nameof(Count) );
        bool result = _items.Add( id );
        Added( id );
        OnPropertyChanged( nameof(Count) );
        return result;
    }
    public bool Add( T          value ) => Add( value.ID );
    private bool Contains( string id ) => _items.Contains( id );
    public bool Contains( T     value ) => Contains( value.ID );
    private bool Remove( string id )
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

    internal void Add( IEnumerable<string>? value )
    {
        if ( value is null ) { return; }

        foreach ( string id in value ) { Add( id ); }
    }


    public void Add( IEnumerable<T>? value ) => Add( value?.Select( x => x.ID ) );
    public void Add( IDCollection<T>? value )
    {
        if ( value is null ) { return; }

        foreach ( string id in value ) { Add( id ); }
    }
    private void Added( string id ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, id ) );


    public void Clear()
    {
        _items.Clear();
        Cleared();
    }
    private void Cleared() => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );

    // public static IDCollection<T, string> Create( in ReadOnlySpan<char> span )
    // {
    //     if ( span.IsEmpty ) { return new IDCollection<T, string>(); }
    //
    //     if ( !span.Contains(SEPARATOR) ) { throw new ArgumentException($"{nameof(span)} doesn't contain a '{SEPARATOR}'"); }
    //
    //     ReadOnlySpan<char> value = span.Trim()
    //                                    .TrimStart('[')
    //                                    .TrimEnd(']');
    //
    //     var collection = new IDCollection<T, string>();
    //
    //     foreach ( ReadOnlySpan<char> section in value.SplitOn(SEPARATOR) ) { collection.Add(string.Parse(section)); }
    //
    //     return collection;
    // }


    public void Init( string? json )
    {
        Clear();
        if ( string.IsNullOrWhiteSpace( json ) ) { return; }

        foreach ( string n in json.FromJson<List<string>>() ) { Add( n ); }
    }
    private void Removed( string id ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, id ) );


    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    public IEnumerator<string> GetEnumerator() => _items.GetEnumerator();
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
