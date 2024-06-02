// Jakar.Extensions :: Jakar.Database
// 09/01/2022  6:40 PM

using Microsoft.Extensions.Caching.Memory;



namespace Jakar.Database.Caches;


public interface ITableCacheService : IHostedService, IAsyncDisposable;



public interface ITableCache<TRecord> : IAsyncEnumerable<TRecord>, ITableCacheService
    where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public int  Count      { get; }
    public bool HasChanged { get; }
    public TRecord? this[ RecordPair<TRecord> id ] { get; }
    public TRecord? this[ RecordID<TRecord>   id ] { get; }
    public IEnumerable<RecordID<TRecord>> ChangedOrExpired { get; }
    public IEnumerable<RecordID<TRecord>> Keys             { get; }
    public IEnumerable<TRecord>           RecordsChanged   { get; }
    public IEnumerable<TRecord>           RecordsExpired   { get; }
    public void                           Reset();
    public bool                           Contains( RecordPair<TRecord>    id );
    public bool                           Contains( RecordID<TRecord>      id );
    public bool                           Contains( TRecord                record );
    public void                           AddOrUpdate( TRecord             record );
    public bool                           TryGetValue( RecordPair<TRecord> id,         [NotNullWhen( true )] out TRecord? record );
    public bool                           TryGetValue( RecordID<TRecord>   id,         [NotNullWhen( true )] out TRecord? record );
    public ValueTask<TRecord?>            TryGetValue( Activity?           activity,   RecordPair<TRecord>                pair, CancellationToken token );
    public ValueTask<TRecord?>            TryGetValue( Activity?           activity,   RecordID<TRecord>                  pair, CancellationToken token );
    public ValueTask<TRecord?>            TryRemove( Activity?             activity,   RecordPair<TRecord>                id,   CancellationToken token = default );
    public ValueTask<TRecord?>            TryRemove( Activity?             activity,   RecordID<TRecord>                  id,   CancellationToken token = default );
    public ValueTask                      RefreshAsync( Activity?          activity,   CancellationToken                  token );
    public ValueTask                      RefreshAsync( DbConnection       connection, DbTransaction                      transaction, Activity? activity, CancellationToken token = default );
}



public interface ITableCacheFactory : ITableCacheService
{
    ITableCache<TRecord> GetCache<TRecord>( DbTable<TRecord> table )
        where TRecord : class, ITableRecord<TRecord>, IDbReaderMapping<TRecord>;
}



public interface ITableCache : ITableCacheFactory, IDistributedCache, IMemoryCache;



public sealed class TableCacheFactory( ILoggerFactory factory, IOptions<TableCacheOptions> options, IMemoryCache memoryCache, IDistributedCache distributedCache ) : ITableCache
{
    private readonly ConcurrentBag<ITableCacheService> _services         = [];
    private readonly IDistributedCache                 _distributedCache = distributedCache;
    private readonly ILoggerFactory                    _factory          = factory;
    private readonly IMemoryCache                      _memoryCache      = memoryCache;
    private readonly IOptions<TableCacheOptions>       _options          = options;
    private          CancellationTokenSource?          _source;


    public void Dispose() => DisposeAsync().AsTask().Wait();
    public async ValueTask DisposeAsync()
    {
        if ( _source is not null )
        {
            await _source.CancelAsync();
            _source.Dispose();
            _source = null;
        }

        _memoryCache.Dispose();
        foreach ( ITableCacheService cache in _services ) { await cache.DisposeAsync(); }
    }


    public ITableCache<TRecord> GetCache<TRecord>( DbTable<TRecord> table )
        where TRecord : class, ITableRecord<TRecord>, IDbReaderMapping<TRecord>
    {
        _source?.Cancel();
        _source = null;
        TableCache<TRecord> cache = new(table, _factory.CreateLogger<TableCache<TRecord>>(), _options);
        _services.Add( cache );
        return cache;
    }


    public async Task StartAsync( CancellationToken token )
    {
        while ( token.ShouldContinue() )
        {
            try
            {
                CancellationTokenSource source = _source = CancellationTokenSource.CreateLinkedTokenSource( token );

                using ( source )
                {
                    var tasks = new Task[_services.Count];
                    foreach ( (int i, IHostedService service) in _services.Enumerate( 0 ) ) { tasks[i] = service.StartAsync( source.Token ); }

                    await Task.WhenAll( tasks );
                }
            }
            catch ( TaskCanceledException ) { await StopAsync( token ); }
        }
    }
    public async Task StopAsync( CancellationToken token )
    {
        if ( _source is not null )
        {
            await _source.CancelAsync();
            _source = null;
        }

        await Task.WhenAll( StopAllAsync( token ) );
    }
    private IEnumerable<Task> StopAllAsync( CancellationToken token )
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( ITableCacheService service in _services ) { yield return service.StopAsync( token ); }
    }


    public byte[]?       Get( string          key )                                  => _distributedCache.Get( key );
    public Task<byte[]?> GetAsync( string     key, CancellationToken token = new() ) => _distributedCache.GetAsync( key, token );
    public void          Refresh( string      key )                                  => _distributedCache.Refresh( key );
    public Task          RefreshAsync( string key, CancellationToken token = new() ) => _distributedCache.RefreshAsync( key, token );
    public void          Remove( string       key )                                                                                                 => _distributedCache.Remove( key );
    public Task          RemoveAsync( string  key, CancellationToken token = new() )                                                                => _distributedCache.RemoveAsync( key, token );
    public void          Set( string          key, byte[]            value, DistributedCacheEntryOptions options )                                  => _distributedCache.Set( key, value, options );
    public Task          SetAsync( string     key, byte[]            value, DistributedCacheEntryOptions options, CancellationToken token = new() ) => _distributedCache.SetAsync( key, value, options, token );
    public ICacheEntry   CreateEntry( object  key )                    => _memoryCache.CreateEntry( key );
    public void          Remove( object       key )                    => _memoryCache.Remove( key );
    public bool          TryGetValue( object  key, out object? value ) => _memoryCache.TryGetValue( key, out value );
}



public sealed class TableCache<TRecord>( DbTable<TRecord> table, ILogger<TableCache<TRecord>> logger, IOptions<TableCacheOptions> options ) : ITableCache<TRecord>
    where TRecord : class, ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    private readonly ConcurrentDictionary<RecordID<TRecord>, CacheEntry<TRecord>> _records = new(RecordID<TRecord>.Equalizer);
    private readonly DbTable<TRecord>                                             _table   = table;
    private readonly ILogger<TableCache<TRecord>>                                 _logger  = logger;
    private readonly TableCacheOptions                                            _options = options.Value;


    public int  Count      { [MethodImpl(                         MethodImplOptions.AggressiveInlining )] get => _records.Count; }
    public bool HasChanged { [MethodImpl(                         MethodImplOptions.AggressiveInlining )] get => _records.Values.Any( static x => x.HasChanged ); }
    public TRecord? this[ RecordPair<TRecord> id ] { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => this[id.ID]; }
    public TRecord? this[ RecordID<TRecord> id ]
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        get => TryGetValue( id, out TRecord? value )
                   ? value
                   : default;
    }
    public IEnumerable<RecordID<TRecord>> Keys { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _records.Keys; }
    public IEnumerable<RecordID<TRecord>> ChangedOrExpired
    {
        get
        {
            TimeSpan lifeSpan = _options.ExpireTime;

            foreach ( CacheEntry<TRecord> entry in _records.Values )
            {
                if ( entry.HasChangedOrExpired( lifeSpan ) ) { yield return entry.ID; }
            }
        }
    }
    public IEnumerable<TRecord> RecordsChanged
    {
        get
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach ( CacheEntry<TRecord> entry in _records.Values )
            {
                if ( entry.HasChanged is false ) { continue; }

                TRecord? record = entry.TryGetValue( _options );
                if ( record is not null ) { yield return record; }
            }
        }
    }
    public IEnumerable<TRecord> RecordsExpired
    {
        get
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach ( CacheEntry<TRecord> entry in _records.Values )
            {
                TRecord? record = entry.TryGetValue( _options );
                if ( record is not null ) { yield return record; }
            }
        }
    }


    public async ValueTask DisposeAsync()
    {
        await RefreshAsync( CancellationToken.None );
        Reset();
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public void Reset()                                => _records.Clear();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool Contains( RecordPair<TRecord> id )     => _records.ContainsKey( id.ID );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool Contains( RecordID<TRecord>   id )     => _records.ContainsKey( id );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool Contains( TRecord             record ) => _records.ContainsKey( record.ID );


    public void AddOrUpdate( TRecord record )
    {
        if ( _records.TryGetValue( record.ID, out CacheEntry<TRecord>? entry ) is false ) { _records[record.ID] = entry = new CacheEntry<TRecord>( record.ID ); }

        entry.SetValue( in record );
    }


    public ValueTask<TRecord?> TryGetValue( Activity? activity, RecordPair<TRecord> pair, CancellationToken token ) => TryGetValue( activity, pair.ID, token );
    public async ValueTask<TRecord?> TryGetValue( Activity? activity, RecordID<TRecord> id, CancellationToken token )
    {
        if ( _records.TryGetValue( id, out CacheEntry<TRecord>? entry ) is false ) { return default; }

        TRecord? record = await entry.TryGetValue( activity, _table, _options, token );
        if ( record is not null ) { return record; }

        _records.TryRemove( id, out _ );
        return null;
    }
    public bool TryGetValue( RecordPair<TRecord> id, [NotNullWhen( true )] out TRecord? record ) => TryGetValue( id.ID, out record );
    public bool TryGetValue( RecordID<TRecord> id, [NotNullWhen( true )] out TRecord? record )
    {
        if ( _records.TryGetValue( id, out CacheEntry<TRecord>? entry ) )
        {
            record = entry.TryGetValue( _options );
            return record is not null;
        }

        record = default;
        return false;
    }


    public ValueTask<TRecord?> TryRemove( Activity? activity, RecordPair<TRecord> id, CancellationToken token = default ) => TryRemove( activity, id.ID, token );
    public async ValueTask<TRecord?> TryRemove( Activity? activity, RecordID<TRecord> id, CancellationToken token = default )
    {
        if ( _records.TryRemove( id, out CacheEntry<TRecord>? entry ) ) { return await entry.TryGetValue( activity, _table, _options, token ); }

        return null;
    }


    public       ValueTask RefreshAsync( CancellationToken token )                             => RefreshAsync( Activity.Current, token );
    public async ValueTask RefreshAsync( Activity?         activity, CancellationToken token ) => await _table.TryCall( RefreshAsync, activity, token );
    public async ValueTask RefreshAsync( DbConnection connection, DbTransaction transaction, Activity? activity, CancellationToken token = default )
    {
        if ( _records.IsEmpty )
        {
            RecordID<TRecord>[] ids = _records.Keys.ToArray( _records.Count );

            foreach ( RecordID<TRecord> id in ids )
            {
                TRecord? record = await _table.Get( connection, transaction, activity, id, token );
                if ( record is null ) { continue; }

                AddOrUpdate( record );
            }

            return;
        }

        if ( HasChanged )
        {
            await _table.Update( connection, transaction, activity, RecordsChanged, token );

            await foreach ( TRecord record in _table.Get( connection, transaction, activity, ChangedOrExpired, token ) ) { AddOrUpdate( record ); }
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
            catch ( Exception e ) { Log.CacheFailure( _logger, e, nameof(TableCache<TRecord>) ); }
        }
    }
    public async Task StopAsync( CancellationToken token ) => await _table.Update( Activity.Current, RecordsChanged, token );


    IAsyncEnumerator<TRecord> IAsyncEnumerable<TRecord>.GetAsyncEnumerator( CancellationToken token )           => GetAsyncEnumerator( token );
    public AsyncEnumerator                              GetAsyncEnumerator( CancellationToken token = default ) => new(this, token);


    public ReadOnlyMemory<RecordPair<TRecord>> GetPairs()
    {
        ICollection<CacheEntry<TRecord>> values = _records.Values;
        RecordPair<TRecord>[]            array  = GC.AllocateUninitializedArray<RecordPair<TRecord>>( values.Count );
        foreach ( (int i, CacheEntry<TRecord>? entry) in values.Enumerate( 0 ) ) { array[i] = entry.ToPair(); }

        Array.Sort( array, RecordPair<TRecord>.Sorter );
        return array;
    }



    public sealed class AsyncEnumerator( TableCache<TRecord> tableCache, CancellationToken token = default ) : IAsyncEnumerable<TRecord>, IAsyncEnumerator<TRecord>
    {
        private const    int                                 START_INDEX = -1;
        private readonly TableCache<TRecord>                 _tableCache = tableCache;
        private          bool                                _isDisposed;
        private          CancellationToken                   _token = token;
        private          int                                 _index = START_INDEX;
        private          ReadOnlyMemory<RecordPair<TRecord>> _pairs;
        private          RecordPair<TRecord>                 _pair;
        private          TRecord?                            _current;
        private          Activity?                           _activity;

        public   TRecord Current        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _current ?? throw new NullReferenceException( nameof(_current) ); }
        internal bool    ShouldContinue { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _token.ShouldContinue() && _index < _pairs.Length; }


        public ValueTask DisposeAsync()
        {
            Reset();
            _isDisposed = true;
            return default;
        }


        public async ValueTask<bool> MoveNextAsync()
        {
            ThrowIfDisposed();
            if ( _pairs.IsEmpty ) { Reset(); }

            _index++;

            if ( ShouldContinue )
            {
                _pair    = _pairs.Span[_index];
                _current = await _tableCache.TryGetValue( _activity, _pair, _token );
                return _current is not null;
            }

            Reset();
            return false;
        }
        public void Reset()
        {
            ThrowIfDisposed();
            _pairs   = _tableCache.GetPairs();
            _pair    = default;
            _current = null;
            _index   = START_INDEX;
        }
        [MethodImpl( MethodImplOptions.AggressiveInlining )] private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf( _isDisposed, typeof(AsyncEnumerator) );


        IAsyncEnumerator<TRecord> IAsyncEnumerable<TRecord>.GetAsyncEnumerator( CancellationToken token ) => GetAsyncEnumerator( Activity.Current, token );
        public AsyncEnumerator GetAsyncEnumerator( Activity? activity, CancellationToken token = default )
        {
            Reset();
            _token    = token;
            _activity = activity;
            return this;
        }


        public override string ToString() => $"AsyncEnumerator<{typeof(TRecord).Name}>( {nameof(_index)} : {_index}, {nameof(ShouldContinue)} : {ShouldContinue} )";
    }
}
