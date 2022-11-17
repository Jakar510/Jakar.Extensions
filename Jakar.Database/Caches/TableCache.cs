// Jakar.Extensions :: Jakar.Database
// 09/01/2022  6:40 PM

namespace Jakar.Database.Caches;


public sealed class TableCache<TRecord> : Service, IHostedService, IReadOnlyCollection<TRecord>, IAsyncEnumerator<TRecord?> where TRecord : TableRecord<TRecord>
{
    private readonly ConcurrentDictionary<long, CacheEntry<TRecord>> _records = new();
    private readonly DbTableBase<TRecord>                            _table;
    private readonly ILogger                                         _logger;
    private readonly KeyGenerator<CacheEntry<TRecord>>               _generator;
    private readonly TimeSpan                                        _expireTime = TimeSpan.FromMinutes( 1 );
    private readonly TimeSpan                                        _refreshTime;


    public bool                 HasChanged     => _records.Values.Any( x => x.HasChanged );
    public IEnumerable<long>    Changed        => from entry in _records.Values where entry.HasChanged select entry.ID;
    public IEnumerable<long>    Keys           => _records.Keys;
    public IEnumerable<TRecord> Records        => _records.Values.Select( x => x.Value );
    public IEnumerable<TRecord> RecordsChanged => from entry in _records.Values where entry.HasChanged select entry.Saved();
    public IEnumerable<TRecord> RecordsExpired => from entry in _records.Values where entry.HasExpired( _expireTime ) select entry.Value;
    public int                  Count          => _records.Count;
    public TRecord?             Current        => this[_generator.Current];


    public TRecord? this[ long key ] => TryGetValue( key, out TRecord? value )
                                            ? value
                                            : default;


    public TableCache( DbTableBase<TRecord> table, IOptions<TableCacheOptions> options ) : this( table, options.Value ) { }
    public TableCache( DbTableBase<TRecord> table, TableCacheOptions           options ) : this( table, options.Factory, options.RefreshTime ) { }
    public TableCache( DbTableBase<TRecord> table, ILoggerFactory factory, TimeSpan refreshTime ) : base()
    {
        _table       = table;
        _refreshTime = refreshTime;
        _logger      = factory.CreateLogger( GetType() );
        _generator   = new KeyGenerator<CacheEntry<TRecord>>( _records );
    }
    public bool Contains( long    key ) => _records.Values.Any( x => x.ID.Equals( key ) );
    public bool Contains( TRecord key ) => _records.Values.Any( x => x.Equals( key ) );


    public bool TryGetValue( long key, [NotNullWhen( true )] out TRecord? value )
    {
        if ( _records.TryGetValue( key, out CacheEntry<TRecord>? entry ) )
        {
            value = entry.Value;
            return true;
        }

        value = default;
        return false;
    }


    public bool TryRemove( TRecord pair ) => _records.TryRemove( pair.ID, out _ );
    public bool TryRemove( TRecord pair, [NotNullWhen( true )] out TRecord? value ) => TryRemove( pair.ID, out value );
    public bool TryRemove( long    key ) => _records.TryRemove( key, out _ );
    public bool TryRemove( long key, [NotNullWhen( true )] out TRecord? value )
    {
        if ( _records.TryRemove( key, out CacheEntry<TRecord>? entry ) )
        {
            value = entry.Value;
            return true;
        }

        value = default;
        return false;
    }


    public async ValueTask AddOrUpdate( IAsyncEnumerable<TRecord?> records, CancellationToken token = default )
    {
        await foreach ( TRecord? record in records.WithCancellation( token ) )
        {
            if ( record is null ) { continue; }

            AddOrUpdate( record );
        }
    }
    private async ValueTask Refresh( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        if ( HasChanged )
        {
            await _table.Update( connection, transaction, RecordsChanged, token );
            TRecord[] changed = await _table.Get( connection, transaction, Changed, token );
            AddOrUpdate( changed );
            return;
        }


        TRecord[] records = await _table.All( connection, transaction, token );
        _records.Clear();
        AddOrUpdate( records );
    }
    public void AddOrUpdate( IEnumerable<TRecord> records )
    {
        foreach ( TRecord? record in records ) { AddOrUpdate( record ); }
    }
    public void AddOrUpdate( TRecord record )
    {
        if ( Contains( record.ID ) )
        {
            _records[record.ID]
               .Value = record;
        }
        else { _records.TryAdd( record.ID, record ); }
    }


    public void Clear() => _records.Clear();
    protected override void Dispose( bool disposing )
    {
        _generator.Dispose();
        _records.Clear();
        Clear();
    }
    public void Reset() => _generator.Reset();
    public override ValueTask DisposeAsync()
    {
        _records.Clear();
        _generator.Dispose();
        return default;
    }


    public async ValueTask<bool> MoveNextAsync()
    {
        await _table.Call( Refresh );
        return _generator.MoveNext();
    }


    public async Task StartAsync( CancellationToken token )
    {
        using var timer = new PeriodicTimer( _refreshTime );

        while ( token.ShouldContinue() )
        {
            try
            {
                await timer.WaitForNextTickAsync( token );
                await _table.Call( Refresh, token );
            }
            catch ( Exception e ) { _logger.LogCritical( e, default, Array.Empty<object?>() ); }
        }
    }
    public async Task StopAsync( CancellationToken token ) => await _table.Update( RecordsChanged, token );


    public IEnumerator<TRecord> GetEnumerator() => Records.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
