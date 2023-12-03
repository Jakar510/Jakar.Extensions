// Jakar.Extensions :: Jakar.Database
// 09/02/2022  3:43 PM

namespace Jakar.Database.Caches;


public sealed class CacheEntry<TRecord> : ObservableClass, IRecordPair, IEquatable<TRecord>, IEquatable<CacheEntry<TRecord>>, IComparable<CacheEntry<TRecord>>, IComparable
    where TRecord : class, ITableRecord<TRecord>, IDbReaderMapping<TRecord>, IMsJsonContext<TRecord>
{
    private readonly DateTimeOffset _lastTime = DateTimeOffset.UtcNow;
    private          string         _json     = string.Empty;
    private          UInt128        _hash;
    public           bool           HasChanged => Spans.Hash128( _json ) != _hash;


    public DateTimeOffset    DateCreated  { get; private set; }
    public DateTimeOffset?   LastModified { get; private set; }
    Guid IUniqueID<Guid>.    ID           => ID.Value;
    public RecordID<TRecord> ID           { get; init; }


    public TRecord Value
    {
        get => TryGetValue() ?? throw new NullReferenceException( nameof(_json) );
        set
        {
            DateCreated  = value.DateCreated;
            LastModified = value.LastModified;
            _json        = JsonSerializer_.Serialize( value, TRecord.JsonOptions( false ) );
            _hash        = Spans.Hash128( _json );
        }
    }
    private TRecord? TryGetValue()
    {
        if ( string.IsNullOrWhiteSpace( _json ) ) { return default; }

        return JsonSerializer_.Deserialize( _json, TRecord.JsonTypeInfo() );
    }


    public CacheEntry( TRecord value )
    {
        ID    = value.ID;
        Value = value;
    }
    public   RecordPair<TRecord> ToPair()                       => new(ID, DateCreated);
    internal TRecord             Saved()                        => Value;
    public   bool                HasExpired( in TimeSpan time ) => DateTimeOffset.UtcNow - _lastTime >= time;


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

        return string.Compare( _json, _json, StringComparison.Ordinal );
    }
    public bool Equals( CacheEntry<TRecord>? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return string.Equals( _json, other._json, StringComparison.Ordinal );
    }
    public          bool Equals( TRecord? other ) => Value.Equals( other );
    public override bool Equals( object?  obj )   => ReferenceEquals( this, obj ) || obj is CacheEntry<TRecord> other && Equals( other );
    public override int  GetHashCode()            => _hash.GetHashCode();


    public static bool operator ==( CacheEntry<TRecord>? left, CacheEntry<TRecord>? right ) => Equalizer<CacheEntry<TRecord>>.Default.Equals( left, right );
    public static bool operator >( CacheEntry<TRecord>?  left, CacheEntry<TRecord>? right ) => Sorter<CacheEntry<TRecord>>.Default.Compare( left, right ) > 0;
    public static bool operator >=( CacheEntry<TRecord>? left, CacheEntry<TRecord>? right ) => Sorter<CacheEntry<TRecord>>.Default.Compare( left, right ) >= 0;
    public static bool operator !=( CacheEntry<TRecord>? left, CacheEntry<TRecord>? right ) => !Equalizer<CacheEntry<TRecord>>.Default.Equals( left, right );
    public static bool operator <( CacheEntry<TRecord>?  left, CacheEntry<TRecord>? right ) => Sorter<CacheEntry<TRecord>>.Default.Compare( left, right ) < 0;
    public static bool operator <=( CacheEntry<TRecord>? left, CacheEntry<TRecord>? right ) => Sorter<CacheEntry<TRecord>>.Default.Compare( left, right ) <= 0;
}
