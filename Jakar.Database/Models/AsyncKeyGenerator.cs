// Jakar.Extensions :: Jakar.Database
// 10/16/2022  4:52 PM

namespace Jakar.Database;


/// <summary>
///     <see href="https://stackoverflow.com/a/15992856/9530917"/>
/// </summary>
public record struct AsyncKeyGenerator<TRecord> : IAsyncEnumerator<Guid>, IAsyncEnumerable<Guid> where TRecord : TableRecord<TRecord>
{
    private readonly DbTable<TRecord>  _table;
    private readonly CancellationToken _token = default;
    private          KeyGenerator      _generator;
    public           Guid              Current { get; private set; } = default;


    public AsyncKeyGenerator( DbTable<TRecord> table ) => _table = table;
    public AsyncKeyGenerator( DbTable<TRecord> table, CancellationToken token ) : this( table ) => _token = token;
    public ValueTask DisposeAsync()
    {
        _generator.Dispose();
        Current = default;
        this    = default;
        return default;
    }
    public void Reset() => _generator = default;


    public ValueTask<bool> MoveNextAsync() => _table.Call( MoveNextAsync, _token );
    public async ValueTask<bool> MoveNextAsync( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        if ( token.IsCancellationRequested )
        {
            Current = default;
            return false;
        }

        if ( _generator.IsEmpty ) { _generator = await _table.SortedIDs( connection, transaction, token ); }

        if ( _generator.MoveNext() ) { Current = _generator.Current; }
        else
        {
            Current    = default;
            _generator = default;
        }

        return Current != Guid.Empty;
    }


    IAsyncEnumerator<Guid> IAsyncEnumerable<Guid>.GetAsyncEnumerator( CancellationToken token ) => WithCancellation( token );
    public AsyncKeyGenerator<TRecord> WithCancellation( CancellationToken               token ) => new(_table, token);
}
