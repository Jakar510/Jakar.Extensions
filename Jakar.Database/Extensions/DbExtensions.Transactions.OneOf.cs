

namespace Jakar.Database;


public static partial class DbExtensions
{
    public static async IAsyncEnumerable<OneOf<TResult, Error>> TryCall<TResult>( this IConnectableDb                                                                           db,
                                                                                  Func<DbConnection, DbTransaction, CancellationToken, IAsyncEnumerable<OneOf<TResult, Error>>> func,
                                                                                  [EnumeratorCancellation] CancellationToken                                                    token = default
    )
    {
        await using DbConnection                connection  = await db.ConnectAsync( token );
        await using DbTransaction               transaction = await connection.BeginTransactionAsync( IsolationLevel.ReadCommitted, token );
        IAsyncEnumerable<OneOf<TResult, Error>> enumerable  = func( connection, transaction, token );
        bool                                    passed      = true;

        await foreach ( OneOf<TResult, Error> result in enumerable.WithCancellation( token ) )
        {
            if ( result.IsT0 ) { yield return result.AsT0; }
            else
            {
                passed = false;
                yield return result.AsT1;
            }
        }

        if ( passed ) { await transaction.CommitAsync( token ); }
        else { await transaction.RollbackAsync( token ); }
    }
    public static async IAsyncEnumerable<OneOf<TResult, Error>> TryCall<TArg1, TResult>( this IConnectableDb                                                                                  db,
                                                                                         Func<DbConnection, DbTransaction, TArg1, CancellationToken, IAsyncEnumerable<OneOf<TResult, Error>>> func,
                                                                                         TArg1                                                                                                arg1,
                                                                                         [EnumeratorCancellation] CancellationToken                                                           token
    )
    {
        await using DbConnection                connection  = await db.ConnectAsync( token );
        await using DbTransaction               transaction = await connection.BeginTransactionAsync( IsolationLevel.ReadCommitted, token );
        IAsyncEnumerable<OneOf<TResult, Error>> enumerable  = func( connection, transaction, arg1, token );
        bool                                    passed      = true;

        await foreach ( OneOf<TResult, Error> result in enumerable.WithCancellation( token ) )
        {
            if ( result.IsT0 ) { yield return result.AsT0; }
            else
            {
                passed = false;
                yield return result.AsT1;
            }
        }

        if ( passed ) { await transaction.CommitAsync( token ); }
        else { await transaction.RollbackAsync( token ); }
    }
    public static async IAsyncEnumerable<OneOf<TResult, Error>> TryCall<TArg1, TArg2, TResult>( this IConnectableDb                                                                                         db,
                                                                                                Func<DbConnection, DbTransaction, TArg1, TArg2, CancellationToken, IAsyncEnumerable<OneOf<TResult, Error>>> func,
                                                                                                TArg1                                                                                                       arg1,
                                                                                                TArg2                                                                                                       arg2,
                                                                                                [EnumeratorCancellation] CancellationToken                                                                  token
    )
    {
        await using DbConnection                connection  = await db.ConnectAsync( token );
        await using DbTransaction               transaction = await connection.BeginTransactionAsync( IsolationLevel.ReadCommitted, token );
        IAsyncEnumerable<OneOf<TResult, Error>> enumerable  = func( connection, transaction, arg1, arg2, token );
        bool                                    passed      = true;

        await foreach ( OneOf<TResult, Error> result in enumerable.WithCancellation( token ) )
        {
            if ( result.IsT0 ) { yield return result.AsT0; }
            else
            {
                passed = false;
                yield return result.AsT1;
            }
        }

        if ( passed ) { await transaction.CommitAsync( token ); }
        else { await transaction.RollbackAsync( token ); }
    }
    public static async IAsyncEnumerable<OneOf<TResult, Error>> TryCall<TArg1, TArg2, TArg3, TResult>( this IConnectableDb                                                                                                db,
                                                                                                       Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, CancellationToken, IAsyncEnumerable<OneOf<TResult, Error>>> func,
                                                                                                       TArg1                                                                                                              arg1,
                                                                                                       TArg2                                                                                                              arg2,
                                                                                                       TArg3                                                                                                              arg3,
                                                                                                       [EnumeratorCancellation] CancellationToken                                                                         token
    )
    {
        await using DbConnection                connection  = await db.ConnectAsync( token );
        await using DbTransaction               transaction = await connection.BeginTransactionAsync( IsolationLevel.ReadCommitted, token );
        IAsyncEnumerable<OneOf<TResult, Error>> enumerable  = func( connection, transaction, arg1, arg2, arg3, token );
        bool                                    passed      = true;

        await foreach ( OneOf<TResult, Error> result in enumerable.WithCancellation( token ) )
        {
            if ( result.IsT0 ) { yield return result.AsT0; }
            else
            {
                passed = false;
                yield return result.AsT1;
            }
        }

        if ( passed ) { await transaction.CommitAsync( token ); }
        else { await transaction.RollbackAsync( token ); }
    }
    public static async IAsyncEnumerable<OneOf<TResult, Error>> TryCall<TArg1, TArg2, TArg3, TArg4, TResult>( this IConnectableDb                                                                                                       db,
                                                                                                              Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, CancellationToken, IAsyncEnumerable<OneOf<TResult, Error>>> func,
                                                                                                              TArg1                                                                                                                     arg1,
                                                                                                              TArg2                                                                                                                     arg2,
                                                                                                              TArg3                                                                                                                     arg3,
                                                                                                              TArg4                                                                                                                     arg4,
                                                                                                              [EnumeratorCancellation] CancellationToken                                                                                token
    )
    {
        await using DbConnection                connection  = await db.ConnectAsync( token );
        await using DbTransaction               transaction = await connection.BeginTransactionAsync( IsolationLevel.ReadCommitted, token );
        IAsyncEnumerable<OneOf<TResult, Error>> enumerable  = func( connection, transaction, arg1, arg2, arg3, arg4, token );
        bool                                    passed      = true;

        await foreach ( OneOf<TResult, Error> result in enumerable.WithCancellation( token ) )
        {
            if ( result.IsT0 ) { yield return result.AsT0; }
            else
            {
                passed = false;
                yield return result.AsT1;
            }
        }

        if ( passed ) { await transaction.CommitAsync( token ); }
        else { await transaction.RollbackAsync( token ); }
    }
    public static async IAsyncEnumerable<OneOf<TResult, Error>> TryCall<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>( this IConnectableDb db,
                                                                                                                     Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, CancellationToken, IAsyncEnumerable<OneOf<TResult, Error>>>
                                                                                                                         func,
                                                                                                                     TArg1                                      arg1,
                                                                                                                     TArg2                                      arg2,
                                                                                                                     TArg3                                      arg3,
                                                                                                                     TArg4                                      arg4,
                                                                                                                     TArg5                                      arg5,
                                                                                                                     [EnumeratorCancellation] CancellationToken token
    )
    {
        await using DbConnection                connection  = await db.ConnectAsync( token );
        await using DbTransaction               transaction = await connection.BeginTransactionAsync( IsolationLevel.ReadCommitted, token );
        IAsyncEnumerable<OneOf<TResult, Error>> enumerable  = func( connection, transaction, arg1, arg2, arg3, arg4, arg5, token );
        bool                                    passed      = true;

        await foreach ( OneOf<TResult, Error> result in enumerable.WithCancellation( token ) )
        {
            if ( result.IsT0 ) { yield return result.AsT0; }
            else
            {
                passed = false;
                yield return result.AsT1;
            }
        }

        if ( passed ) { await transaction.CommitAsync( token ); }
        else { await transaction.RollbackAsync( token ); }
    }
    public static async IAsyncEnumerable<OneOf<TResult, Error>> TryCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>( this IConnectableDb db,
                                                                                                                            Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, CancellationToken,
                                                                                                                                IAsyncEnumerable<OneOf<TResult, Error>>> func,
                                                                                                                            TArg1                                      arg1,
                                                                                                                            TArg2                                      arg2,
                                                                                                                            TArg3                                      arg3,
                                                                                                                            TArg4                                      arg4,
                                                                                                                            TArg5                                      arg5,
                                                                                                                            TArg6                                      arg6,
                                                                                                                            [EnumeratorCancellation] CancellationToken token
    )
    {
        await using DbConnection                connection  = await db.ConnectAsync( token );
        await using DbTransaction               transaction = await connection.BeginTransactionAsync( IsolationLevel.ReadCommitted, token );
        IAsyncEnumerable<OneOf<TResult, Error>> enumerable  = func( connection, transaction, arg1, arg2, arg3, arg4, arg5, arg6, token );
        bool                                    passed      = true;

        await foreach ( OneOf<TResult, Error> result in enumerable.WithCancellation( token ) )
        {
            if ( result.IsT0 ) { yield return result.AsT0; }
            else
            {
                passed = false;
                yield return result.AsT1;
            }
        }

        if ( passed ) { await transaction.CommitAsync( token ); }
        else { await transaction.RollbackAsync( token ); }
    }
    public static async IAsyncEnumerable<OneOf<TResult, Error>> TryCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>( this IConnectableDb db,
                                                                                                                                   Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, CancellationToken,
                                                                                                                                       IAsyncEnumerable<OneOf<TResult, Error>>> func,
                                                                                                                                   TArg1                                      arg1,
                                                                                                                                   TArg2                                      arg2,
                                                                                                                                   TArg3                                      arg3,
                                                                                                                                   TArg4                                      arg4,
                                                                                                                                   TArg5                                      arg5,
                                                                                                                                   TArg6                                      arg6,
                                                                                                                                   TArg7                                      arg7,
                                                                                                                                   [EnumeratorCancellation] CancellationToken token
    )
    {
        await using DbConnection                connection  = await db.ConnectAsync( token );
        await using DbTransaction               transaction = await connection.BeginTransactionAsync( IsolationLevel.ReadCommitted, token );
        IAsyncEnumerable<OneOf<TResult, Error>> enumerable  = func( connection, transaction, arg1, arg2, arg3, arg4, arg5, arg6, arg7, token );
        bool                                    passed      = true;

        await foreach ( OneOf<TResult, Error> result in enumerable.WithCancellation( token ) )
        {
            if ( result.IsT0 ) { yield return result.AsT0; }
            else
            {
                passed = false;
                yield return result.AsT1;
            }
        }

        if ( passed ) { await transaction.CommitAsync( token ); }
        else { await transaction.RollbackAsync( token ); }
    }
    public static async IAsyncEnumerable<OneOf<TResult, Error>> TryCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>( this IConnectableDb db,
                                                                                                                                          Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, CancellationToken,
                                                                                                                                              IAsyncEnumerable<OneOf<TResult, Error>>> func,
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
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( IsolationLevel.ReadCommitted, token );

        IAsyncEnumerable<OneOf<TResult, Error>> enumerable = func( connection,
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

        await foreach ( OneOf<TResult, Error> result in enumerable.WithCancellation( token ) )
        {
            if ( result.IsT0 ) { yield return result.AsT0; }
            else
            {
                passed = false;
                yield return result.AsT1;
            }
        }

        if ( passed ) { await transaction.CommitAsync( token ); }
        else { await transaction.RollbackAsync( token ); }
    }


    public static async ValueTask<OneOf<TResult, Error>> TryCall<TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction, CancellationToken, ValueTask<OneOf<TResult, Error>>> func, CancellationToken token = default )
    {
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( IsolationLevel.ReadCommitted, token );

        try
        {
            OneOf<TResult, Error> result = await func( connection, transaction, token );
            if ( result.IsT0 ) { await transaction.CommitAsync( token ); }

            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync( token );
            throw;
        }
    }
    public static async ValueTask<OneOf<TResult, Error>> TryCall<TArg1, TResult>( this IConnectableDb                                                                           db,
                                                                                  Func<DbConnection, DbTransaction, TArg1, CancellationToken, ValueTask<OneOf<TResult, Error>>> func,
                                                                                  TArg1                                                                                         arg1,
                                                                                  CancellationToken                                                                             token = default
    )
    {
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( IsolationLevel.ReadCommitted, token );

        try
        {
            OneOf<TResult, Error> result = await func( connection, transaction, arg1, token );
            if ( result.IsT0 ) { await transaction.CommitAsync( token ); }

            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync( token );
            throw;
        }
    }
    public static async ValueTask<OneOf<TResult, Error>> TryCall<TArg1, TArg2, TResult>( this IConnectableDb                                                                                  db,
                                                                                         Func<DbConnection, DbTransaction, TArg1, TArg2, CancellationToken, ValueTask<OneOf<TResult, Error>>> func,
                                                                                         TArg1                                                                                                arg1,
                                                                                         TArg2                                                                                                arg2,
                                                                                         CancellationToken                                                                                    token = default
    )
    {
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( IsolationLevel.ReadCommitted, token );

        try
        {
            OneOf<TResult, Error> result = await func( connection, transaction, arg1, arg2, token );
            if ( result.IsT0 ) { await transaction.CommitAsync( token ); }

            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync( token );
            throw;
        }
    }
    public static async ValueTask<OneOf<TResult, Error>> TryCall<TArg1, TArg2, TArg3, TResult>( this IConnectableDb                                                                                         db,
                                                                                                Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, CancellationToken, ValueTask<OneOf<TResult, Error>>> func,
                                                                                                TArg1                                                                                                       arg1,
                                                                                                TArg2                                                                                                       arg2,
                                                                                                TArg3                                                                                                       arg3,
                                                                                                CancellationToken                                                                                           token
    )
    {
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( IsolationLevel.ReadCommitted, token );

        try
        {
            OneOf<TResult, Error> result = await func( connection, transaction, arg1, arg2, arg3, token );
            if ( result.IsT0 ) { await transaction.CommitAsync( token ); }

            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync( token );
            throw;
        }
    }
    public static async ValueTask<OneOf<TResult, Error>> TryCall<TArg1, TArg2, TArg3, TArg4, TResult>( this IConnectableDb                                                                                                db,
                                                                                                       Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, CancellationToken, ValueTask<OneOf<TResult, Error>>> func,
                                                                                                       TArg1                                                                                                              arg1,
                                                                                                       TArg2                                                                                                              arg2,
                                                                                                       TArg3                                                                                                              arg3,
                                                                                                       TArg4                                                                                                              arg4,
                                                                                                       CancellationToken                                                                                                  token
    )
    {
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( IsolationLevel.ReadCommitted, token );

        try
        {
            OneOf<TResult, Error> result = await func( connection, transaction, arg1, arg2, arg3, arg4, token );
            if ( result.IsT0 ) { await transaction.CommitAsync( token ); }

            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync( token );
            throw;
        }
    }
    public static async ValueTask<OneOf<TResult, Error>> TryCall<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>( this IConnectableDb                                                                                                       db,
                                                                                                              Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, CancellationToken, ValueTask<OneOf<TResult, Error>>> func,
                                                                                                              TArg1                                                                                                                     arg1,
                                                                                                              TArg2                                                                                                                     arg2,
                                                                                                              TArg3                                                                                                                     arg3,
                                                                                                              TArg4                                                                                                                     arg4,
                                                                                                              TArg5                                                                                                                     arg5,
                                                                                                              CancellationToken                                                                                                         token
    )
    {
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( IsolationLevel.ReadCommitted, token );

        try
        {
            OneOf<TResult, Error> result = await func( connection, transaction, arg1, arg2, arg3, arg4, arg5, token );
            if ( result.IsT0 ) { await transaction.CommitAsync( token ); }

            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync( token );
            throw;
        }
    }
    public static async ValueTask<OneOf<TResult, Error>> TryCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>( this IConnectableDb db,
                                                                                                                     Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, CancellationToken, ValueTask<OneOf<TResult, Error>>>
                                                                                                                         func,
                                                                                                                     TArg1             arg1,
                                                                                                                     TArg2             arg2,
                                                                                                                     TArg3             arg3,
                                                                                                                     TArg4             arg4,
                                                                                                                     TArg5             arg5,
                                                                                                                     TArg6             arg6,
                                                                                                                     CancellationToken token
    )
    {
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( IsolationLevel.ReadCommitted, token );

        try
        {
            OneOf<TResult, Error> result = await func( connection, transaction, arg1, arg2, arg3, arg4, arg5, arg6, token );
            if ( result.IsT0 ) { await transaction.CommitAsync( token ); }

            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync( token );
            throw;
        }
    }
    public static async ValueTask<OneOf<TResult, Error>> TryCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>( this IConnectableDb db,
                                                                                                                            Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, CancellationToken,
                                                                                                                                ValueTask<OneOf<TResult, Error>>> func,
                                                                                                                            TArg1             arg1,
                                                                                                                            TArg2             arg2,
                                                                                                                            TArg3             arg3,
                                                                                                                            TArg4             arg4,
                                                                                                                            TArg5             arg5,
                                                                                                                            TArg6             arg6,
                                                                                                                            TArg7             arg7,
                                                                                                                            CancellationToken token
    )
    {
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( IsolationLevel.ReadCommitted, token );

        try
        {
            OneOf<TResult, Error> result = await func( connection, transaction, arg1, arg2, arg3, arg4, arg5, arg6, arg7, token );
            if ( result.IsT0 ) { await transaction.CommitAsync( token ); }

            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync( token );
            throw;
        }
    }
    public static async ValueTask<OneOf<TResult, Error>> TryCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>( this IConnectableDb db,
                                                                                                                                   Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, CancellationToken,
                                                                                                                                       ValueTask<OneOf<TResult, Error>>> func,
                                                                                                                                   TArg1             arg1,
                                                                                                                                   TArg2             arg2,
                                                                                                                                   TArg3             arg3,
                                                                                                                                   TArg4             arg4,
                                                                                                                                   TArg5             arg5,
                                                                                                                                   TArg6             arg6,
                                                                                                                                   TArg7             arg7,
                                                                                                                                   TArg8             arg8,
                                                                                                                                   CancellationToken token
    )
    {
        await using DbConnection  connection  = await db.ConnectAsync( token );
        await using DbTransaction transaction = await connection.BeginTransactionAsync( IsolationLevel.ReadCommitted, token );

        try
        {
            OneOf<TResult, Error> result = await func( connection,
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

            if ( result.IsT0 ) { await transaction.CommitAsync( token ); }

            return result;
        }
        catch ( Exception )
        {
            await transaction.RollbackAsync( token );
            throw;
        }
    }
}
