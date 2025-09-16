// Jakar.Extensions :: Jakar.Database
// 10/16/2022  4:52 PM

namespace Jakar.Database;


/// <summary>
///     <see href="https://stackoverflow.com/a/15992856/9530917"/>
/// </summary>
public sealed class AsyncKeyGenerator<TClass>( DbTable<TClass> table, CancellationToken token = default ) : IAsyncEnumerator<RecordID<TClass>>, IAsyncEnumerable<RecordID<TClass>>
    where TClass : class, ITableRecord<TClass>, IDbReaderMapping<TClass>
{
    private readonly DbTable<TClass>      __table = table;
    private          CancellationToken     __token = token;
    private          KeyGenerator<TClass> __generator;
    public           RecordID<TClass>     Current { get; private set; }


    public ValueTask DisposeAsync()
    {
        __generator.Dispose();
        Current   = default;
        return ValueTask.CompletedTask;
    }
    public void Reset() => __generator = default;


    public ValueTask<bool> MoveNextAsync() => __table.Call( MoveNextAsync, __token );
    public async ValueTask<bool> MoveNextAsync( DbConnection connection, DbTransaction? transaction, CancellationToken token )
    {
        if ( token.IsCancellationRequested )
        {
            Current = default;
            return false;
        }

        if ( __generator.IsEmpty )
        {
            IEnumerable<RecordPair<TClass>> pairs = await __table.SortedIDs( connection, transaction, token );
            __generator = KeyGenerator<TClass>.Create( pairs );
        }

        if ( __generator.MoveNext() ) { Current = __generator.Current; }
        else
        {
            Current    = default;
            __generator = default;
        }

        return Current.value != Guid.Empty;
    }


    IAsyncEnumerator<RecordID<TClass>> IAsyncEnumerable<RecordID<TClass>>.GetAsyncEnumerator( CancellationToken token ) => WithCancellation( token );
    public AsyncKeyGenerator<TClass> WithCancellation( CancellationToken token )
    {
        __token = token;
        return this;
    }
}
