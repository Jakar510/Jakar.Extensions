// TrueLogic :: TrueLogic.Common
// 05/12/2022  5:00 PM

namespace Jakar.Database;


[Serializable]
[DataBaseType(DbType.String)]
public class CollectionWrapper<TValue, TOwner, TID> : CollectionAlerts<TValue>, ICollectionWrapper<TValue, TID> where TValue : IUniqueID<TID>
                                                                                                                where TOwner : BaseTableRecord<TOwner, TID>
                                                                                                                where TID : struct, IComparable<TID>, IEquatable<TID>
{
    private IDCollection<TValue, TID>? _items;
    private string?                    _json;


    public sealed override int  Count      => _items?.Count ?? 0;
    public                 bool IsEmpty    => Count == 0;
    public                 bool IsNotEmpty => Count > 0;


    internal IDCollection<TValue, TID> Items
    {
        get => _items ?? throw new NullReferenceException(nameof(_items));
        set
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if ( _items is not null )
            {
                _items.CollectionChanged -= Items_OnCollectionChanged;
                _items.PropertyChanged   -= Items_OnPropertyChanged;
                _items.PropertyChanging  -= Items_OnPropertyChanging;
            }

            SetProperty(ref _items, value);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if ( _items is null ) { return; }

            _items.CollectionChanged += Items_OnCollectionChanged;
            _items.PropertyChanged   += Items_OnPropertyChanged;
            _items.PropertyChanging  += Items_OnPropertyChanging;
        }
    }


    public string? Json
    {
        get => _json;
        set
        {
            SetProperty(ref _json, value);
            Items = IDCollection<TValue, TID>.Create(value) ?? new IDCollection<TValue, TID>();
        }
    }


    public CollectionWrapper() : this(new IDCollection<TValue, TID>()) { }
    public CollectionWrapper( string?                       json ) : this(IDCollection<TValue, TID>.Create(json)) { }
    public CollectionWrapper( params TValue[]?              value ) : this(IDCollection<TValue, TID>.Create(value)) { }
    public CollectionWrapper( ICollection<TValue>?          collection ) : this(IDCollection<TValue, TID>.Create(collection)) { }
    protected CollectionWrapper( IDCollection<TValue, TID>? collection ) => Items = collection ?? new IDCollection<TValue, TID>();

    // public CollectionWrapper(  ReadOnlySpan<char>   span ) : this( IDCollection<TValue, TID>.Create(span)) { }
    public virtual void Dispose()
    {
        Items = null!;
        GC.SuppressFinalize(this);
    }


    public static CollectionWrapper<TValue, TOwner, TID> Create( ICollection<TValue>? value )
    {
        if ( value is null ) { return new CollectionWrapper<TValue, TOwner, TID>(); }

        var collection = new CollectionWrapper<TValue, TOwner, TID>(value);

        return collection;
    }
    public static CollectionWrapper<TValue, TOwner, TID> Create( string? jsonOrCsv )
    {
        if ( string.IsNullOrWhiteSpace(jsonOrCsv) ) { return new CollectionWrapper<TValue, TOwner, TID>(); }

        jsonOrCsv = jsonOrCsv.Replace("\"", string.Empty);

        return new CollectionWrapper<TValue, TOwner, TID>(IDCollection<TValue, TID>.Create(jsonOrCsv));
    }


    public void Add( IEnumerable<TValue>?       value ) => Items.Add(value);
    public void Add( params TValue[]?           value ) => Items.Add(value);
    public void Add( HashSet<TValue>?           value ) => Items.Add(value);
    public bool Add( TValue                     value ) => Items.Add(value);
    public void Add( IDCollection<TValue, TID>? value ) => Items.Add(value);
    public bool Remove( TValue                  value ) => Items.Remove(value);
    public bool Contains( TValue                value ) => Items.Contains(value);
    public virtual void SetValues( string?      json, [CallerMemberName] string? caller = default ) => Items.Init(json);


    private void Items_OnCollectionChanged( object? sender, NotifyCollectionChangedEventArgs e )
    {
        SetProperty(ref _json, Items.ToJson(), nameof(Json));
        OnChanged(e);
    }
    private void Items_OnPropertyChanged( object?  sender, PropertyChangedEventArgs  e ) => OnPropertyChanged(e.PropertyName);
    private void Items_OnPropertyChanging( object? sender, PropertyChangingEventArgs e ) => OnPropertyChanging(e.PropertyName);


    public string ToJson() => Items.ToJson();
    public string ToPrettyJson() => Items.ToPrettyJson();


    /// <summary> Gets the JSON representation of this collection </summary>
    public override string ToString() => Json ?? ToJson();
    public string ToString( ReadOnlySpan<char> format, IFormatProvider? _ )
    {
        if ( format.IsEmpty ) { return ToString(); }

        Span<char> span = stackalloc char[format.Length];
        format.ToUpperInvariant(span);

        if ( span.SequenceEqual("CSV") ) { return Items.ToCsv(); }

        if ( span.SequenceEqual("JSON") ) { return Items.ToJson(); }

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
        return ToString(span, formatProvider);
    }
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        ReadOnlySpan<char> result = ToString(format, provider);
        result.CopyTo(destination);
        charsWritten = result.Length;
        return true;
    }


    public IEnumerator<TID> GetEnumerator()
    {
        if ( _items is null ) { yield break; }

        foreach ( TID id in _items ) { yield return id; }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override bool Equals( object? obj ) => ReferenceEquals(this, obj) || ( obj is CollectionWrapper<TValue, TOwner, TID> other && Equals(other) );
    public bool Equals( ICollectionWrapper<TValue, TID>? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return Json == other.Json;
    }
    public override int GetHashCode() => HashCode.Combine(Json);


    public static bool operator ==( CollectionWrapper<TValue, TOwner, TID>? left, CollectionWrapper<TValue, TOwner, TID>? right ) => Equals(left, right);
    public static bool operator !=( CollectionWrapper<TValue, TOwner, TID>? left, CollectionWrapper<TValue, TOwner, TID>? right ) => !Equals(left, right);



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
