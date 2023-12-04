// Jakar.Extensions :: Jakar.Database
// 09/01/2022  6:40 PM

namespace Jakar.Database.Caches;


public sealed class TableCache<TRecord> : IHostedService, IAsyncEnumerable<TRecord?>
    where TRecord : class, ITableRecord<TRecord>, IDbReaderMapping<TRecord>, IMsJsonContext<TRecord>
{
    private readonly ConcurrentDictionary<RecordID<TRecord>, CacheEntry<TRecord>> _records = new(RecordID<TRecord>.Equalizer);
    private readonly DbTable<TRecord>                                             _table;
    private readonly ILogger<TableCache<TRecord>>                                 _logger;
    private readonly TableCacheOptions                                            _options;


    public bool HasChanged
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => _records.Values.Any( static x => x.HasChanged );
    }
    public IEnumerable<RecordID<TRecord>> Changed => GetChangedIDs();
    public IEnumerable<RecordID<TRecord>> Keys
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => _records.Keys;
    }
    public IEnumerable<TRecord> RecordsChanged
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => GetChangedRecords();
    }
    public IEnumerable<TRecord> RecordsExpired
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => GetExpiredRecords();
    }
    public int Count
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => _records.Count;
    }
    public TRecord? this[ RecordPair<TRecord> id ]
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => this[id.ID];
    }
    public TRecord? this[ RecordID<TRecord> id ]
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
        get => TryGetValue( id, out TRecord? value )
                   ? value
                   : default;
    }


    // ReSharper disable once ConvertToPrimaryConstructor
    public TableCache( DbTable<TRecord> table, ILogger<TableCache<TRecord>> logger, IOptions<TableCacheOptions> options )
    {
        _table   = table;
        _logger  = logger;
        _options = options.Value;
    }
    public async ValueTask DisposeAsync()
    {
        await RefreshAsync( CancellationToken.None );
        Clear();
    }


    private IEnumerable<TRecord> GetChangedRecords()
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( CacheEntry<TRecord> entry in _records.Values )
        {
            if ( entry.HasChanged is false ) { continue; }

            var record = entry.TryGetValue( _options );
            if ( record is not null ) { yield return record; }
        }
    }
    private IEnumerable<TRecord> GetExpiredRecords()
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( CacheEntry<TRecord> entry in _records.Values )
        {
            var record = entry.TryGetValue( _options );
            if ( record is not null ) { yield return record; }
        }
    }
    private IEnumerable<RecordID<TRecord>> GetChangedIDs()
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( CacheEntry<TRecord> entry in _records.Values )
        {
            if ( entry.HasChanged ) { yield return entry.ID; }
        }
    }
    public void Clear()                                => _records.Clear();
    public bool Contains( RecordPair<TRecord> id )     => _records.ContainsKey( id.ID );
    public bool Contains( RecordID<TRecord>   id )     => _records.ContainsKey( id );
    public bool Contains( TRecord             record ) => _records.ContainsKey( record.ID );


    public void AddOrUpdate( TRecord record )
    {
        if ( _records.TryGetValue( record.ID, out CacheEntry<TRecord>? entry ) is false ) { _records[record.ID] = entry = new CacheEntry<TRecord>( record.ID ); }

        entry.SetValue( record );
    }


    public ValueTask<TRecord?> TryGetValue( RecordPair<TRecord> pair, CancellationToken token ) => TryGetValue( pair.ID, token );
    public async ValueTask<TRecord?> TryGetValue( RecordID<TRecord> id, CancellationToken token )
    {
        if ( _records.TryGetValue( id, out CacheEntry<TRecord>? entry ) is false ) { return default; }

        TRecord? record = await entry.TryGetValue( _table, _options, token );
        if ( record is not null ) { return record; }

        _records.TryRemove( id, out _ );
        return default;
    }
    public bool TryGetValue( RecordPair<TRecord> id, [ NotNullWhen( true ) ] out TRecord? record ) => TryGetValue( id.ID, out record );
    public bool TryGetValue( RecordID<TRecord> id, [ NotNullWhen( true ) ] out TRecord? record )
    {
        if ( _records.TryGetValue( id, out CacheEntry<TRecord>? entry ) )
        {
            record = entry.TryGetValue( _options );
            return record is not null;
        }

        record = default;
        return false;
    }


    public ValueTask<TRecord?> TryRemove( RecordPair<TRecord> id, CancellationToken token = default ) => TryRemove( id.ID, token );
    public async ValueTask<TRecord?> TryRemove( RecordID<TRecord> id, CancellationToken token = default )
    {
        if ( _records.TryRemove( id, out CacheEntry<TRecord>? entry ) ) { return await entry.TryGetValue( _table, _options, token ); }

        return default;
    }


    public async ValueTask RefreshAsync( CancellationToken token ) => await _table.TryCall( RefreshAsync, token );
    private async ValueTask RefreshAsync( DbConnection connection, DbTransaction transaction, CancellationToken token = default )
    {
        if ( _records.IsEmpty )
        {
            foreach ( RecordID<TRecord> id in _records.Keys.ToArray( _records.Count ) )
            {
                TRecord? record = await _table.Get( connection, transaction, id, token );
                if ( record is not null ) { AddOrUpdate( record ); }
            }

            return;
        }

        if ( HasChanged )
        {
            await _table.Update( connection, transaction, RecordsChanged, token );
            await foreach ( TRecord record in _table.Get( connection, transaction, Changed, token ) ) { AddOrUpdate( record ); }
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
                await RefreshAsync( token );
            }
            catch ( Exception e ) { _logger.LogCritical( e, "Cache Failure: [ {Table} ]", nameof(TableCache<TRecord>) ); }
        }
    }
    public async Task StopAsync( CancellationToken token ) => await _table.Update( RecordsChanged, token );


    IAsyncEnumerator<TRecord?> IAsyncEnumerable<TRecord?>.GetAsyncEnumerator( CancellationToken token )           => GetAsyncEnumerator( token );
    public AsyncEnumerator                                GetAsyncEnumerator( CancellationToken token = default ) => new(this, token);



    public sealed class AsyncEnumerator : IAsyncEnumerable<TRecord>, IAsyncEnumerator<TRecord>
    {
        private const    int                       START_INDEX = -1;
        private readonly List<RecordPair<TRecord>> _cache      = new();
        private readonly TableCache<TRecord>       _tableCache;
        private          bool                      _isDisposed;
        private          CancellationToken         _token;
        private          int                       _index = START_INDEX;
        private          RecordPair<TRecord>       _pair;
        private          TRecord?                  _current;
        internal         bool                      ShouldContinue => _token.ShouldContinue() && _index < _cache.Count;
        public           TRecord                   Current        => _current ?? throw new NullReferenceException( nameof(_current) );


        // ReSharper disable once ConvertToPrimaryConstructor
        public AsyncEnumerator( TableCache<TRecord> tableCache, CancellationToken token = default )
        {
            _tableCache = tableCache;
            _token      = token;
        }
        public ValueTask DisposeAsync()
        {
            Reset();
            _isDisposed = true;
            return default;
        }


        private static bool MoveNext( ref int index, in ReadOnlySpan<RecordPair<TRecord>> span, out RecordPair<TRecord> current )
        {
            int i = Interlocked.Add( ref index, 1 );

            current = i < span.Length
                          ? span[i]
                          : default;

            return i < span.Length;
        }
        public async ValueTask<bool> MoveNextAsync()
        {
            if ( _isDisposed ) { throw new ObjectDisposedException( nameof(AsyncEnumerator) ); }

            if ( _cache.IsEmpty() )
            {
                Reset();
                _cache.EnsureCapacity( _tableCache.Count );
                _cache.AddRange( _tableCache._records.Values.Select( static x => x.ToPair() ) );
                _cache.Sort( RecordPair<TRecord>.Sorter );
            }

            if ( MoveNext( ref _index, CollectionsMarshal.AsSpan( _cache ), out _pair ) )
            {
                _current = await _tableCache.TryGetValue( _pair, _token );
                return _current is not null;
            }

            Reset();
            return false;
        }
        IAsyncEnumerator<TRecord> IAsyncEnumerable<TRecord>.GetAsyncEnumerator( CancellationToken token ) => GetAsyncEnumerator( token );
        public AsyncEnumerator GetAsyncEnumerator( CancellationToken token = default )
        {
            Reset();
            _token = token;
            return this;
        }
        public void Reset()
        {
            if ( _isDisposed ) { throw new ObjectDisposedException( nameof(AsyncEnumerator) ); }

            _cache.Clear();
            _pair = default;
            Interlocked.Exchange( ref _index,   START_INDEX );
            Interlocked.Exchange( ref _current, null );
        }


        public override string ToString() => $"AsyncEnumerator<{typeof(TRecord).Name}>( {nameof(_index)} : {_index}, {nameof(ShouldContinue)} : {ShouldContinue} )";
    }
}
