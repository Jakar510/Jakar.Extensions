namespace Jakar.Extensions;


public class MultiDict<TKey> : Dictionary<TKey, object?>
    where TKey : notnull
{
    #region ctor

    public MultiDict() : this( 0 ) { }
    public MultiDict( IEqualityComparer<TKey>?                 comparer ) : base( 0, comparer ) { }
    public MultiDict( int                                      capacity,   IEqualityComparer<TKey>? comparer = null ) : base( capacity, comparer ) { }
    public MultiDict( IDictionary<TKey, object?>               dictionary, IEqualityComparer<TKey>? comparer = null ) : base( dictionary, comparer ) { }
    public MultiDict( IEnumerable<KeyValuePair<TKey, object?>> collection, IEqualityComparer<TKey>? comparer = null ) : base( collection, comparer ) { }
    [Obsolete] protected MultiDict( SerializationInfo          info,       StreamingContext         context ) : base( info, context ) { }

    #endregion



    #region Gets

    public bool ValueAs( TKey key, out double    value ) => ValueAs<double>( key, out value );
    public bool ValueAs( TKey key, out double?   value ) => ValueAs<double?>( key, out value );
    public bool ValueAs( TKey key, out float     value ) => ValueAs<float>( key, out value );
    public bool ValueAs( TKey key, out float?    value ) => ValueAs<float?>( key, out value );
    public bool ValueAs( TKey key, out long      value ) => ValueAs<long>( key, out value );
    public bool ValueAs( TKey key, out long?     value ) => ValueAs<long?>( key, out value );
    public bool ValueAs( TKey key, out ulong     value ) => ValueAs<ulong>( key, out value );
    public bool ValueAs( TKey key, out ulong?    value ) => ValueAs<ulong?>( key, out value );
    public bool ValueAs( TKey key, out int       value ) => ValueAs<int>( key, out value );
    public bool ValueAs( TKey key, out int?      value ) => ValueAs<int?>( key, out value );
    public bool ValueAs( TKey key, out uint      value ) => ValueAs<uint>( key, out value );
    public bool ValueAs( TKey key, out uint?     value ) => ValueAs<uint?>( key, out value );
    public bool ValueAs( TKey key, out short     value ) => ValueAs<short>( key, out value );
    public bool ValueAs( TKey key, out short?    value ) => ValueAs<short?>( key, out value );
    public bool ValueAs( TKey key, out ushort    value ) => ValueAs<ushort>( key, out value );
    public bool ValueAs( TKey key, out ushort?   value ) => ValueAs<ushort?>( key, out value );
    public bool ValueAs( TKey key, out Guid      value ) => ValueAs<Guid>( key, out value );
    public bool ValueAs( TKey key, out Guid?     value ) => ValueAs<Guid?>( key, out value );
    public bool ValueAs( TKey key, out DateTime  value ) => ValueAs<DateTime>( key, out value );
    public bool ValueAs( TKey key, out DateTime? value ) => ValueAs<DateTime?>( key, out value );
    public bool ValueAs( TKey key, out TimeSpan  value ) => ValueAs<TimeSpan>( key, out value );
    public bool ValueAs( TKey key, out TimeSpan? value ) => ValueAs<TimeSpan?>( key, out value );
    public bool ValueAs( TKey key, out bool      value ) => ValueAs<bool>( key, out value );
    public bool ValueAs( TKey key, out bool?     value ) => ValueAs<bool?>( key, out value );


    public bool ValueAs( TKey key, [NotNullWhen( true )] out IPAddress?  value ) => ValueAs<IPAddress>( key, out value );
    public bool ValueAs( TKey key, [NotNullWhen( true )] out AppVersion? value ) => ValueAs<AppVersion?>( key, out value );
    public bool ValueAs( TKey key, [NotNullWhen( true )] out Version?    value ) => ValueAs<Version>( key, out value );


    public bool ValueAs<T>( TKey key, [NotNullWhen( true )] out T? value )
    {
        object? s = this[key];

        if ( s is T item )
        {
            value = item;
            return true;
        }

        value = default;
        return false;
    }


    public T ValueAs<T>( TKey key )
    {
        if ( !ContainsKey( key ) ) { throw new KeyNotFoundException( key.ToString() ); }

        return ExpectedValueTypeException<TKey>.Verify<T>( this[key], key );
    }

    #endregion



    #region Adds

    public void Add<T>( TKey key, T value ) => this[key] = value;

    public void Add( TKey key, double     value ) => this[key] = value;
    public void Add( TKey key, double?    value ) => this[key] = value;
    public void Add( TKey key, float      value ) => this[key] = value;
    public void Add( TKey key, float?     value ) => this[key] = value;
    public void Add( TKey key, long       value ) => this[key] = value;
    public void Add( TKey key, long?      value ) => this[key] = value;
    public void Add( TKey key, ulong      value ) => this[key] = value;
    public void Add( TKey key, ulong?     value ) => this[key] = value;
    public void Add( TKey key, int        value ) => this[key] = value;
    public void Add( TKey key, uint?      value ) => this[key] = value;
    public void Add( TKey key, short      value ) => this[key] = value;
    public void Add( TKey key, short?     value ) => this[key] = value;
    public void Add( TKey key, ushort     value ) => this[key] = value;
    public void Add( TKey key, ushort?    value ) => this[key] = value;
    public void Add( TKey key, Guid       value ) => this[key] = value;
    public void Add( TKey key, Guid?      value ) => this[key] = value;
    public void Add( TKey key, DateTime   value ) => this[key] = value;
    public void Add( TKey key, DateTime?  value ) => this[key] = value;
    public void Add( TKey key, TimeSpan   value ) => this[key] = value;
    public void Add( TKey key, TimeSpan?  value ) => this[key] = value;
    public void Add( TKey key, bool       value ) => this[key] = value;
    public void Add( TKey key, bool?      value ) => this[key] = value;
    public void Add( TKey key, IPAddress  value ) => this[key] = value;
    public void Add( TKey key, AppVersion value ) => this[key] = value;
    public void Add( TKey key, Version    value ) => this[key] = value;

    #endregion
}



public class MultiDict : MultiDict<string>
{
    public MultiDict() : this( 0 ) { }
    public MultiDict( IEqualityComparer<string>?                 comparer ) : base( 0, comparer ) { }
    public MultiDict( int                                        capacity,   IEqualityComparer<string>? comparer = null ) : base( capacity, comparer ) { }
    public MultiDict( IDictionary<string, object?>               dictionary, IEqualityComparer<string>? comparer = null ) : base( dictionary, comparer ) { }
    public MultiDict( IEnumerable<KeyValuePair<string, object?>> collection, IEqualityComparer<string>? comparer = null ) : base( collection, comparer ) { }
    [Obsolete] protected MultiDict( SerializationInfo            info,       StreamingContext           context ) : base( info, context ) { }
}
