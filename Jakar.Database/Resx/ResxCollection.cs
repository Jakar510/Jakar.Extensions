namespace Jakar.Database.Resx;


public sealed class ResxCollection : IResxCollection
{
    private readonly ConcurrentBag<ResxRowRecord> __rows = [];


    public int Count { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __rows.Count; }


    public ResxCollection() { }
    public ResxCollection( IEnumerable<ResxRowRecord> collection ) => __rows.Add(collection);


    public ResxSet GetSet( in SupportedLanguage language )
    {
        ResxSet set = new(Count);
        foreach ( ResxRowRecord row in this ) { set[row.KeyID] = row.GetValue(language); }

        return set;
    }

    public async ValueTask<ResxSet> GetSetAsync( IResxProvider provider, SupportedLanguage language, CancellationToken token = default )
    {
        if ( __rows.IsEmpty ) { await Init(provider, token); }

        return GetSet(language);
    }


    public ValueTask Init( IResxProvider  provider, CancellationToken      token                          = default ) => Init(provider, provider.Resx, token);
    public ValueTask Init( IConnectableDb db,       DbTable<ResxRowRecord> table, CancellationToken token = default ) => db.Call(Init, table, token);
    public async ValueTask Init( DbConnection connection, DbTransaction? transaction, DbTable<ResxRowRecord> table, CancellationToken token = default )
    {
        __rows.Clear();
        await foreach ( ResxRowRecord record in table.All(connection, transaction, token) ) { __rows.Add(record); }
    }


    [MustDisposeResource] public IEnumerator<ResxRowRecord> GetEnumerator() => __rows.GetEnumerator();
    [MustDisposeResource]        IEnumerator IEnumerable.   GetEnumerator() => GetEnumerator();
}
