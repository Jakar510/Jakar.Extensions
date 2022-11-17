// TrueLogic :: TrueLogic.Common
// 05/12/2022  5:00 PM

namespace Jakar.Database;


[Serializable]
[DataBaseType( DbType.String )]
public class CollectionWrapper<TValue, TOwner> : CollectionAlerts<TValue>, ICollectionWrapper<TValue> where TValue : IUniqueID<long>
                                                                                                      where TOwner : TableRecord<TOwner>
{
    private IDCollection<TValue>? _items;
    private string?               _json;
    public  bool                  IsEmpty    => Count == 0;
    public  bool                  IsNotEmpty => Count > 0;


    internal IDCollection<TValue> Items
    {
        get => _items ?? throw new NullReferenceException( nameof(_items) );
        set
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if ( _items is not null )
            {
                _items.CollectionChanged -= Items_OnCollectionChanged;
                _items.PropertyChanged   -= Items_OnPropertyChanged;
                _items.PropertyChanging  -= Items_OnPropertyChanging;
            }

            SetProperty( ref _items, value );

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if ( _items is null ) { return; }

            _items.CollectionChanged += Items_OnCollectionChanged;
            _items.PropertyChanged   += Items_OnPropertyChanged;
            _items.PropertyChanging  += Items_OnPropertyChanging;
        }
    }


    public sealed override int Count => _items?.Count ?? 0;


    public string? Json
    {
        get => _json;
        set
        {
            SetProperty( ref _json, value );
            Items = IDCollection<TValue>.Create( value ) ?? new IDCollection<TValue>();
        }
    }


    public CollectionWrapper() : this( new IDCollection<TValue>() ) { }
    public CollectionWrapper( string?                  json ) : this( IDCollection<TValue>.Create( json ) ) { }
    public CollectionWrapper( params TValue[]?         value ) : this( IDCollection<TValue>.Create( value ) ) { }
    public CollectionWrapper( ICollection<TValue>?     collection ) : this( IDCollection<TValue>.Create( collection ) ) { }
    protected CollectionWrapper( IDCollection<TValue>? collection ) => Items = collection ?? new IDCollection<TValue>();


    public static bool operator ==( CollectionWrapper<TValue, TOwner>? left, CollectionWrapper<TValue, TOwner>? right ) => Equals( left, right );
    public static bool operator !=( CollectionWrapper<TValue, TOwner>? left, CollectionWrapper<TValue, TOwner>? right ) => !Equals( left, right );


    public static CollectionWrapper<TValue, TOwner> Create( ICollection<TValue>? value )
    {
        if ( value is null ) { return new CollectionWrapper<TValue, TOwner>(); }

        var collection = new CollectionWrapper<TValue, TOwner>( value );

        return collection;
    }
    public static CollectionWrapper<TValue, TOwner> Create( string? jsonOrCsv )
    {
        if ( string.IsNullOrWhiteSpace( jsonOrCsv ) ) { return new CollectionWrapper<TValue, TOwner>(); }

        jsonOrCsv = jsonOrCsv.Replace( "\"", string.Empty );

        return new CollectionWrapper<TValue, TOwner>( IDCollection<TValue>.Create( jsonOrCsv ) );
    }

    public override bool Equals( object? obj ) => ReferenceEquals( this, obj ) || obj is CollectionWrapper<TValue, TOwner> other && Equals( other );
    public override int GetHashCode() => HashCode.Combine( Json );


    private void Items_OnCollectionChanged( object? sender, NotifyCollectionChangedEventArgs e )
    {
        SetProperty( ref _json, Items.ToJson(), nameof(Json) );
        OnChanged( e );
    }
    private void Items_OnPropertyChanged( object?  sender, PropertyChangedEventArgs  e ) => OnPropertyChanged( e.PropertyName );
    private void Items_OnPropertyChanging( object? sender, PropertyChangingEventArgs e ) => OnPropertyChanging( e.PropertyName );

    // public CollectionWrapper(  ReadOnlySpan<char>   span ) : this( IDCollection<TValue>.Create(span)) { }
    public virtual void Dispose()
    {
        Items = null!;
        GC.SuppressFinalize( this );
    }


    public void Add( IEnumerable<TValue>?        value ) => Items.Add( value );
    public void Add( params TValue[]?            value ) => Items.Add( value );
    public void Add( HashSet<TValue>?            value ) => Items.Add( value );
    public bool Add( TValue                      value ) => Items.Add( value );
    public void Add( ICollectionWrapper<TValue>? value ) => Items.Add( value );
    public bool Remove( TValue                   value ) => Items.Remove( value );
    public bool Contains( TValue                 value ) => Items.Contains( value );
    public virtual void SetValues( string?       json, [CallerMemberName] string? caller = default ) => Items.Init( json );


    public string ToJson() => Items.ToJson();
    public string ToPrettyJson() => Items.ToPrettyJson();


    /// <summary> Gets the JSON representation of this collection </summary>
    public override string ToString() => Json ?? ToJson();
    public string ToString( ReadOnlySpan<char> format, IFormatProvider? _ )
    {
        if ( format.IsEmpty ) { return ToString(); }

        Span<char> span = stackalloc char[format.Length];
        format.ToUpperInvariant( span );

        if ( span.SequenceEqual( "CSV" ) ) { return Items.ToCsv(); }

        if ( span.SequenceEqual( "JSON" ) ) { return Items.ToJson(); }

        return ToString();
    }


    TypeCode IConvertible.GetTypeCode() => TypeCode.String;


    string IConvertible.ToString( IFormatProvider? provider ) => ToString();
    object IConvertible.ToType( Type conversionType, IFormatProvider? provider )
    {
        if ( conversionType == typeof(string) ) { return ToString(); }

        throw new NotImplementedException();
    }


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


    public IEnumerator<long> GetEnumerator()
    {
        if ( _items is null ) { yield break; }

        foreach ( long id in _items ) { yield return id; }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public bool Equals( ICollectionWrapper<TValue>? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return Json == other.Json;
    }



    #region not used conversions

    bool IConvertible.ToBoolean( IFormatProvider?      provider ) => throw new NotImplementedException();
    byte IConvertible.ToByte( IFormatProvider?         provider ) => throw new NotImplementedException();
    char IConvertible.ToChar( IFormatProvider?         provider ) => throw new NotImplementedException();
    DateTime IConvertible.ToDateTime( IFormatProvider? provider ) => throw new NotImplementedException();
    decimal IConvertible.ToDecimal( IFormatProvider?   provider ) => throw new NotImplementedException();
    double IConvertible.ToDouble( IFormatProvider?     provider ) => throw new NotImplementedException();
    short IConvertible.ToInt16( IFormatProvider?       provider ) => throw new NotImplementedException();
    int IConvertible.ToInt32( IFormatProvider?         provider ) => throw new NotImplementedException();
    long IConvertible.ToInt64( IFormatProvider?        provider ) => throw new NotImplementedException();
    sbyte IConvertible.ToSByte( IFormatProvider?       provider ) => throw new NotImplementedException();
    float IConvertible.ToSingle( IFormatProvider?      provider ) => throw new NotImplementedException();
    ushort IConvertible.ToUInt16( IFormatProvider?     provider ) => throw new NotImplementedException();
    uint IConvertible.ToUInt32( IFormatProvider?       provider ) => throw new NotImplementedException();
    ulong IConvertible.ToUInt64( IFormatProvider?      provider ) => throw new NotImplementedException();

    #endregion
}
