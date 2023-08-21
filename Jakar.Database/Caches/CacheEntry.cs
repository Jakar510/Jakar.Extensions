// Jakar.Extensions :: Jakar.Database
// 09/02/2022  3:43 PM

namespace Jakar.Database.Caches;


public sealed class CacheEntry<TRecord> : ObservableClass, IRecordPair, IEquatable<TRecord>, IComparable<TRecord>, IEquatable<CacheEntry<TRecord>>, IComparable<CacheEntry<TRecord>>, IComparable where TRecord : TableRecord<TRecord>

{
    private DateTimeOffset _lastTime;
    private int            _hash;
    private TRecord?       _value;
    public  Guid?          CreatedBy   => _value?.CreatedBy?.Value;
    public  DateTimeOffset DateCreated => _value?.DateCreated ?? default;


    public bool              HasChanged   => Value.GetHashCode() != _hash;
    public RecordID<TRecord> ID           { get; init; }
    public DateTimeOffset?   LastModified => _value?.LastModified;
    Guid IUniqueID<Guid>.ID => ID.Value;


    public TRecord Value
    {
        get => _value ?? throw new NullReferenceException( nameof(_value) );
        set
        {
            if ( !SetProperty( ref _value, value ) ) { return; }

            _hash     = value.GetHashCode();
            _lastTime = DateTimeOffset.UtcNow;
        }
    }


    public CacheEntry( TRecord value )
    {
        Value = value;
        ID    = value.ID;
    }
    internal TRecord Saved()
    {
        _hash = _value?.GetHashCode() ?? 0;
        return Value;
    }
    public bool HasExpired( in TimeSpan time ) => DateTimeOffset.UtcNow - _lastTime >= time;


    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        return other is CacheEntry<TRecord> entry
                   ? CompareTo( entry )
                   : throw new ExpectedValueTypeException( other, typeof(CacheEntry<TRecord>) );
    }
    public int CompareTo( CacheEntry<TRecord>? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        return Value.CompareTo( other.Value );
    }
    public int CompareTo( TRecord? other ) => Value.CompareTo( other );
    public bool Equals( CacheEntry<TRecord>? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return Value.Equals( other.Value );
    }
    public bool Equals( TRecord?         other ) => Value.Equals( other );
    public override bool Equals( object? obj ) => ReferenceEquals( this, obj ) || obj is CacheEntry<TRecord> other && Equals( other );
    public override int GetHashCode() => Value.GetHashCode();


    public static bool operator ==( CacheEntry<TRecord>? left, CacheEntry<TRecord>? right ) => Equalizer<CacheEntry<TRecord>>.Default.Equals( left, right );
    public static bool operator >( CacheEntry<TRecord>?  left, CacheEntry<TRecord>? right ) => Sorter<CacheEntry<TRecord>>.Default.Compare( left, right ) > 0;
    public static bool operator >=( CacheEntry<TRecord>? left, CacheEntry<TRecord>? right ) => Sorter<CacheEntry<TRecord>>.Default.Compare( left, right ) >= 0;
    public static bool operator !=( CacheEntry<TRecord>? left, CacheEntry<TRecord>? right ) => !Equalizer<CacheEntry<TRecord>>.Default.Equals( left, right );
    public static bool operator <( CacheEntry<TRecord>?  left, CacheEntry<TRecord>? right ) => Sorter<CacheEntry<TRecord>>.Default.Compare( left, right ) < 0;
    public static bool operator <=( CacheEntry<TRecord>? left, CacheEntry<TRecord>? right ) => Sorter<CacheEntry<TRecord>>.Default.Compare( left, right ) <= 0;
}
