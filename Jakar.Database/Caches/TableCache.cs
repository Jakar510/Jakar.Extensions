// Jakar.Extensions :: Jakar.Database
// 09/01/2022  6:40 PM

using ZXing.Aztec.Internal;



namespace Jakar.Database.Caches;


public sealed class TableCache<TRecord> : IHostedService, IReadOnlyCollection<TRecord>, IAsyncEnumerable<TRecord?>
    where TRecord : class, ITableRecord<TRecord>, IDbReaderMapping<TRecord>, IMsJsonContext<TRecord>
{
    private readonly DbTable<TRecord>                                    _table;
    private readonly ILogger<TableCache<TRecord>>                        _logger;
    private readonly ConcurrentDictionary<Guid, CacheEntry<TRecord>>     _records = new();
    private readonly ConcurrentObservableCollection<RecordPair<TRecord>> _keys    = new();
    private readonly TableCacheOptions                                   _options;


    public IEnumerable<RecordID<TRecord>> Changed    => GetChanged();
    public int                            Count      => _records.Count;
    public TRecord?                       Current    { get; private set; }
    public bool                           HasChanged => _records.Values.Any( static x => x.HasChanged );


    public TRecord? this[ RecordPair<TRecord> key ] => this[key.ID.Value];
    public TRecord? this[ RecordID<TRecord>   key ] => this[key.Value];
    public TRecord? this[ Guid key ] => TryGetValue( key, out TRecord? value )
                                            ? value
                                            : default;
    public IEnumerable<Guid>    Keys           => _records.Keys;
    public IEnumerable<TRecord> Records        => _records.Values.Select( x => x.Value );
    public IEnumerable<TRecord> RecordsChanged => from entry in _records.Values where entry.HasChanged select entry.Value;
    public IEnumerable<TRecord> RecordsExpired => from entry in _records.Values where entry.HasExpired( _options.ExpireTime ) select entry.Value;


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


    private IEnumerable<RecordID<TRecord>> GetChanged()
    {
        // from entry in _records.Values where entry.HasChanged select entry.ID;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( CacheEntry<TRecord> entry in _records.Values )
        {
            if ( entry.HasChanged ) { yield return entry.ID; }
        }
    }
    public void Clear()
    {
        _keys.Clear();
        _records.Clear();
    }
    public bool Contains( RecordPair<TRecord> key )    => _records.ContainsKey( key.ID.Value );
    public bool Contains( RecordID<TRecord>   key )    => _records.ContainsKey( key.Value );
    public bool Contains( TRecord             record ) => _records.ContainsKey( record.ID.Value );


    public async ValueTask AddOrUpdate( TRecord record, CancellationToken token = default )
    {
        await ResetKeysAsync( token );

        if ( _records.TryGetValue( record.ID.Value, out CacheEntry<TRecord>? entry ) ) { entry.Value = record; }
        else { _records[record.ID.Value]                                                             = new CacheEntry<TRecord>( record ); }
    }


    public bool TryGetValue( Guid key, [ NotNullWhen( true ) ] out TRecord? value )
    {
        if ( _records.TryGetValue( key, out CacheEntry<TRecord>? entry ) )
        {
            value = entry.Value;
            return true;
        }

        value = default;
        return false;
    }


    public async ValueTask<TRecord?> TryRemove( RecordPair<TRecord> key, CancellationToken token = default )
    {
        await RefreshAsync( token );

        if ( _records.TryRemove( key.ID.Value, out CacheEntry<TRecord>? entry ) ) { return entry.Value; }

        return default;
    }


    public async ValueTask ResetKeysAsync( CancellationToken token = default )
    {
        await _keys.ClearAsync( token );
        IEnumerable<RecordPair<TRecord>> pairs = _records.Values.Select( static x => x.ToPair() );
        await _keys.AddAsync( pairs, token );
    }
    public async ValueTask RefreshAsync( CancellationToken token ) => await _table.TryCall( RefreshAsync, token );
    private async ValueTask RefreshAsync( DbConnection connection, DbTransaction transaction, CancellationToken token = default )
    {
        if ( _records.IsEmpty )
        {
            await foreach ( RecordPair<TRecord> pair in _keys.AsyncValues.GetAsyncEnumerator( token ) )
            {
                var record = await _table.Get( connection, transaction, pair.ID, token );

                if ( record is null ) { await _keys.RemoveAsync( pair, token ); }
                else { await AddOrUpdate( record, token ); }
            }

            return;
        }

        if ( HasChanged )
        {
            await _table.Update( connection, transaction, RecordsChanged, token );
            await foreach ( TRecord record in _table.Get( connection, transaction, Changed, token ) ) { await AddOrUpdate( record, token ); }
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
                await _table.TryCall( RefreshAsync, token );
            }
            catch ( Exception e ) { _logger.LogCritical( e, "Cache Failure: [ {Table} ]", nameof(TableCache<TRecord>) ); }
        }
    }
    public async Task StopAsync( CancellationToken token ) => await _table.Update( RecordsChanged, token );


    IAsyncEnumerator<TRecord?> IAsyncEnumerable<TRecord?>.GetAsyncEnumerator( CancellationToken token )           => GetAsyncEnumerator( token );
    public AsyncEnumerator                                GetAsyncEnumerator( CancellationToken token = default ) => new(this, token);
    public IEnumerator<TRecord>                           GetEnumerator()                                         => Records.GetEnumerator();
    IEnumerator IEnumerable.                              GetEnumerator()                                         => GetEnumerator();



    public sealed class AsyncEnumerator : IAsyncEnumerable<TRecord>, IAsyncEnumerator<TRecord>
    {
        private const    int                                 START_INDEX = -1;
        private readonly TableCache<TRecord>                 _tableCache;
        private          bool                                _isDisposed;
        private          CancellationToken                   _token;
        private          int                                 _index = START_INDEX;
        private          RecordPair<TRecord>?                _pair;
        private          TRecord?                            _current;
        private          ReadOnlyMemory<RecordPair<TRecord>> _cache;
        public           TRecord                             Current        => _current ?? throw new NullReferenceException( nameof(_current) );
        internal         bool                                ShouldContinue => _token.ShouldContinue() && _index < _cache.Length;


        // ReSharper disable once ConvertToPrimaryConstructor
        public AsyncEnumerator( TableCache<TRecord> tableCache, CancellationToken token = default )
        {
            _tableCache = tableCache;
            _token      = token;
        }
        public ValueTask DisposeAsync()
        {
            _isDisposed = true;
            _cache      = default;
            return default;
        }


        internal static bool MoveNext( ref int index, in ReadOnlySpan<RecordPair<TRecord>> span, [ NotNullWhen( true ) ] out RecordPair<TRecord>? current )
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

            // ReSharper disable once InvertIf
            if ( _cache.IsEmpty )
            {
                Interlocked.Exchange( ref _index, START_INDEX );
                ILockedCollection<RecordPair<TRecord>> collection = _tableCache._keys;
                _cache = await collection.CopyAsync( _token );
            }

            bool result = MoveNext( ref _index, _cache.Span, out _pair );

            if ( result is false ) { Reset(); }
            else
            {
                Debug.Assert( _pair.HasValue, nameof(_pair) + " != null" );
                _current = _tableCache[_pair.Value];
            }

            return result;
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

            _cache = default;
            Interlocked.Exchange( ref _index,   START_INDEX );
            Interlocked.Exchange( ref _current, null );
        }


        public override string ToString() => $"AsyncEnumerator<{typeof(TRecord).Name}>( {nameof(_index)} : {_index}, {nameof(ShouldContinue)} : {ShouldContinue} )";
    }
}
