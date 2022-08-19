// Jakar.Extensions :: Jakar.Database
// 08/18/2022  10:35 PM


namespace Jakar.Database;


public static class DbExtensions
{
    public static bool IsValidRowID<TID>( this IUniqueID<TID>    unique ) where TID : IComparable<TID>, IEquatable<TID> => !unique.ID.Equals(default);
    public static bool IsValidRowID( this      IUniqueID<Guid>   unique ) => unique.ID != Guid.Empty;
    public static bool IsValidRowID( this      IUniqueID<long>   unique ) => unique.ID > 0;
    public static bool IsValidRowID( this      IUniqueID<int>    unique ) => unique.ID > 0;
    public static bool IsValidRowID( this      IUniqueID<string> unique ) => !string.IsNullOrWhiteSpace(unique.ID);


    public static string GetTableName<TRecord>() where TRecord : class => GetTableName(typeof(TRecord));
    public static string GetTableName( in Type type ) => type.GetCustomAttribute<Dapper.Contrib.Extensions.TableAttribute>()
                                                            ?.Name ?? type.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.TableAttribute>()
                                                                         ?.Name ?? type.Name;


    public static async Task Call( this IConnectableDb db, Func<DbConnection, DbTransaction, CancellationToken, Task> func, CancellationToken token )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);

        try
        {
            await func(conn, transaction, token);
            await transaction.CommitAsync(token);
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }
    public static async Task Call<TArg1>( this IConnectableDb db, Func<DbConnection, DbTransaction, TArg1, CancellationToken, Task> func, TArg1 arg1, CancellationToken token )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);

        try
        {
            await func(conn, transaction, arg1, token);
            await transaction.CommitAsync(token);
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }
    public static async Task Call<TArg1, TArg2>( this IConnectableDb db, Func<DbConnection, DbTransaction, TArg1, TArg2, CancellationToken, Task> func, TArg1 arg1, TArg2 arg2, CancellationToken token )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);

        try
        {
            await func(conn, transaction, arg1, arg2, token);
            await transaction.CommitAsync(token);
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }
    public static async Task Call<TArg1, TArg2, TArg3>( this IConnectableDb db, Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, CancellationToken, Task> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, CancellationToken token )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);

        try
        {
            await func(conn, transaction, arg1, arg2, arg3, token);
            await transaction.CommitAsync(token);
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }
    public static async Task Call<TArg1, TArg2, TArg3, TArg4>( this IConnectableDb                                                                    db,
                                                               Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, CancellationToken, Task> func,
                                                               TArg1                                                                                  arg1,
                                                               TArg2                                                                                  arg2,
                                                               TArg3                                                                                  arg3,
                                                               TArg4                                                                                  arg4,
                                                               CancellationToken                                                                      token
    )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);

        try
        {
            await func(conn, transaction, arg1, arg2, arg3, arg4, token);
            await transaction.CommitAsync(token);
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }
    public static async Task Call<TArg1, TArg2, TArg3, TArg4, TArg5>( this IConnectableDb                                                                           db,
                                                                      Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, CancellationToken, Task> func,
                                                                      TArg1                                                                                         arg1,
                                                                      TArg2                                                                                         arg2,
                                                                      TArg3                                                                                         arg3,
                                                                      TArg4                                                                                         arg4,
                                                                      TArg5                                                                                         arg5,
                                                                      CancellationToken                                                                             token
    )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);

        try
        {
            await func(conn, transaction, arg1, arg2, arg3, arg4, arg5, token);
            await transaction.CommitAsync(token);
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }
    public static async Task Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>( this IConnectableDb                                                                                  db,
                                                                             Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, CancellationToken, Task> func,
                                                                             TArg1                                                                                                arg1,
                                                                             TArg2                                                                                                arg2,
                                                                             TArg3                                                                                                arg3,
                                                                             TArg4                                                                                                arg4,
                                                                             TArg5                                                                                                arg5,
                                                                             TArg6                                                                                                arg6,
                                                                             CancellationToken                                                                                    token
    )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);

        try
        {
            await func(conn, transaction, arg1, arg2, arg3, arg4, arg5, arg6, token);
            await transaction.CommitAsync(token);
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }
    public static async Task Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>( this IConnectableDb                                                                                         db,
                                                                                    Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, CancellationToken, Task> func,
                                                                                    TArg1                                                                                                       arg1,
                                                                                    TArg2                                                                                                       arg2,
                                                                                    TArg3                                                                                                       arg3,
                                                                                    TArg4                                                                                                       arg4,
                                                                                    TArg5                                                                                                       arg5,
                                                                                    TArg6                                                                                                       arg6,
                                                                                    TArg7                                                                                                       arg7,
                                                                                    CancellationToken                                                                                           token
    )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);

        try
        {
            await func(conn, transaction, arg1, arg2, arg3, arg4, arg5, arg6, arg7, token);
            await transaction.CommitAsync(token);
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }
    public static async Task Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>( this IConnectableDb                                                                                                db,
                                                                                           Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, CancellationToken, Task> func,
                                                                                           TArg1                                                                                                              arg1,
                                                                                           TArg2                                                                                                              arg2,
                                                                                           TArg3                                                                                                              arg3,
                                                                                           TArg4                                                                                                              arg4,
                                                                                           TArg5                                                                                                              arg5,
                                                                                           TArg6                                                                                                              arg6,
                                                                                           TArg7                                                                                                              arg7,
                                                                                           TArg8                                                                                                              arg8,
                                                                                           CancellationToken                                                                                                  token
    )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);

        try
        {
            await func(conn,
                       transaction,
                       arg1,
                       arg2,
                       arg3,
                       arg4,
                       arg5,
                       arg6,
                       arg7,
                       arg8,
                       token);

            await transaction.CommitAsync(token);
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }


    public static async Task<TResult> Call<TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction, CancellationToken, Task<TResult>> func, CancellationToken token )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);

        try
        {
            TResult result = await func(conn, transaction, token);
            await transaction.CommitAsync(token);
            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }
    public static async Task<TResult> Call<TArg1, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction, TArg1, CancellationToken, Task<TResult>> func, TArg1 arg1, CancellationToken token )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);

        try
        {
            TResult result = await func(conn, transaction, arg1, token);
            await transaction.CommitAsync(token);
            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }
    public static async Task<TResult> Call<TArg1, TArg2, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction, TArg1, TArg2, CancellationToken, Task<TResult>> func, TArg1 arg1, TArg2 arg2, CancellationToken token )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);

        try
        {
            TResult result = await func(conn, transaction, arg1, arg2, token);
            await transaction.CommitAsync(token);
            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }
    public static async Task<TResult> Call<TArg1, TArg2, TArg3, TResult>( this IConnectableDb                                                                      db,
                                                                          Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, CancellationToken, Task<TResult>> func,
                                                                          TArg1                                                                                    arg1,
                                                                          TArg2                                                                                    arg2,
                                                                          TArg3                                                                                    arg3,
                                                                          CancellationToken                                                                        token
    )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);

        try
        {
            TResult result = await func(conn, transaction, arg1, arg2, arg3, token);
            await transaction.CommitAsync(token);
            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }
    public static async Task<TResult> Call<TArg1, TArg2, TArg3, TArg4, TResult>( this IConnectableDb                                                                             db,
                                                                                 Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, CancellationToken, Task<TResult>> func,
                                                                                 TArg1                                                                                           arg1,
                                                                                 TArg2                                                                                           arg2,
                                                                                 TArg3                                                                                           arg3,
                                                                                 TArg4                                                                                           arg4,
                                                                                 CancellationToken                                                                               token
    )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);

        try
        {
            TResult result = await func(conn, transaction, arg1, arg2, arg3, arg4, token);
            await transaction.CommitAsync(token);
            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }
    public static async Task<TResult> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>( this IConnectableDb                                                                                    db,
                                                                                        Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, CancellationToken, Task<TResult>> func,
                                                                                        TArg1                                                                                                  arg1,
                                                                                        TArg2                                                                                                  arg2,
                                                                                        TArg3                                                                                                  arg3,
                                                                                        TArg4                                                                                                  arg4,
                                                                                        TArg5                                                                                                  arg5,
                                                                                        CancellationToken                                                                                      token
    )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);

        try
        {
            TResult result = await func(conn, transaction, arg1, arg2, arg3, arg4, arg5, token);
            await transaction.CommitAsync(token);
            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }
    public static async Task<TResult> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>( this IConnectableDb                                                                                           db,
                                                                                               Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, CancellationToken, Task<TResult>> func,
                                                                                               TArg1                                                                                                         arg1,
                                                                                               TArg2                                                                                                         arg2,
                                                                                               TArg3                                                                                                         arg3,
                                                                                               TArg4                                                                                                         arg4,
                                                                                               TArg5                                                                                                         arg5,
                                                                                               TArg6                                                                                                         arg6,
                                                                                               CancellationToken                                                                                             token
    )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);

        try
        {
            TResult result = await func(conn, transaction, arg1, arg2, arg3, arg4, arg5, arg6, token);
            await transaction.CommitAsync(token);
            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }
    public static async Task<TResult> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>( this IConnectableDb                                                                                                  db,
                                                                                                      Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, CancellationToken, Task<TResult>> func,
                                                                                                      TArg1                                                                                                                arg1,
                                                                                                      TArg2                                                                                                                arg2,
                                                                                                      TArg3                                                                                                                arg3,
                                                                                                      TArg4                                                                                                                arg4,
                                                                                                      TArg5                                                                                                                arg5,
                                                                                                      TArg6                                                                                                                arg6,
                                                                                                      TArg7                                                                                                                arg7,
                                                                                                      CancellationToken                                                                                                    token
    )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);

        try
        {
            TResult result = await func(conn, transaction, arg1, arg2, arg3, arg4, arg5, arg6, arg7, token);
            await transaction.CommitAsync(token);
            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }
    public static async Task<TResult> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>( this IConnectableDb                                                                                                         db,
                                                                                                             Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, CancellationToken, Task<TResult>> func,
                                                                                                             TArg1                                                                                                                       arg1,
                                                                                                             TArg2                                                                                                                       arg2,
                                                                                                             TArg3                                                                                                                       arg3,
                                                                                                             TArg4                                                                                                                       arg4,
                                                                                                             TArg5                                                                                                                       arg5,
                                                                                                             TArg6                                                                                                                       arg6,
                                                                                                             TArg7                                                                                                                       arg7,
                                                                                                             TArg8                                                                                                                       arg8,
                                                                                                             CancellationToken                                                                                                           token
    )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);

        try
        {
            TResult result = await func(conn,
                                        transaction,
                                        arg1,
                                        arg2,
                                        arg3,
                                        arg4,
                                        arg5,
                                        arg6,
                                        arg7,
                                        arg8,
                                        token);

            await transaction.CommitAsync(token);
            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }


    public static async IAsyncEnumerable<TResult> Call<TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction, CancellationToken, IAsyncEnumerable<TResult>> func, [EnumeratorCancellation] CancellationToken token )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);
        IAsyncEnumerable<TResult> result;

        try
        {
            result = func(conn, transaction, token);

            await transaction.CommitAsync(token);
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }

        await foreach ( TResult item in result.WithCancellation(token) ) { yield return item; }
    }
    public static async IAsyncEnumerable<TResult> Call<TArg1, TResult>( this IConnectableDb                                                                    db,
                                                                        Func<DbConnection, DbTransaction, TArg1, CancellationToken, IAsyncEnumerable<TResult>> func,
                                                                        TArg1                                                                                  arg1,
                                                                        [EnumeratorCancellation] CancellationToken                                             token
    )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);
        IAsyncEnumerable<TResult> result;

        try
        {
            result = func(conn, transaction, arg1, token);

            await transaction.CommitAsync(token);
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }

        await foreach ( TResult item in result.WithCancellation(token) ) { yield return item; }
    }
    public static async IAsyncEnumerable<TResult> Call<TArg1, TArg2, TResult>( this IConnectableDb                                                                           db,
                                                                               Func<DbConnection, DbTransaction, TArg1, TArg2, CancellationToken, IAsyncEnumerable<TResult>> func,
                                                                               TArg1                                                                                         arg1,
                                                                               TArg2                                                                                         arg2,
                                                                               [EnumeratorCancellation] CancellationToken                                                    token
    )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);
        IAsyncEnumerable<TResult> result;

        try
        {
            result = func(conn, transaction, arg1, arg2, token);

            await transaction.CommitAsync(token);
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }

        await foreach ( TResult item in result.WithCancellation(token) ) { yield return item; }
    }
    public static async IAsyncEnumerable<TResult> Call<TArg1, TArg2, TArg3, TResult>( this IConnectableDb                                                                                  db,
                                                                                      Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, CancellationToken, IAsyncEnumerable<TResult>> func,
                                                                                      TArg1                                                                                                arg1,
                                                                                      TArg2                                                                                                arg2,
                                                                                      TArg3                                                                                                arg3,
                                                                                      [EnumeratorCancellation] CancellationToken                                                           token
    )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);
        IAsyncEnumerable<TResult> result;

        try
        {
            result = func(conn, transaction, arg1, arg2, arg3, token);

            await transaction.CommitAsync(token);
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }

        await foreach ( TResult item in result.WithCancellation(token) ) { yield return item; }
    }
    public static async IAsyncEnumerable<TResult> Call<TArg1, TArg2, TArg3, TArg4, TResult>( this IConnectableDb                                                                                         db,
                                                                                             Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, CancellationToken, IAsyncEnumerable<TResult>> func,
                                                                                             TArg1                                                                                                       arg1,
                                                                                             TArg2                                                                                                       arg2,
                                                                                             TArg3                                                                                                       arg3,
                                                                                             TArg4                                                                                                       arg4,
                                                                                             [EnumeratorCancellation] CancellationToken                                                                  token
    )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);
        IAsyncEnumerable<TResult> result;

        try
        {
            result = func(conn, transaction, arg1, arg2, arg3, arg4, token);

            await transaction.CommitAsync(token);
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }

        await foreach ( TResult item in result.WithCancellation(token) ) { yield return item; }
    }
    public static async IAsyncEnumerable<TResult> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>( this IConnectableDb                                                                                                db,
                                                                                                    Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, CancellationToken, IAsyncEnumerable<TResult>> func,
                                                                                                    TArg1                                                                                                              arg1,
                                                                                                    TArg2                                                                                                              arg2,
                                                                                                    TArg3                                                                                                              arg3,
                                                                                                    TArg4                                                                                                              arg4,
                                                                                                    TArg5                                                                                                              arg5,
                                                                                                    [EnumeratorCancellation] CancellationToken                                                                         token
    )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);
        IAsyncEnumerable<TResult> result;

        try
        {
            result = func(conn, transaction, arg1, arg2, arg3, arg4, arg5, token);

            await transaction.CommitAsync(token);
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }

        await foreach ( TResult item in result.WithCancellation(token) ) { yield return item; }
    }
    public static async IAsyncEnumerable<TResult> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>( this IConnectableDb                                                                                                       db,
                                                                                                           Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, CancellationToken, IAsyncEnumerable<TResult>> func,
                                                                                                           TArg1                                                                                                                     arg1,
                                                                                                           TArg2                                                                                                                     arg2,
                                                                                                           TArg3                                                                                                                     arg3,
                                                                                                           TArg4                                                                                                                     arg4,
                                                                                                           TArg5                                                                                                                     arg5,
                                                                                                           TArg6                                                                                                                     arg6,
                                                                                                           [EnumeratorCancellation] CancellationToken                                                                                token
    )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);
        IAsyncEnumerable<TResult> result;

        try
        {
            result = func(conn, transaction, arg1, arg2, arg3, arg4, arg5, arg6, token);

            await transaction.CommitAsync(token);
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }

        await foreach ( TResult item in result.WithCancellation(token) ) { yield return item; }
    }
    public static async IAsyncEnumerable<TResult> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>( this IConnectableDb                                                                                                              db,
                                                                                                                  Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, CancellationToken, IAsyncEnumerable<TResult>> func,
                                                                                                                  TArg1                                                                                                                            arg1,
                                                                                                                  TArg2                                                                                                                            arg2,
                                                                                                                  TArg3                                                                                                                            arg3,
                                                                                                                  TArg4                                                                                                                            arg4,
                                                                                                                  TArg5                                                                                                                            arg5,
                                                                                                                  TArg6                                                                                                                            arg6,
                                                                                                                  TArg7                                                                                                                            arg7,
                                                                                                                  [EnumeratorCancellation] CancellationToken                                                                                       token
    )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);
        IAsyncEnumerable<TResult> result;

        try
        {
            result = func(conn, transaction, arg1, arg2, arg3, arg4, arg5, arg6, arg7, token);

            await transaction.CommitAsync(token);
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }

        await foreach ( TResult item in result.WithCancellation(token) ) { yield return item; }
    }
    public static async IAsyncEnumerable<TResult> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>( this IConnectableDb db,
                                                                                                                         Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, CancellationToken,
                                                                                                                             IAsyncEnumerable<TResult>> func,
                                                                                                                         TArg1                                      arg1,
                                                                                                                         TArg2                                      arg2,
                                                                                                                         TArg3                                      arg3,
                                                                                                                         TArg4                                      arg4,
                                                                                                                         TArg5                                      arg5,
                                                                                                                         TArg6                                      arg6,
                                                                                                                         TArg7                                      arg7,
                                                                                                                         TArg8                                      arg8,
                                                                                                                         [EnumeratorCancellation] CancellationToken token
    )
    {
        await using DbConnection  conn        = await db.ConnectAsync(token);
        await using DbTransaction transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);
        IAsyncEnumerable<TResult> result;

        try
        {
            result = func(conn,
                          transaction,
                          arg1,
                          arg2,
                          arg3,
                          arg4,
                          arg5,
                          arg6,
                          arg7,
                          arg8,
                          token);

            await transaction.CommitAsync(token);
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync(token);
            throw;
        }

        await foreach ( TResult item in result.WithCancellation(token) ) { yield return item; }
    }
}
