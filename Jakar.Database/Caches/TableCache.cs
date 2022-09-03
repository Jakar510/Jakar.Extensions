// Jakar.Extensions :: Jakar.Database
// 09/01/2022  6:40 PM

namespace Jakar.Database;


public sealed class TableCache<TRecord, TID> : Service, IHostedService, IReadOnlyCollection<TRecord>, IAsyncEnumerator<TRecord?> where TRecord : BaseTableRecord<TRecord, TID>
                                                                                                                                 where TID : IComparable<TID>, IEquatable<TID>
{
    public sealed class CacheEntry : CacheEntry<TRecord, TID>
    {
        public CacheEntry( TRecord                          value ) : base(value) { }
        public static implicit operator CacheEntry( TRecord value ) => new(value);
    }



    private readonly ILogger                               _logger;
    private readonly TimeSpan                              _refreshTime;
    private readonly IDbTable<TRecord, TID>                _table;
    private readonly KeyGenerator<TID, CacheEntry>         _generator;
    private readonly ConcurrentDictionary<TID, CacheEntry> _records = new();


    public bool                 HasChanged     => _records.Values.Any(x => x.HasChanged);
    public IEnumerable<TID>     Changed        => from entry in _records.Values where entry.HasChanged select entry.ID;
    public IEnumerable<TRecord> RecordsChanged => from entry in _records.Values where entry.HasChanged select entry.Value;


    public TRecord? this[ TID key ] => TryGetValue(key, out TRecord? value)
                                           ? value
                                           : default;
    public IEnumerable<TID>     Keys    => _records.Keys;
    public IEnumerable<TRecord> Records => _records.Values.Select(x => x.Value);
    public TRecord?             Current => this[_generator.Current];
    public int                  Count   => _records.Count;


    public TableCache( IDbTable<TRecord, TID> table, IOptions<TableCacheOptions> options ) : this(table, options.Value) { }
    public TableCache( IDbTable<TRecord, TID> table, TableCacheOptions           options ) : this(table, options.Factory, options.RefreshTime) { }
    public TableCache( IDbTable<TRecord, TID> table, ILoggerFactory factory, TimeSpan refreshTime ) : base()
    {
        _table       = table;
        _refreshTime = refreshTime;
        _logger      = factory.CreateLogger(GetType());
        _generator   = new KeyGenerator<TID, CacheEntry>(_records);
    }
    protected override void Dispose( bool disposing )
    {
        _generator.Dispose();
        _records.Clear();
        Clear();
    }
    public override ValueTask DisposeAsync()
    {
        _records.Clear();
        _generator.Dispose();
        return default;
    }


    public async ValueTask<bool> MoveNextAsync()
    {
        await _table.Call(Refresh);
        return _generator.MoveNext();
    }
    public void Reset() => _generator.Reset();


    public async Task StartAsync( CancellationToken token )
    {
        using var timer = new PeriodicTimer(_refreshTime);

        while ( !token.IsCancellationRequested )
        {
            try
            {
                await timer.WaitForNextTickAsync(token);
                await _table.Call(Refresh, token);
            }
            catch ( Exception e ) { _logger.LogCritical(e, default, Array.Empty<object?>()); }
        }
    }
    public async Task StopAsync( CancellationToken token ) => await _table.Update(RecordsChanged, token);
    private async Task Refresh( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        if ( HasChanged )
        {
            await _table.Update(connection, transaction, RecordsChanged, token);
            await AddOrUpdate(_table.Get(connection, transaction, Changed, token), token);
        }


        List<TRecord> records = await _table.All(connection, transaction, token);
        _records.Clear();
        AddOrUpdate(records);
    }


    public async Task AddOrUpdate( IAsyncEnumerable<TRecord?> records, CancellationToken token = default )
    {
        await foreach ( TRecord? record in records.WithCancellation(token) )
        {
            if ( record is null ) { continue; }

            AddOrUpdate(record);
        }
    }
    public void AddOrUpdate( IEnumerable<TRecord> records )
    {
        foreach ( TRecord? record in records ) { AddOrUpdate(record); }
    }
    public void AddOrUpdate( TRecord record )
    {
        if ( Contains(record.ID) )
        {
            _records[record.ID]
               .Value = record;
        }
        else { _records.TryAdd(record.ID, record); }
    }


    public void Clear() => _records.Clear();
    public bool Contains( TID     key ) => _records.Values.Any(x => x.ID.Equals(key));
    public bool Contains( TRecord key ) => _records.Values.Any(x => x.Equals(key));


    public bool TryRemove( TRecord pair ) => _records.TryRemove(pair.ID, out _);
    public bool TryRemove( TID     key ) => _records.TryRemove(key,      out _);


    public bool TryGetValue( TID key, [NotNullWhen(true)] out TRecord? value )
    {
        if ( _records.TryGetValue(key, out CacheEntry? entry) )
        {
            value = entry.Value;
            return true;
        }

        value = default;
        return false;
    }


    public IEnumerator<TRecord> GetEnumerator() => Records.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
