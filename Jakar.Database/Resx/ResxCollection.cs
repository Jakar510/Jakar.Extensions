namespace Jakar.Database.Resx;


public sealed class ResxCollection : IResxCollection
{
    private readonly ConcurrentBag<ResxRowRecord> _rows = [];


    public int Count { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _rows.Count; }


    public ResxCollection() { }
    public ResxCollection( IEnumerable<ResxRowRecord> collection ) => _rows.Add( collection );


    public ResxSet GetSet( in SupportedLanguage language )
    {
        var set = new ResxSet( Count );
        foreach ( ResxRowRecord row in this ) { set[row.KeyID] = row.GetValue( language ); }

        return set;
    }

    public async ValueTask<ResxSet> GetSetAsync( Activity? activity, IResxProvider provider, SupportedLanguage language, CancellationToken token = default )
    {
        if ( _rows.IsEmpty ) { await Init( activity, provider, token ); }

        return GetSet( language );
    }


    public ValueTask Init( Activity? activity, IResxProvider  provider, CancellationToken      token                          = default ) => Init( activity, provider, provider.Resx, token );
    public ValueTask Init( Activity? activity, IConnectableDb db,       DbTable<ResxRowRecord> table, CancellationToken token = default ) => db.Call( Init, activity, table, token );
    public async ValueTask Init( DbConnection connection, DbTransaction? transaction, Activity? activity, DbTable<ResxRowRecord> table, CancellationToken token = default )
    {
        _rows.Clear();
        await foreach ( ResxRowRecord record in table.All( connection, transaction, activity, token ) ) { _rows.Add( record ); }
    }


    [MustDisposeResource] public IEnumerator<ResxRowRecord> GetEnumerator() => _rows.GetEnumerator();
    [MustDisposeResource]        IEnumerator IEnumerable.   GetEnumerator() => GetEnumerator();
}
