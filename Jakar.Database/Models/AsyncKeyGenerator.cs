// Jakar.Extensions :: Jakar.Database
// 10/16/2022  4:52 PM

namespace Jakar.Database;


/// <summary>
///     <see href="https://stackoverflow.com/a/15992856/9530917"/>
/// </summary>
public sealed class AsyncKeyGenerator<TClass>( DbTable<TClass> table, CancellationToken token = default ) : IAsyncEnumerator<RecordID<TClass>>, IAsyncEnumerable<RecordID<TClass>>
    where TClass : class, ITableRecord<TClass>, IDbReaderMapping<TClass>
{
    private readonly DbTable<TClass>      _table = table;
    private          CancellationToken     _token = token;
    private          KeyGenerator<TClass> _generator;
    public           RecordID<TClass>     Current { get; private set; }


    public ValueTask DisposeAsync()
    {
        _generator.Dispose();
        Current   = default;
        return ValueTask.CompletedTask;
    }
    public void Reset() => _generator = default;


    public ValueTask<bool> MoveNextAsync() => _table.Call( MoveNextAsync, _token );
    public async ValueTask<bool> MoveNextAsync( DbConnection connection, DbTransaction? transaction, CancellationToken token )
    {
        if ( token.IsCancellationRequested )
        {
            Current = default;
            return false;
        }

        if ( _generator.IsEmpty )
        {
            IEnumerable<RecordPair<TClass>> pairs = await _table.SortedIDs( connection, transaction, token );
            _generator = KeyGenerator<TClass>.Create( pairs );
        }

        if ( _generator.MoveNext() ) { Current = _generator.Current; }
        else
        {
            Current    = default;
            _generator = default;
        }

        return Current.value != Guid.Empty;
    }


    IAsyncEnumerator<RecordID<TClass>> IAsyncEnumerable<RecordID<TClass>>.GetAsyncEnumerator( CancellationToken token ) => WithCancellation( token );
    public AsyncKeyGenerator<TClass> WithCancellation( CancellationToken token )
    {
        _token = token;
        return this;
    }
}
