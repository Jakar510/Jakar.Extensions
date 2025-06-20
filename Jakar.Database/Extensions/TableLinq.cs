// Jakar.Extensions :: Jakar.Database
// 09/06/2022  4:54 PM

namespace Jakar.Database;


public static class TableLinq
{
    public static async IAsyncEnumerable<TClass> Where<TClass>( this IAsyncEnumerable<TClass> source, Func<DbConnection, DbTransaction?, TClass, CancellationToken, bool> func, DbConnection connection, DbTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TClass record in source.WithCancellation( token ) )
        {
            if ( func( connection, transaction, record, token ) ) { yield return record; }
        }
    }
    public static async IAsyncEnumerable<TClass> Where<TClass>( this IAsyncEnumerable<TClass> source, Func<DbConnection, DbTransaction?, TClass, CancellationToken, Task<bool>> func, DbConnection connection, DbTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TClass record in source.WithCancellation( token ) )
        {
            if ( await func( connection, transaction, record, token ) ) { yield return record; }
        }
    }
    public static async IAsyncEnumerable<TResult> Select<TClass, TResult>( this IAsyncEnumerable<TClass> source, Func<DbConnection, DbTransaction?, TClass, CancellationToken, TResult> func, DbConnection connection, DbTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TClass record in source.WithCancellation( token ) ) { yield return func( connection, transaction, record, token ); }
    }
    public static async IAsyncEnumerable<TResult> Select<TClass, TResult>( this IAsyncEnumerable<TClass> source, Func<DbConnection, DbTransaction?, TClass, CancellationToken, Task<TResult>> func, DbConnection connection, DbTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TClass record in source.WithCancellation( token ) ) { yield return await func( connection, transaction, record, token ); }
    }
}
