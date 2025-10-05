// Jakar.Extensions :: Jakar.Database
// 09/06/2022  4:54 PM

namespace Jakar.Database;


public static class TableLinq
{
    public static async IAsyncEnumerable<TSelf> CreateAsync<TSelf>( this DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
        where TSelf : class, ITableRecord<TSelf>
    {
        while ( await reader.ReadAsync(token) ) { yield return TSelf.Create(reader); }
    }


    public static async IAsyncEnumerable<TSelf> Where<TSelf>( this IAsyncEnumerable<TSelf> source, Func<NpgsqlConnection, DbTransaction?, TSelf, CancellationToken, bool> func, NpgsqlConnection connection, DbTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TSelf record in source.WithCancellation(token) )
        {
            if ( func(connection, transaction, record, token) ) { yield return record; }
        }
    }
    public static async IAsyncEnumerable<TSelf> Where<TSelf>( this IAsyncEnumerable<TSelf> source, Func<NpgsqlConnection, DbTransaction?, TSelf, CancellationToken, Task<bool>> func, NpgsqlConnection connection, DbTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TSelf record in source.WithCancellation(token) )
        {
            if ( await func(connection, transaction, record, token) ) { yield return record; }
        }
    }
    public static async IAsyncEnumerable<TResult> Select<TSelf, TResult>( this IAsyncEnumerable<TSelf> source, Func<NpgsqlConnection, DbTransaction?, TSelf, CancellationToken, TResult> func, NpgsqlConnection connection, DbTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TSelf record in source.WithCancellation(token) ) { yield return func(connection, transaction, record, token); }
    }
    public static async IAsyncEnumerable<TResult> Select<TSelf, TResult>( this IAsyncEnumerable<TSelf> source, Func<NpgsqlConnection, DbTransaction?, TSelf, CancellationToken, Task<TResult>> func, NpgsqlConnection connection, DbTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TSelf record in source.WithCancellation(token) ) { yield return await func(connection, transaction, record, token); }
    }
}
