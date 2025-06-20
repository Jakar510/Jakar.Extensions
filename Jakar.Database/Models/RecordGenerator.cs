// Jakar.Extensions :: Jakar.Database
// 10/16/2022  4:51 PM

namespace Jakar.Database;


/// <summary>
///     <see href="https://stackoverflow.com/a/15992856/9530917"/>
/// </summary>
public sealed class RecordGenerator<TClass>( DbTable<TClass> table ) : IAsyncEnumerable<TClass>, IAsyncEnumerator<TClass>
    where TClass : class, ITableRecord<TClass>, IDbReaderMapping<TClass>
{
    private readonly AsyncKeyGenerator<TClass> _generator = new(table);
    private readonly DbTable<TClass>           _table     = table;
    private          CancellationToken          _token;
    private          TClass?                   _current;


    public TClass Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _current ?? throw new NullReferenceException( nameof(_current) ); }


    public RecordGenerator( DbTable<TClass> table, CancellationToken token ) : this( table ) => _token = token;
    public async ValueTask DisposeAsync()
    {
        _current = null;
        await _generator.DisposeAsync();
    }


    public ValueTask<bool> MoveNextAsync() => _table.Call( MoveNextAsync, _token );
    public async ValueTask<bool> MoveNextAsync( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        if ( token.IsCancellationRequested )
        {
            _current = null;
            return false;
        }

        if ( await _generator.MoveNextAsync( connection, transaction, token ) ) { _current = await _table.Get( connection, transaction, _generator.Current, token ); }
        else
        {
            _current = null;
            _generator.Reset();
        }

        return _current is not null;
    }


    IAsyncEnumerator<TClass> IAsyncEnumerable<TClass>.GetAsyncEnumerator( CancellationToken token ) => WithCancellation( token );
    public RecordGenerator<TClass> WithCancellation( CancellationToken token )
    {
        _token = token;
        _generator.WithCancellation( token );
        return this;
    }
}
