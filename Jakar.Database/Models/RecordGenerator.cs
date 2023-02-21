// Jakar.Extensions :: Jakar.Database
// 10/16/2022  4:51 PM

namespace Jakar.Database;


/// <summary>
///     <see href="https://stackoverflow.com/a/15992856/9530917"/>
/// </summary>
public struct RecordGenerator<TRecord> : IAsyncEnumerator<TRecord?> where TRecord : TableRecord<TRecord>
{
    private readonly DbTable<TRecord> _table;
    private          TRecord?         _current = default;


    private Guid? _ID => _current?.ID;
    public TRecord Current
    {
        get => _current ?? throw new NullReferenceException( nameof(_current) );
        set => _current = value;
    }


    public RecordGenerator( DbTable<TRecord> table ) => _table = table;
    public ValueTask DisposeAsync()
    {
        _current = default;
        return default;
    }


    public void Reset() => _current = default;
    public async ValueTask<bool> MoveNextAsync( CancellationToken token = default )
    {
        await using DbConnection connection = await _table.ConnectAsync( token );
        return await MoveNextAsync( connection, default, token );
    }
    public async ValueTask<bool> MoveNextAsync( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        if ( token.IsCancellationRequested )
        {
            _current = default;
            return default;
        }

        _current = await _table.Next( connection, transaction, _ID, token );
        return _current is not null;
    }
    ValueTask<bool> IAsyncEnumerator<TRecord?>.MoveNextAsync() => MoveNextAsync();
}
