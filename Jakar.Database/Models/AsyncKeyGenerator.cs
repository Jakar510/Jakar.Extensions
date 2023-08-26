// Jakar.Extensions :: Jakar.Database
// 10/16/2022  4:52 PM

namespace Jakar.Database;


/// <summary>
///     <see href="https://stackoverflow.com/a/15992856/9530917"/>
/// </summary>
public record struct AsyncKeyGenerator<TRecord> : IAsyncEnumerator<RecordID<TRecord>>, IAsyncEnumerable<RecordID<TRecord>> where TRecord : TableRecord<TRecord>
{
    private readonly DbTable<TRecord>      _table;
    private readonly CancellationToken     _token;
    private          KeyGenerator<TRecord> _generator;
    public           RecordID<TRecord>     Current { get; private set; } = default;


    public AsyncKeyGenerator( DbTable<TRecord> table, CancellationToken token = default )
    {
        _table = table;
        _token = token;
    }
    public ValueTask DisposeAsync()
    {
        _generator.Dispose();
        Current = default;
        this    = default;
        return default;
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
            IEnumerable<RecordPair<TRecord>> pairs = await _table.SortedIDs( connection, transaction, token );
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


    IAsyncEnumerator<RecordID<TRecord>> IAsyncEnumerable<RecordID<TRecord>>.GetAsyncEnumerator( CancellationToken token ) => WithCancellation( token );
    [Pure] public readonly AsyncKeyGenerator<TRecord> WithCancellation( CancellationToken                         token ) => new(_table, token);
}
