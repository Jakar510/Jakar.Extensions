// Jakar.Extensions :: Jakar.Database
// 09/06/2022  4:54 PM

namespace Jakar.Database;


public static class TableLinq
{
    public static async IAsyncEnumerable<TClass> CreateAsync<TClass>( this DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
        where TClass : class, ITableRecord<TClass>, IDbReaderMapping<TClass>
    {
        while ( await reader.ReadAsync(token) ) { yield return TClass.Create(reader); }
    }


    public static async IAsyncEnumerable<TClass> Where<TClass>( this IAsyncEnumerable<TClass> source, Func<NpgsqlConnection, DbTransaction?, TClass, CancellationToken, bool> func, NpgsqlConnection connection, DbTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TClass record in source.WithCancellation(token) )
        {
            if ( func(connection, transaction, record, token) ) { yield return record; }
        }
    }
    public static async IAsyncEnumerable<TClass> Where<TClass>( this IAsyncEnumerable<TClass> source, Func<NpgsqlConnection, DbTransaction?, TClass, CancellationToken, Task<bool>> func, NpgsqlConnection connection, DbTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TClass record in source.WithCancellation(token) )
        {
            if ( await func(connection, transaction, record, token) ) { yield return record; }
        }
    }
    public static async IAsyncEnumerable<TResult> Select<TClass, TResult>( this IAsyncEnumerable<TClass> source, Func<NpgsqlConnection, DbTransaction?, TClass, CancellationToken, TResult> func, NpgsqlConnection connection, DbTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TClass record in source.WithCancellation(token) ) { yield return func(connection, transaction, record, token); }
    }
    public static async IAsyncEnumerable<TResult> Select<TClass, TResult>( this IAsyncEnumerable<TClass> source, Func<NpgsqlConnection, DbTransaction?, TClass, CancellationToken, Task<TResult>> func, NpgsqlConnection connection, DbTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TClass record in source.WithCancellation(token) ) { yield return await func(connection, transaction, record, token); }
    }
}
