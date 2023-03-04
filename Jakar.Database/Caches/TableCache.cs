// Jakar.Extensions :: Jakar.Database
// 09/01/2022  6:40 PM

namespace Jakar.Database.Caches;


public sealed class TableCache<TRecord> : IHostedService, IReadOnlyCollection<TRecord>, IAsyncEnumerator<TRecord?> where TRecord : TableRecord<TRecord>
{
    private readonly ConcurrentDictionary<Guid, CacheEntry<TRecord>> _records = new();
    private readonly DbTable<TRecord>                                _table;
    private readonly ILogger<TableCache<TRecord>>                    _logger;
    private readonly List<Guid>                                      _keys = new();
    private readonly TableCacheOptions                               _options;
    private          int                                             _index = -1;
    public           IEnumerable<Guid>                               Changed => from entry in _records.Values where entry.HasChanged select entry.ID;
    public           int                                             Count   => _records.Count;


    public TRecord? Current
    {
        get
        {
            Debug.Assert( _index >= 0 && _keys.Count >= 0 && _index < _keys.Count );

            return TryGetValue( _keys[_index], out TRecord? value )
                       ? value
                       : default;
        }
    }

    public bool HasChanged => _records.Values.Any( x => x.HasChanged );


    public TRecord? this[ Guid key ] => TryGetValue( key, out TRecord? value )
                                            ? value
                                            : default;
    public IEnumerable<Guid>    Keys           => _records.Keys;
    public IEnumerable<TRecord> Records        => _records.Values.Select( x => x.Value );
    public IEnumerable<TRecord> RecordsChanged => from entry in _records.Values where entry.HasChanged select entry.Saved();
    public IEnumerable<TRecord> RecordsExpired => from entry in _records.Values where entry.HasExpired( _options.ExpireTime ) select entry.Value;


    public TableCache( DbTable<TRecord> table, ILogger<TableCache<TRecord>> logger, IOptions<TableCacheOptions> options )
    {
        _table   = table;
        _options = options.Value;
        _logger  = logger;
    }
    public async ValueTask DisposeAsync()
    {
        await _table.TryCall( Refresh );
        Clear();
    }


    public void Clear()
    {
        _records.Clear();
        _keys.Clear();
    }
    public bool Contains( Guid    key ) => _records.ContainsKey( key );
    public bool Contains( TRecord record ) => _records.ContainsKey( record.ID );


    public async ValueTask AddOrUpdate( IAsyncEnumerable<TRecord?> records )
    {
        await foreach ( TRecord? record in records )
        {
            if ( record is null ) { continue; }

            AddOrUpdate( record );
        }
    }
    public async ValueTask AddOrUpdate( IAsyncEnumerable<TRecord?> records, CancellationToken token )
    {
        await foreach ( TRecord? record in records.WithCancellation( token ) )
        {
            if ( record is null ) { continue; }

            AddOrUpdate( record );
        }
    }
    public void AddOrUpdate( IEnumerable<TRecord> records )
    {
        foreach ( TRecord record in records ) { AddOrUpdate( record ); }
    }
    public void AddOrUpdate( TRecord record )
    {
        if ( _keys.Any() ) { Reset(); }

        if ( _records.TryGetValue( record.ID, out CacheEntry<TRecord>? entry ) )
        {
            entry.Value = record;
            return;
        }

        _records[record.ID] = new CacheEntry<TRecord>( record );
    }


    public bool TryGetValue( Guid key, [NotNullWhen( true )] out TRecord? value )
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
    public bool TryRemove( Guid    key ) => _records.TryRemove( key, out _ );
    public bool TryRemove( Guid key, [NotNullWhen( true )] out TRecord? value )
    {
        if ( _keys.Any() ) { Reset(); }

        if ( _records.TryRemove( key, out CacheEntry<TRecord>? entry ) )
        {
            value = entry.Value;
            return true;
        }

        value = default;
        return false;
    }


    public void Reset()
    {
        var dictionary = new SortedDictionary<DateTimeOffset, Guid>( ValueSorter<DateTimeOffset>.Default );
        foreach ( CacheEntry<TRecord> value in _records.Values ) { dictionary.Add( value.DateCreated, value.ID ); }

        _keys.AddRange( dictionary.Values );
        _index = -1;
    }

    public async ValueTask<bool> MoveNextAsync()
    {
        await _table.TryCall( Refresh );


        if ( _keys.Count == 0 ) { Reset(); }

        return ++_index < Count;
    }
    private async ValueTask Refresh( DbConnection connection, DbTransaction transaction, CancellationToken token = default )
    {
        if ( _records.IsEmpty )
        {
            AddOrUpdate( await _table.All( connection, transaction, token ) );
            return;
        }


        if ( HasChanged )
        {
            await _table.Update( connection, transaction, RecordsChanged, token );
            TRecord[] changed = await _table.Get( connection, transaction, Changed, token );
            AddOrUpdate( changed );
        }
    }
    public async Task StartAsync( CancellationToken token )
    {
        using var timer = new PeriodicTimer( _options.RefreshTime );

        while ( token.ShouldContinue() )
        {
            try
            {
                await timer.WaitForNextTickAsync( token );
                await _table.TryCall( Refresh, token );
            }
            catch ( Exception e ) { _logger.LogCritical( e, "Cache Failure: [ {Table} ]", nameof(TableCache<TRecord>) ); }
        }
    }
    public async Task StopAsync( CancellationToken token ) => await _table.Update( RecordsChanged, token );


    public IEnumerator<TRecord> GetEnumerator() => Records.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
