// Jakar.Extensions :: Jakar.Database
// 10/16/2022  4:51 PM

namespace Jakar.Database;


/// <summary>
///     <see href="https://stackoverflow.com/a/15992856/9530917"/>
/// </summary>
public struct RecordGenerator<TRecord>( DbTable<TRecord> table ) : IAsyncEnumerable<TRecord>, IAsyncEnumerator<TRecord>
    where TRecord : class, ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    private readonly DbTable<TRecord>           _table     = table;
    private readonly CancellationToken          _token     = default;
    private          TRecord?                   _current   = default;
    private          AsyncKeyGenerator<TRecord> _generator = new(table);


    public TRecord Current => _current ?? throw new NullReferenceException( nameof(_current) );


    public RecordGenerator( DbTable<TRecord> table, CancellationToken token ) : this( table ) => _token = token;
    public async ValueTask DisposeAsync()
    {
        _current = default;
        await _generator.DisposeAsync();
        this = default;
    }


    public ValueTask<bool> MoveNextAsync() => _table.Call( MoveNextAsync, _token );
    public async ValueTask<bool> MoveNextAsync( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        if ( token.IsCancellationRequested )
        {
            _current = default;
            return false;
        }

        if ( await _generator.MoveNextAsync( connection, transaction, token ) ) { _current = await _table.Get( connection, transaction, _generator.Current, token ); }
        else
        {
            _current = default;
            _generator.Reset();
        }

        return _current is not null;
    }


    IAsyncEnumerator<TRecord> IAsyncEnumerable<TRecord>.GetAsyncEnumerator( CancellationToken token ) => WithCancellation( token );
    public RecordGenerator<TRecord>                     WithCancellation( CancellationToken   token ) => new(_table, token);
}
