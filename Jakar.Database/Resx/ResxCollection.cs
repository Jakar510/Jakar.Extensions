namespace Jakar.Database.Resx;


public sealed class ResxCollection : IResxCollection
{
    private readonly ConcurrentBag<ResxRowRecord> _rows = new();


    public int Count => _rows.Count;


    public ResxCollection() { }
    public ResxCollection( IEnumerable<ResxRowRecord> collection ) => _rows.Add( collection );


    public ResxSet GetSet( in SupportedLanguage language )
    {
        var set = new ResxSet( Count );
        foreach ( ResxRowRecord row in this ) { set[row.KeyID] = row.GetValue( language ); }

        return set;
    }

    public async ValueTask<ResxSet> GetSetAsync( IResxProvider provider, SupportedLanguage language, CancellationToken token = default )
    {
        if ( _rows.IsEmpty ) { await Init( provider, token ); }

        return GetSet( language );
    }


    public ValueTask Init( IResxProvider  provider, CancellationToken     token                          = default ) => Init( provider, provider.Resx, token );
    public ValueTask Init( IConnectableDb db,       DbTable<ResxRowRecord> table, CancellationToken token = default ) => db.Call( Init, table, token );
    public async ValueTask Init( DbConnection connection, DbTransaction? transaction, DbTable<ResxRowRecord> table, CancellationToken token = default )
    {
        _rows.Clear();
        IEnumerable<ResxRowRecord> records = await table.All( connection, transaction, token );
        _rows.Add( records );
    }


    public IEnumerator<ResxRowRecord> GetEnumerator() => _rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
