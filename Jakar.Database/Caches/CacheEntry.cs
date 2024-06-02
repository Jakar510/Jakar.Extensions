// Jakar.Extensions :: Jakar.Database
// 09/02/2022  3:43 PM

namespace Jakar.Database.Caches;


public sealed class CacheEntry<TRecord>( RecordID<TRecord> id ) : ObservableClass, IRecordPair, IEquatable<TRecord>, IEquatable<CacheEntry<TRecord>>, IComparable<CacheEntry<TRecord>>, IComparable
    where TRecord : class, ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    private DateTimeOffset? _lastUpdated;
    private string          _json = string.Empty;
    private UInt128         _hash;

    // private WeakReference<TRecord>? _current;

    public static Equalizer<CacheEntry<TRecord>> Equalizer    { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => Equalizer<CacheEntry<TRecord>>.Default; }
    public static Sorter<CacheEntry<TRecord>>    Sorter       { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => Sorter<CacheEntry<TRecord>>.Default; }
    public        DateTimeOffset                 DateCreated  { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get; private set; }
    public        bool                           HasValueSet  { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => _lastUpdated.HasValue; }
    public        bool                           HasChanged   { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => _hash == GetHash( _json ); }
    Guid IUniqueID<Guid>.                        ID           => ID.Value;
    public RecordID<TRecord>                     ID           { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get; } = id;
    public DateTimeOffset?                       LastModified { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get; private set; }


    [Pure]
    public async ValueTask<TRecord?> TryGetValue( Activity? activity, DbTable<TRecord> table, TableCacheOptions options, CancellationToken token )
    {
        TRecord? record = TryGetValue( options );
        if ( record is not null ) { return record; }

        record = await table.Get( activity, ID, token );
        if ( record is null ) { return default; }

        SetValue( in record );
        return record;
    }


    [Pure]
    public TRecord? TryGetValue( TableCacheOptions options )
    {
        lock (this)
        {
            string json = _json;
            if ( string.IsNullOrWhiteSpace( json ) || _lastUpdated.HasExpired( options.ExpireTime ) ) { return null; }

            return json.FromJson<TRecord>();
        }
    }


    public void SetValue( ref readonly TRecord record )
    {
        if ( ID != record.ID ) { throw new ArgumentException( "ID mismatch" ); }

        lock (this)
        {
            DateCreated  = record.DateCreated;
            LastModified = record.LastModified;
            _json        = record.ToJson();
            _hash        = GetHash( _json );
            _lastUpdated = DateTimeOffset.UtcNow;
        }
    }


    [Pure] public bool HasChangedOrExpired( in TimeSpan lifeSpan ) => _lastUpdated.HasExpired( lifeSpan ) || _hash != GetHash( _json );


    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public static UInt128             GetHash( in ReadOnlySpan<char> value ) => Spans.Hash128( value );
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public        RecordPair<TRecord> ToPair()                               => new(ID, DateCreated);


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

        int lastModifiedComparison = Nullable.Compare( LastModified, other.LastModified );
        if ( lastModifiedComparison != 0 ) { return lastModifiedComparison; }

        return string.Compare( _json, other._json, StringComparison.Ordinal );
    }
    public bool Equals( CacheEntry<TRecord>? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return ID.Equals( other.ID ) && string.Equals( _json, other._json, StringComparison.Ordinal );
    }
    public          bool Equals( TRecord? other ) => _json.FromJson<TRecord>().Equals( other );
    public override bool Equals( object?  obj )   => ReferenceEquals( this, obj ) || obj is CacheEntry<TRecord> other && Equals( other );
    public override int  GetHashCode()            => ID.GetHashCode();


    public static bool operator ==( CacheEntry<TRecord>? left, CacheEntry<TRecord>? right ) => Equalizer.Equals( left, right );
    public static bool operator >( CacheEntry<TRecord>?  left, CacheEntry<TRecord>? right ) => Sorter.Compare( left, right ) > 0;
    public static bool operator >=( CacheEntry<TRecord>? left, CacheEntry<TRecord>? right ) => Sorter.Compare( left, right ) >= 0;
    public static bool operator !=( CacheEntry<TRecord>? left, CacheEntry<TRecord>? right ) => !Equalizer.Equals( left, right );
    public static bool operator <( CacheEntry<TRecord>?  left, CacheEntry<TRecord>? right ) => Sorter.Compare( left, right ) < 0;
    public static bool operator <=( CacheEntry<TRecord>? left, CacheEntry<TRecord>? right ) => Sorter.Compare( left, right ) <= 0;
}
