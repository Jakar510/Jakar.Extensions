﻿// Jakar.Extensions :: Jakar.Database
// 09/02/2022  3:43 PM

namespace Jakar.Database.Caches;


public sealed class CacheEntry<TRecord>( RecordID<TRecord> id ) : ObservableClass, IRecordPair, IEquatable<TRecord>, IEquatable<CacheEntry<TRecord>>, IComparable<CacheEntry<TRecord>>, IComparable
    where TRecord : class, ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    private DateTimeOffset _lastTime = DateTimeOffset.UtcNow;
    private string         _json     = string.Empty;
    private UInt128        _hash;
    public  DateTimeOffset DateCreated { get; private set; }

    // private WeakReference<TRecord>? _current;

    public bool              HasChanged   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _hash == Spans.Hash128( _json ); }
    Guid IUniqueID<Guid>.    ID           => ID.Value;
    public RecordID<TRecord> ID           { get; } = id;
    public DateTimeOffset?   LastModified { get; private set; }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public async ValueTask<TRecord?> TryGetValue( DbTable<TRecord> table, TableCacheOptions options, CancellationToken token )
    {
        TRecord? record = TryGetValue( options );
        if ( record is not null ) { return record; }

        record = await table.Get( ID, token );
        if ( record is null ) { return default; }

        SetValue( record );
        return record;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public TRecord? TryGetValue( in TableCacheOptions options )
    {
        lock (this)
        {
            string json = _json;
            if ( string.IsNullOrWhiteSpace( json ) || HasExpired( options.ExpireTime ) ) { return default; }

            var record = json.FromJson<TRecord>();
            _lastTime = DateTimeOffset.UtcNow;
            return record;
        }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public void SetValue( TRecord record )
    {
        if ( ID != record.ID ) { throw new ArgumentException( "ID mismatch" ); }

        lock (this)
        {
            DateCreated  = record.DateCreated;
            LastModified = record.LastModified;
            string json = record.ToJson();
            Interlocked.Exchange( ref _json, json );
            _hash     = Spans.Hash128( json );
            _lastTime = DateTimeOffset.UtcNow;
        }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public RecordPair<TRecord> ToPair()                       => new(ID, DateCreated);
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool                HasExpired( in TimeSpan time ) => DateTimeOffset.UtcNow - _lastTime >= time;


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

        return string.Compare( _json, other._json, StringComparison.Ordinal );
    }
    public bool Equals( CacheEntry<TRecord>? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return string.Equals( _json, other._json, StringComparison.Ordinal );
    }
    public          bool Equals( TRecord? other ) => _json.FromJson<TRecord>().Equals( other );
    public override bool Equals( object?  obj )   => ReferenceEquals( this, obj ) || obj is CacheEntry<TRecord> other && Equals( other );
    public override int  GetHashCode()            => ID.GetHashCode();


    public static bool operator ==( CacheEntry<TRecord>? left, CacheEntry<TRecord>? right ) => Equalizer<CacheEntry<TRecord>>.Default.Equals( left, right );
    public static bool operator >( CacheEntry<TRecord>?  left, CacheEntry<TRecord>? right ) => Sorter<CacheEntry<TRecord>>.Default.Compare( left, right ) > 0;
    public static bool operator >=( CacheEntry<TRecord>? left, CacheEntry<TRecord>? right ) => Sorter<CacheEntry<TRecord>>.Default.Compare( left, right ) >= 0;
    public static bool operator !=( CacheEntry<TRecord>? left, CacheEntry<TRecord>? right ) => !Equalizer<CacheEntry<TRecord>>.Default.Equals( left, right );
    public static bool operator <( CacheEntry<TRecord>?  left, CacheEntry<TRecord>? right ) => Sorter<CacheEntry<TRecord>>.Default.Compare( left, right ) < 0;
    public static bool operator <=( CacheEntry<TRecord>? left, CacheEntry<TRecord>? right ) => Sorter<CacheEntry<TRecord>>.Default.Compare( left, right ) <= 0;
}
