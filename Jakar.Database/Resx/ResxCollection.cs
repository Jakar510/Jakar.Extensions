namespace Jakar.Database.Resx;


public sealed class ResxCollection : IResxCollection
{
    private readonly ConcurrentBag<ResxRowTable> _rows = new();


    public int Count => _rows.Count;


    public ResxCollection() { }
    public ResxCollection( IEnumerable<ResxRowTable> collection ) => _rows.Add( collection );


    public ResxSet GetSet( in SupportedLanguage language )
    {
        var set = new ResxSet( Count );
        foreach ( ResxRowTable row in this ) { set[row.ID] = row.GetValue( language ); }

        return set;
    }

    public async ValueTask<ResxSet> GetSetAsync( IResxProvider provider, SupportedLanguage language, CancellationToken token = default )
    {
        if ( _rows.IsEmpty ) { await Init( provider, token ); }

        return GetSet( language );
    }


    public ValueTask Init( IResxProvider  provider, CancellationToken         token                          = default ) => Init( provider, provider.Resx, token );
    public ValueTask Init( IConnectableDb db,       DbTableBase<ResxRowTable> table, CancellationToken token = default ) => db.Call( Init, table, token );
    public async ValueTask Init( DbConnection connection, DbTransaction? transaction, DbTableBase<ResxRowTable> table, CancellationToken token = default )
    {
        _rows.Clear();
        ResxRowTable[] records = await table.All( connection, transaction, token );
        _rows.Add( records );
    }


    public IEnumerator<ResxRowTable> GetEnumerator() => _rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
