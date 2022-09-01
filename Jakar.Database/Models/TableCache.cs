namespace Jakar.Database;


public class TableCache<TRecord, TID> : IAsyncEnumerator<TRecord> where TRecord : UserRecord<TRecord, TID>
                                                                  where TID : IComparable<TID>, IEquatable<TID>
{
    protected readonly DbTable<TRecord, TID>                        _table;
    protected readonly ObservableConcurrentDictionary<TID, TRecord> _records = new();
    protected readonly HashSet<TID>                                 _changed = new();


    public TRecord Current { get; protected set; } = default!;


    public TableCache( DbTable<TRecord, TID> table ) => _table = table;
    public async ValueTask DisposeAsync()
    {
        await SaveToDB();
        _records.Clear();
    }

    protected async Task SaveToDB( CancellationToken token = default )
    {
        foreach ( TRecord record in _records.Values ) { await _table.Insert(record, token); }
    }
    protected async Task RefreshFromDB( CancellationToken token = default )
    {
        List<TRecord> records = await _table.All(token);
        _records.Clear();
        foreach ( TRecord record in records ) { _records.Add(record.ID, record); }
    }
    public async ValueTask<bool> MoveNextAsync() => false;
}
