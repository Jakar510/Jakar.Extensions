// Jakar.Extensions :: Jakar.Database
// 09/06/2022  4:54 PM

namespace Jakar.Database;


public static class TableLinq
{
    public static async IAsyncEnumerable<TResult> Select<TRecord, TResult>(this IAsyncEnumerable<TRecord> source,
                                                                            Func<DbConnection, DbTransaction?, TRecord, CancellationToken, TResult> func,
                                                                            DbConnection connection,
                                                                            DbTransaction? transaction,
                                                                            [EnumeratorCancellation] CancellationToken token = default
    )
    {
        await foreach (TRecord record in source.WithCancellation( token )) { yield return func( connection, transaction, record, token ); }
    }
    public static async IAsyncEnumerable<TResult> Select<TRecord, TResult>(this IAsyncEnumerable<TRecord> source,
                                                                            Func<DbConnection, DbTransaction?, TRecord, CancellationToken, Task<TResult>> func,
                                                                            DbConnection connection,
                                                                            DbTransaction? transaction,
                                                                            [EnumeratorCancellation] CancellationToken token = default
    )
    {
        await foreach (TRecord record in source.WithCancellation( token )) { yield return await func( connection, transaction, record, token ); }
    }


    public static async IAsyncEnumerable<TRecord> Where<TRecord>(this IAsyncEnumerable<TRecord> source,
                                                                  Func<DbConnection, DbTransaction?, TRecord, CancellationToken, bool> func,
                                                                  DbConnection connection,
                                                                  DbTransaction? transaction,
                                                                  [EnumeratorCancellation] CancellationToken token = default
    )
    {
        await foreach (TRecord record in source.WithCancellation( token ))
        {
            if (func( connection, transaction, record, token )) { yield return record; }
        }
    }
    public static async IAsyncEnumerable<TRecord> Where<TRecord>(this IAsyncEnumerable<TRecord> source,
                                                                  Func<DbConnection, DbTransaction?, TRecord, CancellationToken, Task<bool>> func,
                                                                  DbConnection connection,
                                                                  DbTransaction? transaction,
                                                                  [EnumeratorCancellation] CancellationToken token = default
    )
    {
        await foreach (TRecord record in source.WithCancellation( token ))
        {
            if (await func( connection, transaction, record, token )) { yield return record; }
        }
    }
}
