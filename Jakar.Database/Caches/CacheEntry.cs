// Jakar.Extensions :: Jakar.Database
// 09/02/2022  3:43 PM

namespace Jakar.Database;


public sealed class CacheEntry<TRecord> : ObservableClass, IEquatable<TRecord>, IComparable<TRecord>, IEquatable<CacheEntry<TRecord>>, IComparable<CacheEntry<TRecord>>, IComparable where TRecord : TableRecord<TRecord>

{
    private DateTimeOffset _lastTime;
    private int            _hash;
    private TRecord?       _value;
    public  bool           HasChanged => Value.GetHashCode() != _hash;
    public  long           ID         => Value.ID;


    public TRecord Value
    {
        get => _value ?? throw new NullReferenceException( nameof(_value) );
        set
        {
            if (!SetProperty( ref _value, value )) { return; }

            _hash     = value.GetHashCode();
            _lastTime = DateTimeOffset.Now;
        }
    }


    public CacheEntry( TRecord                                   value ) => Value = value;
    public static implicit operator CacheEntry<TRecord>( TRecord value ) => new(value);


    public static bool operator ==( CacheEntry<TRecord>? left, CacheEntry<TRecord>? right ) => Equalizer<CacheEntry<TRecord>>.Instance.Equals( left, right );
    public static bool operator !=( CacheEntry<TRecord>? left, CacheEntry<TRecord>? right ) => !Equalizer<CacheEntry<TRecord>>.Instance.Equals( left, right );
    public static bool operator <( CacheEntry<TRecord>?  left, CacheEntry<TRecord>? right ) => Sorter<CacheEntry<TRecord>>.Instance.Compare( left, right ) < 0;
    public static bool operator >( CacheEntry<TRecord>?  left, CacheEntry<TRecord>? right ) => Sorter<CacheEntry<TRecord>>.Instance.Compare( left, right ) > 0;
    public static bool operator <=( CacheEntry<TRecord>? left, CacheEntry<TRecord>? right ) => Sorter<CacheEntry<TRecord>>.Instance.Compare( left, right ) <= 0;
    public static bool operator >=( CacheEntry<TRecord>? left, CacheEntry<TRecord>? right ) => Sorter<CacheEntry<TRecord>>.Instance.Compare( left, right ) >= 0;


    public bool HasExpired( in TimeSpan  time ) => DateTimeOffset.Now - _lastTime >= time;
    public override bool Equals( object? obj ) => ReferenceEquals( this, obj ) || obj is CacheEntry<TRecord> other && Equals( other );
    public override int GetHashCode() => Value.GetHashCode();
    public int CompareTo( object? other )
    {
        if (other is null) { return 1; }

        if (ReferenceEquals( this, other )) { return 0; }

        return other is CacheEntry<TRecord> entry
                   ? CompareTo( entry )
                   : throw new ArgumentException( $"Object must be of type {nameof(CacheEntry<TRecord>)}" );
    }
    public int CompareTo( CacheEntry<TRecord>? other )
    {
        if (other is null) { return 1; }

        if (ReferenceEquals( this, other )) { return 0; }

        return Value.CompareTo( other.Value );
    }
    public int CompareTo( TRecord? other ) => Value.CompareTo( other );
    public bool Equals( CacheEntry<TRecord>? other )
    {
        if (other is null) { return false; }

        if (ReferenceEquals( this, other )) { return true; }

        return Value.Equals( other.Value );
    }


    public bool Equals( TRecord? other ) => Value.Equals( other );
}
