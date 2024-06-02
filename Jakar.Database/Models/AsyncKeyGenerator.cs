// Jakar.Extensions :: Jakar.Database
// 10/16/2022  4:52 PM

namespace Jakar.Database;


/// <summary>
///     <see href="https://stackoverflow.com/a/15992856/9530917"/>
/// </summary>
public sealed class AsyncKeyGenerator<TRecord>( DbTable<TRecord> table, CancellationToken token = default ) : IAsyncEnumerator<RecordID<TRecord>>, IAsyncEnumerable<RecordID<TRecord>>
    where TRecord : class, ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    private readonly DbTable<TRecord>      _table = table;
    private          Activity?             _activity;
    private          CancellationToken     _token = token;
    private          KeyGenerator<TRecord> _generator;
    public           RecordID<TRecord>     Current { get; private set; }


    public ValueTask DisposeAsync()
    {
        _generator.Dispose();
        Current   = default;
        _activity = null;
        return default;
    }
    public void Reset() => _generator = default;


    public ValueTask<bool> MoveNextAsync() => _table.Call( MoveNextAsync, _activity, _token );
    public async ValueTask<bool> MoveNextAsync( DbConnection connection, DbTransaction? transaction, Activity? activity, CancellationToken token )
    {
        if ( token.IsCancellationRequested )
        {
            Current = default;
            return false;
        }

        if ( _generator.IsEmpty )
        {
            IEnumerable<RecordPair<TRecord>> pairs = await _table.SortedIDs( connection, transaction, activity, token );
            _generator = KeyGenerator<TRecord>.Create( pairs );
        }

        if ( _generator.MoveNext() ) { Current = _generator.Current; }
        else
        {
            Current    = default;
            _generator = default;
        }

        return Current.Value != Guid.Empty;
    }


    IAsyncEnumerator<RecordID<TRecord>> IAsyncEnumerable<RecordID<TRecord>>.GetAsyncEnumerator( CancellationToken token ) => WithCancellation( null, token );


    public AsyncKeyGenerator<TRecord> WithCancellation( Activity? activity, CancellationToken token )
    {
        _activity = activity;
        _token    = token;
        return this;
    }
}
