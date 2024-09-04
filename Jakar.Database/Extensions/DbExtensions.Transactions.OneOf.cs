namespace Jakar.Database;


public static partial class DbExtensions
{
    public static async IAsyncEnumerable<ErrorOrResult<TResult>> TryCall<TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction,  CancellationToken, IAsyncEnumerable<ErrorOrResult<TResult>>> func,  [EnumeratorCancellation] CancellationToken token = default )
    {
        await using DbConnection                 connection  = await db.ConnectAsync( token );
        await using DbTransaction                transaction = await connection.BeginTransactionAsync( db.TransactionIsolationLevel, token );
        IAsyncEnumerable<ErrorOrResult<TResult>> enumerable  = func( connection, transaction,  token );
        bool                                     passed      = true;

        await foreach ( ErrorOrResult<TResult> result in enumerable.WithCancellation( token ) )
        {
            if ( result.TryGetValue( out var value, out Error[]? errors ) ) { yield return value; }
            else
            {
                passed = false;
                yield return errors;
            }
        }

        if ( passed ) { await transaction.CommitAsync( token ); }
        else { await transaction.RollbackAsync( token ); }
    }
    public static async IAsyncEnumerable<ErrorOrResult<TResult>> TryCall<TArg1, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction,  TArg1, CancellationToken, IAsyncEnumerable<ErrorOrResult<TResult>>> func,  TArg1 arg1, [EnumeratorCancellation] CancellationToken token )
    {
        await using DbConnection                 connection  = await db.ConnectAsync( token );
        await using DbTransaction                transaction = await connection.BeginTransactionAsync( db.TransactionIsolationLevel, token );
        IAsyncEnumerable<ErrorOrResult<TResult>> enumerable  = func( connection, transaction,  arg1, token );
        bool                                     passed      = true;

        await foreach ( ErrorOrResult<TResult> result in enumerable.WithCancellation( token ) )
        {
            if ( result.TryGetValue( out var value, out Error[]? errors ) ) { yield return value; }
            else
            {
                passed = false;
                yield return errors;
            }
        }

        if ( passed ) { await transaction.CommitAsync( token ); }
        else { await transaction.RollbackAsync( token ); }
    }
    public static async IAsyncEnumerable<ErrorOrResult<TResult>> TryCall<TArg1, TArg2, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction,  TArg1, TArg2, CancellationToken, IAsyncEnumerable<ErrorOrResult<TResult>>> func,  TArg1 arg1, TArg2 arg2, [EnumeratorCancellation] CancellationToken token )
    {
        await using DbConnection                 connection  = await db.ConnectAsync( token );
        await using DbTransaction                transaction = await connection.BeginTransactionAsync( db.TransactionIsolationLevel, token );
        IAsyncEnumerable<ErrorOrResult<TResult>> enumerable  = func( connection, transaction,  arg1, arg2, token );
        bool                                     passed      = true;

        await foreach ( ErrorOrResult<TResult> result in enumerable.WithCancellation( token ) )
        {
            if ( result.TryGetValue( out var value, out Error[]? errors ) ) { yield return value; }
            else
            {
                passed = false;
                yield return errors;
            }
        }

        if ( passed ) { await transaction.CommitAsync( token ); }
        else { await transaction.RollbackAsync( token ); }
    }
    public static async IAsyncEnumerable<ErrorOrResult<TResult>> TryCall<TArg1, TArg2, TArg3, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction,  TArg1, TArg2, TArg3, CancellationToken, IAsyncEnumerable<ErrorOrResult<TResult>>> func,  TArg1 arg1, TArg2 arg2, TArg3 arg3, [EnumeratorCancellation] CancellationToken token )
    {
        await using DbConnection                 connection  = await db.ConnectAsync( token );
        await using DbTransaction                transaction = await connection.BeginTransactionAsync( db.TransactionIsolationLevel, token );
        IAsyncEnumerable<ErrorOrResult<TResult>> enumerable  = func( connection, transaction,  arg1, arg2, arg3, token );
        bool                                     passed      = true;

        await foreach ( ErrorOrResult<TResult> result in enumerable.WithCancellation( token ) )
        {
            if ( result.TryGetValue( out var value, out Error[]? errors ) ) { yield return value; }
            else
            {
                passed = false;
                yield return errors;
            }
        }

        if ( passed ) { await transaction.CommitAsync( token ); }
        else { await transaction.RollbackAsync( token ); }
    }
    public static async IAsyncEnumerable<ErrorOrResult<TResult>> TryCall<TArg1, TArg2, TArg3, TArg4, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction,  TArg1, TArg2, TArg3, TArg4, CancellationToken, IAsyncEnumerable<ErrorOrResult<TResult>>> func,  TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, [EnumeratorCancellation] CancellationToken token )
    {
        await using DbConnection                 connection  = await db.ConnectAsync( token );
        await using DbTransaction                transaction = await connection.BeginTransactionAsync( db.TransactionIsolationLevel, token );
        IAsyncEnumerable<ErrorOrResult<TResult>> enumerable  = func( connection, transaction,  arg1, arg2, arg3, arg4, token );
        bool                                     passed      = true;

        await foreach ( ErrorOrResult<TResult> result in enumerable.WithCancellation( token ) )
        {
            if ( result.TryGetValue( out var value, out Error[]? errors ) ) { yield return value; }
            else
            {
                passed = false;
                yield return errors;
            }
        }

        if ( passed ) { await transaction.CommitAsync( token ); }
        else { await transaction.RollbackAsync( token ); }
    }
    public static async IAsyncEnumerable<ErrorOrResult<TResult>> TryCall<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>( this IConnectableDb                                                                                                                          db,
                                                                                                                      Func<DbConnection, DbTransaction,  TArg1, TArg2, TArg3, TArg4, TArg5, CancellationToken, IAsyncEnumerable<ErrorOrResult<TResult>>> func,
                                                                                                                      
                                                                                                                      TArg1                                                                                                                                        arg1,
                                                                                                                      TArg2                                                                                                                                        arg2,
                                                                                                                      TArg3                                                                                                                                        arg3,
                                                                                                                      TArg4                                                                                                                                        arg4,
                                                                                                                      TArg5                                                                                                                                        arg5,
                                                                                                                      [EnumeratorCancellation] CancellationToken                                                                                                   token
    )
    {
        await using DbConnection                 connection  = await db.ConnectAsync( token );
        await using DbTransaction                transaction = await connection.BeginTransactionAsync( db.TransactionIsolationLevel, token );
        IAsyncEnumerable<ErrorOrResult<TResult>> enumerable  = func( connection, transaction,  arg1, arg2, arg3, arg4, arg5, token );
        bool                                     passed      = true;

        await foreach ( ErrorOrResult<TResult> result in enumerable.WithCancellation( token ) )
        {
            if ( result.TryGetValue( out var value, out Error[]? errors ) ) { yield return value; }
            else
            {
                passed = false;
                yield return errors;
            }
        }

        if ( passed ) { await transaction.CommitAsync( token ); }
        else { await transaction.RollbackAsync( token ); }
    }
    public static async IAsyncEnumerable<ErrorOrResult<TResult>> TryCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>( this IConnectableDb                                                                                                                                 db,
                                                                                                                             Func<DbConnection, DbTransaction,  TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, CancellationToken, IAsyncEnumerable<ErrorOrResult<TResult>>> func,
                                                                                                                             
                                                                                                                             TArg1                                                                                                                                               arg1,
                                                                                                                             TArg2                                                                                                                                               arg2,
                                                                                                                             TArg3                                                                                                                                               arg3,
                                                                                                                             TArg4                                                                                                                                               arg4,
                                                                                                                             TArg5                                                                                                                                               arg5,
                                                                                                                             TArg6                                                                                                                                               arg6,
                                                                                                                             [EnumeratorCancellation] CancellationToken                                                                                                          token
    )
    {
        await using DbConnection                 connection  = await db.ConnectAsync( token );
        await using DbTransaction                transaction = await connection.BeginTransactionAsync( db.TransactionIsolationLevel, token );
        IAsyncEnumerable<ErrorOrResult<TResult>> enumerable  = func( connection, transaction,  arg1, arg2, arg3, arg4, arg5, arg6, token );
        bool                                     passed      = true;

        await foreach ( ErrorOrResult<TResult> result in enumerable.WithCancellation( token ) )
        {
            if ( result.TryGetValue( out var value, out Error[]? errors ) ) { yield return value; }
            else
            {
                passed = false;
                yield return errors;
            }
        }

        if ( passed ) { await transaction.CommitAsync( token ); }
        else { await transaction.RollbackAsync( token ); }
    }
    public static async IAsyncEnumerable<ErrorOrResult<TResult>> TryCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>( this IConnectableDb                                                                                                                                        db,
                                                                                                                                    Func<DbConnection, DbTransaction,  TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, CancellationToken, IAsyncEnumerable<ErrorOrResult<TResult>>> func,
                                                                                                                                    
                                                                                                                                    TArg1                                                                                                                                                      arg1,
                                                                                                                                    TArg2                                                                                                                                                      arg2,
                                                                                                                                    TArg3                                                                                                                                                      arg3,
                                                                                                                                    TArg4                                                                                                                                                      arg4,
                                                                                                                                    TArg5                                                                                                                                                      arg5,
                                                                                                                                    TArg6                                                                                                                                                      arg6,
                                                                                                                                    TArg7                                                                                                                                                      arg7,
                                                                                                                                    [EnumeratorCancellation] CancellationToken                                                                                                                 token
    )
    {
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( db.TransactionIsolationLevel, token );

        IAsyncEnumerable<ErrorOrResult<TResult>> enumerable = func( connection,
                                                                    transaction,
                                                                    
                                                                    arg1,
                                                                    arg2,
                                                                    arg3,
                                                                    arg4,
                                                                    arg5,
                                                                    arg6,
                                                                    arg7,
                                                                    token );

        bool passed = true;

        await foreach ( ErrorOrResult<TResult> result in enumerable.WithCancellation( token ) )
        {
            if ( result.TryGetValue( out var value, out Error[]? errors ) ) { yield return value; }
            else
            {
                passed = false;
                yield return errors;
            }
        }

        if ( passed ) { await transaction.CommitAsync( token ); }
        else { await transaction.RollbackAsync( token ); }
    }
    public static async IAsyncEnumerable<ErrorOrResult<TResult>> TryCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>( this IConnectableDb                                                                                                                                               db,
                                                                                                                                           Func<DbConnection, DbTransaction,  TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, CancellationToken, IAsyncEnumerable<ErrorOrResult<TResult>>> func,
                                                                                                                                           
                                                                                                                                           TArg1                                                                                                                                                             arg1,
                                                                                                                                           TArg2                                                                                                                                                             arg2,
                                                                                                                                           TArg3                                                                                                                                                             arg3,
                                                                                                                                           TArg4                                                                                                                                                             arg4,
                                                                                                                                           TArg5                                                                                                                                                             arg5,
                                                                                                                                           TArg6                                                                                                                                                             arg6,
                                                                                                                                           TArg7                                                                                                                                                             arg7,
                                                                                                                                           TArg8                                                                                                                                                             arg8,
                                                                                                                                           [EnumeratorCancellation] CancellationToken                                                                                                                        token
    )
    {
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( db.TransactionIsolationLevel, token );

        IAsyncEnumerable<ErrorOrResult<TResult>> enumerable = func( connection,
                                                                    transaction,
                                                                    
                                                                    arg1,
                                                                    arg2,
                                                                    arg3,
                                                                    arg4,
                                                                    arg5,
                                                                    arg6,
                                                                    arg7,
                                                                    arg8,
                                                                    token );

        bool passed = true;

        await foreach ( ErrorOrResult<TResult> result in enumerable.WithCancellation( token ) )
        {
            if ( result.TryGetValue( out var value, out Error[]? errors ) ) { yield return value; }
            else
            {
                passed = false;
                yield return errors;
            }
        }

        if ( passed ) { await transaction.CommitAsync( token ); }
        else { await transaction.RollbackAsync( token ); }
    }


    public static async ValueTask<ErrorOrResult<TResult>> TryCall<TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction,  CancellationToken, ValueTask<ErrorOrResult<TResult>>> func,  CancellationToken token = default )
    {
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( db.TransactionIsolationLevel, token );

        try
        {
            ErrorOrResult<TResult> result = await func( connection, transaction,  token );
            if ( result.HasValue ) { await transaction.CommitAsync( token ); }

            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync( token );
            throw;
        }
    }
    public static async ValueTask<ErrorOrResult<TResult>> TryCall<TArg1, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction,  TArg1, CancellationToken, ValueTask<ErrorOrResult<TResult>>> func,  TArg1 arg1, CancellationToken token = default )
    {
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( db.TransactionIsolationLevel, token );

        try
        {
            ErrorOrResult<TResult> result = await func( connection, transaction,  arg1, token );
            if ( result.HasValue ) { await transaction.CommitAsync( token ); }

            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync( token );
            throw;
        }
    }
    public static async ValueTask<ErrorOrResult<TResult>> TryCall<TArg1, TArg2, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction,  TArg1, TArg2, CancellationToken, ValueTask<ErrorOrResult<TResult>>> func,  TArg1 arg1, TArg2 arg2, CancellationToken token = default )
    {
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( db.TransactionIsolationLevel, token );

        try
        {
            ErrorOrResult<TResult> result = await func( connection, transaction,  arg1, arg2, token );
            if ( result.HasValue ) { await transaction.CommitAsync( token ); }

            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync( token );
            throw;
        }
    }
    public static async ValueTask<ErrorOrResult<TResult>> TryCall<TArg1, TArg2, TArg3, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction,  TArg1, TArg2, TArg3, CancellationToken, ValueTask<ErrorOrResult<TResult>>> func,  TArg1 arg1, TArg2 arg2, TArg3 arg3, CancellationToken token )
    {
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( db.TransactionIsolationLevel, token );

        try
        {
            ErrorOrResult<TResult> result = await func( connection, transaction,  arg1, arg2, arg3, token );
            if ( result.HasValue ) { await transaction.CommitAsync( token ); }

            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync( token );
            throw;
        }
    }
    public static async ValueTask<ErrorOrResult<TResult>> TryCall<TArg1, TArg2, TArg3, TArg4, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction,  TArg1, TArg2, TArg3, TArg4, CancellationToken, ValueTask<ErrorOrResult<TResult>>> func,  TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, CancellationToken token )
    {
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( db.TransactionIsolationLevel, token );

        try
        {
            ErrorOrResult<TResult> result = await func( connection, transaction,  arg1, arg2, arg3, arg4, token );
            if ( result.HasValue ) { await transaction.CommitAsync( token ); }

            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync( token );
            throw;
        }
    }
    public static async ValueTask<ErrorOrResult<TResult>> TryCall<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction,  TArg1, TArg2, TArg3, TArg4, TArg5, CancellationToken, ValueTask<ErrorOrResult<TResult>>> func,  TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, CancellationToken token )
    {
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( db.TransactionIsolationLevel, token );

        try
        {
            ErrorOrResult<TResult> result = await func( connection, transaction,  arg1, arg2, arg3, arg4, arg5, token );
            if ( result.HasValue ) { await transaction.CommitAsync( token ); }

            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync( token );
            throw;
        }
    }
    public static async ValueTask<ErrorOrResult<TResult>> TryCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>( this IConnectableDb                                                                                                                          db,
                                                                                                                      Func<DbConnection, DbTransaction,  TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, CancellationToken, ValueTask<ErrorOrResult<TResult>>> func,
                                                                                                                      
                                                                                                                      TArg1                                                                                                                                        arg1,
                                                                                                                      TArg2                                                                                                                                        arg2,
                                                                                                                      TArg3                                                                                                                                        arg3,
                                                                                                                      TArg4                                                                                                                                        arg4,
                                                                                                                      TArg5                                                                                                                                        arg5,
                                                                                                                      TArg6                                                                                                                                        arg6,
                                                                                                                      CancellationToken                                                                                                                            token
    )
    {
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( db.TransactionIsolationLevel, token );

        try
        {
            ErrorOrResult<TResult> result = await func( connection, transaction,  arg1, arg2, arg3, arg4, arg5, arg6, token );
            if ( result.HasValue ) { await transaction.CommitAsync( token ); }

            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync( token );
            throw;
        }
    }
    public static async ValueTask<ErrorOrResult<TResult>> TryCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>( this IConnectableDb                                                                                                                                 db,
                                                                                                                             Func<DbConnection, DbTransaction,  TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, CancellationToken, ValueTask<ErrorOrResult<TResult>>> func,
                                                                                                                             
                                                                                                                             TArg1                                                                                                                                               arg1,
                                                                                                                             TArg2                                                                                                                                               arg2,
                                                                                                                             TArg3                                                                                                                                               arg3,
                                                                                                                             TArg4                                                                                                                                               arg4,
                                                                                                                             TArg5                                                                                                                                               arg5,
                                                                                                                             TArg6                                                                                                                                               arg6,
                                                                                                                             TArg7                                                                                                                                               arg7,
                                                                                                                             CancellationToken                                                                                                                                   token
    )
    {
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( db.TransactionIsolationLevel, token );

        try
        {
            ErrorOrResult<TResult> result = await func( connection,
                                                        transaction,
                                                        
                                                        arg1,
                                                        arg2,
                                                        arg3,
                                                        arg4,
                                                        arg5,
                                                        arg6,
                                                        arg7,
                                                        token );

            if ( result.HasValue ) { await transaction.CommitAsync( token ); }

            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync( token );
            throw;
        }
    }
    public static async ValueTask<ErrorOrResult<TResult>> TryCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>( this IConnectableDb                                                                                                                                        db,
                                                                                                                                    Func<DbConnection, DbTransaction,  TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, CancellationToken, ValueTask<ErrorOrResult<TResult>>> func,
                                                                                                                                    
                                                                                                                                    TArg1                                                                                                                                                      arg1,
                                                                                                                                    TArg2                                                                                                                                                      arg2,
                                                                                                                                    TArg3                                                                                                                                                      arg3,
                                                                                                                                    TArg4                                                                                                                                                      arg4,
                                                                                                                                    TArg5                                                                                                                                                      arg5,
                                                                                                                                    TArg6                                                                                                                                                      arg6,
                                                                                                                                    TArg7                                                                                                                                                      arg7,
                                                                                                                                    TArg8                                                                                                                                                      arg8,
                                                                                                                                    CancellationToken                                                                                                                                          token
    )
    {
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( db.TransactionIsolationLevel, token );

        try
        {
            ErrorOrResult<TResult> result = await func( connection,
                                                        transaction,
                                                        
                                                        arg1,
                                                        arg2,
                                                        arg3,
                                                        arg4,
                                                        arg5,
                                                        arg6,
                                                        arg7,
                                                        arg8,
                                                        token );

            if ( result.HasValue ) { await transaction.CommitAsync( token ); }

            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync( token );
            throw;
        }
    }
}
