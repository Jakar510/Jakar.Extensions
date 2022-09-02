// Jakar.Extensions :: Jakar.Database
// 09/02/2022  3:43 PM

using Microsoft.Extensions.Caching.Memory;



namespace Jakar.Database;


public class CacheEntry<TRecord, TID> : ObservableClass, IEquatable<TRecord>, IComparable<TRecord>, IEquatable<CacheEntry<TRecord, TID>>, IComparable<CacheEntry<TRecord, TID>>, IComparable where TRecord : BaseTableRecord<TRecord, TID>
                                                                                                                                                                                             where TID : IComparable<TID>, IEquatable<TID>
{
    private TRecord?       _value;
    private int            _hash;
    private DateTimeOffset _lastTime;


    public TRecord Value
    {
        get => _value ?? throw new NullReferenceException(nameof(_value));
        set
        {
            if ( !SetProperty(ref _value, value) ) { return; }

            _hash     = value.GetHashCode();
            _lastTime = DateTimeOffset.Now;
        }
    }
    public TID  ID         => Value.ID;
    public bool HasChanged => Value.GetHashCode() != _hash;


    public CacheEntry( TRecord                                        value ) => Value = value;
    public static implicit operator CacheEntry<TRecord, TID>( TRecord value ) => new(value);


    public bool HasExpired( in TimeSpan time ) => DateTimeOffset.Now - _lastTime >= time;


    public bool Equals( TRecord?   other ) => Value.Equals(other);
    public int CompareTo( TRecord? other ) => Value.CompareTo(other);
    public bool Equals( CacheEntry<TRecord, TID>? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return Value.Equals(other.Value);
    }
    public int CompareTo( CacheEntry<TRecord, TID>? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        return Value.CompareTo(other.Value);
    }
    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        return other is CacheEntry<TRecord, TID> entry
                   ? CompareTo(entry)
                   : throw new ArgumentException($"Object must be of type {nameof(CacheEntry<TRecord, TID>)}");
    }
    public override bool Equals( object? obj ) => ReferenceEquals(this, obj) || ( obj is CacheEntry<TRecord, TID> other && Equals(other) );
    public override int GetHashCode() => Value.GetHashCode();


    public static bool operator ==( CacheEntry<TRecord, TID>? left, CacheEntry<TRecord, TID>? right ) => Equalizer<CacheEntry<TRecord, TID>>.Instance.Equals(left, right);
    public static bool operator !=( CacheEntry<TRecord, TID>? left, CacheEntry<TRecord, TID>? right ) => !Equalizer<CacheEntry<TRecord, TID>>.Instance.Equals(left, right);
    public static bool operator <( CacheEntry<TRecord, TID>?  left, CacheEntry<TRecord, TID>? right ) => Sorter<CacheEntry<TRecord, TID>>.Instance.Compare(left, right) < 0;
    public static bool operator >( CacheEntry<TRecord, TID>?  left, CacheEntry<TRecord, TID>? right ) => Sorter<CacheEntry<TRecord, TID>>.Instance.Compare(left, right) > 0;
    public static bool operator <=( CacheEntry<TRecord, TID>? left, CacheEntry<TRecord, TID>? right ) => Sorter<CacheEntry<TRecord, TID>>.Instance.Compare(left, right) <= 0;
    public static bool operator >=( CacheEntry<TRecord, TID>? left, CacheEntry<TRecord, TID>? right ) => Sorter<CacheEntry<TRecord, TID>>.Instance.Compare(left, right) >= 0;
}
