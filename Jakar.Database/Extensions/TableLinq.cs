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


    public static async ValueTask<TSelf[]> CreateAsync<TSelf>( this DbDataReader reader, int initialCapacity, [EnumeratorCancellation] CancellationToken token = default )
        where TSelf : class, ITableRecord<TSelf>
    {
        List<TSelf> list = new(initialCapacity);
        while ( await reader.ReadAsync(token) ) { list.Add(TSelf.Create(reader)); }

        return list.ToArray();
    }


    public static async IAsyncEnumerable<TSelf> Where<TSelf>( this IAsyncEnumerable<TSelf> source, Func<NpgsqlConnection, DbTransaction?, TSelf, CancellationToken, bool> func, NpgsqlConnection connection, DbTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TSelf record in source.WithCancellation(token) )
        {
            if ( func(connection, transaction, record, token) ) { yield return record; }
        }
    }
    public static async IAsyncEnumerable<TSelf> Where<TSelf>( this IAsyncEnumerable<TSelf> source, Func<NpgsqlConnection, DbTransaction?, TSelf, CancellationToken, ValueTask<bool>> func, NpgsqlConnection connection, DbTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
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
    public static async IAsyncEnumerable<TResult> Select<TSelf, TResult>( this IAsyncEnumerable<TSelf> source, Func<NpgsqlConnection, DbTransaction?, TSelf, CancellationToken, ValueTask<TResult>> func, NpgsqlConnection connection, DbTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TSelf record in source.WithCancellation(token) ) { yield return await func(connection, transaction, record, token); }
    }
}
