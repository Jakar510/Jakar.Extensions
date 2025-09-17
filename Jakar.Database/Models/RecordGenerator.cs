// Jakar.Extensions :: Jakar.Database
// 10/16/2022  4:51 PM

namespace Jakar.Database;


/// <summary>
///     <see href="https://stackoverflow.com/a/15992856/9530917"/>
/// </summary>
public sealed class RecordGenerator<TClass>( DbTable<TClass> table ) : IAsyncEnumerable<TClass>, IAsyncEnumerator<TClass>
    where TClass : class, ITableRecord<TClass>, IDbReaderMapping<TClass>
{
    private readonly AsyncKeyGenerator<TClass> __generator = new(table);
    private readonly DbTable<TClass>           __table     = table;
    private          CancellationToken         __token;
    private          TClass?                   __current;


    public TClass Current { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __current ?? throw new NullReferenceException(nameof(__current)); }


    public RecordGenerator( DbTable<TClass> table, CancellationToken token ) : this(table) => __token = token;
    public async ValueTask DisposeAsync()
    {
        __current = null;
        await __generator.DisposeAsync();
    }


    public ValueTask<bool> MoveNextAsync() => __table.Call(MoveNextAsync, __token);
    public async ValueTask<bool> MoveNextAsync( DbConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        if ( token.IsCancellationRequested )
        {
            __current = null;
            return false;
        }

        if ( await __generator.MoveNextAsync(connection, transaction, token) ) { __current = await __table.Get(connection, transaction, __generator.Current, token); }
        else
        {
            __current = null;
            __generator.Reset();
        }

        return __current is not null;
    }


    IAsyncEnumerator<TClass> IAsyncEnumerable<TClass>.GetAsyncEnumerator( CancellationToken token ) => WithCancellation(token);
    public RecordGenerator<TClass> WithCancellation( CancellationToken token )
    {
        __token = token;
        __generator.WithCancellation(token);
        return this;
    }
}
