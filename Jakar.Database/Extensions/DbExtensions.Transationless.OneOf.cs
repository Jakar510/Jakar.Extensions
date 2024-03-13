namespace Jakar.Database;


public static partial class DbExtensions
{
    public static async IAsyncEnumerable<OneOf<TResult, Error>> Call<TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, CancellationToken, IAsyncEnumerable<OneOf<TResult, Error>>> func, [EnumeratorCancellation] CancellationToken token = default )
    {
        await using DbConnection                connection = await db.ConnectAsync( token );
        IAsyncEnumerable<OneOf<TResult, Error>> result     = func( connection, default, token );

        await foreach ( OneOf<TResult, Error> item in result.WithCancellation( token ) ) { yield return item; }
    }
    public static async IAsyncEnumerable<OneOf<TResult, Error>> Call<TArg1, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, TArg1, CancellationToken, IAsyncEnumerable<OneOf<TResult, Error>>> func, TArg1 arg1, [EnumeratorCancellation] CancellationToken token )
    {
        await using DbConnection                connection = await db.ConnectAsync( token );
        IAsyncEnumerable<OneOf<TResult, Error>> result     = func( connection, default, arg1, token );

        await foreach ( OneOf<TResult, Error> item in result.WithCancellation( token ) ) { yield return item; }
    }
    public static async IAsyncEnumerable<OneOf<TResult, Error>> Call<TArg1, TArg2, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, TArg1, TArg2, CancellationToken, IAsyncEnumerable<OneOf<TResult, Error>>> func, TArg1 arg1, TArg2 arg2, [EnumeratorCancellation] CancellationToken token )
    {
        await using DbConnection                connection = await db.ConnectAsync( token );
        IAsyncEnumerable<OneOf<TResult, Error>> result     = func( connection, default, arg1, arg2, token );

        await foreach ( OneOf<TResult, Error> item in result.WithCancellation( token ) ) { yield return item; }
    }
    public static async IAsyncEnumerable<OneOf<TResult, Error>> Call<TArg1, TArg2, TArg3, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, TArg1, TArg2, TArg3, CancellationToken, IAsyncEnumerable<OneOf<TResult, Error>>> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, [EnumeratorCancellation] CancellationToken token )
    {
        await using DbConnection                connection = await db.ConnectAsync( token );
        IAsyncEnumerable<OneOf<TResult, Error>> result     = func( connection, default, arg1, arg2, arg3, token );

        await foreach ( OneOf<TResult, Error> item in result.WithCancellation( token ) ) { yield return item; }
    }
    public static async IAsyncEnumerable<OneOf<TResult, Error>> Call<TArg1, TArg2, TArg3, TArg4, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, TArg1, TArg2, TArg3, TArg4, CancellationToken, IAsyncEnumerable<OneOf<TResult, Error>>> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, [EnumeratorCancellation] CancellationToken token )
    {
        await using DbConnection                connection = await db.ConnectAsync( token );
        IAsyncEnumerable<OneOf<TResult, Error>> result     = func( connection, default, arg1, arg2, arg3, arg4, token );

        await foreach ( OneOf<TResult, Error> item in result.WithCancellation( token ) ) { yield return item; }
    }
    public static async IAsyncEnumerable<OneOf<TResult, Error>> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, TArg1, TArg2, TArg3, TArg4, TArg5, CancellationToken, IAsyncEnumerable<OneOf<TResult, Error>>> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, [EnumeratorCancellation] CancellationToken token )
    {
        await using DbConnection                connection = await db.ConnectAsync( token );
        IAsyncEnumerable<OneOf<TResult, Error>> result     = func( connection, default, arg1, arg2, arg3, arg4, arg5, token );

        await foreach ( OneOf<TResult, Error> item in result.WithCancellation( token ) ) { yield return item; }
    }
    public static async IAsyncEnumerable<OneOf<TResult, Error>> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>( this IConnectableDb                                                                                                                      db,
                                                                                                                         Func<DbConnection, DbTransaction?, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, CancellationToken, IAsyncEnumerable<OneOf<TResult, Error>>> func,
                                                                                                                         TArg1                                                                                                                                    arg1,
                                                                                                                         TArg2                                                                                                                                    arg2,
                                                                                                                         TArg3                                                                                                                                    arg3,
                                                                                                                         TArg4                                                                                                                                    arg4,
                                                                                                                         TArg5                                                                                                                                    arg5,
                                                                                                                         TArg6                                                                                                                                    arg6,
                                                                                                                         [EnumeratorCancellation] CancellationToken                                                                                               token
    )
    {
        await using DbConnection                connection = await db.ConnectAsync( token );
        IAsyncEnumerable<OneOf<TResult, Error>> result     = func( connection, default, arg1, arg2, arg3, arg4, arg5, arg6, token );

        await foreach ( OneOf<TResult, Error> item in result.WithCancellation( token ) ) { yield return item; }
    }
    public static async IAsyncEnumerable<OneOf<TResult, Error>> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>( this IConnectableDb                                                                                                                             db,
                                                                                                                                Func<DbConnection, DbTransaction?, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, CancellationToken, IAsyncEnumerable<OneOf<TResult, Error>>> func,
                                                                                                                                TArg1                                                                                                                                           arg1,
                                                                                                                                TArg2                                                                                                                                           arg2,
                                                                                                                                TArg3                                                                                                                                           arg3,
                                                                                                                                TArg4                                                                                                                                           arg4,
                                                                                                                                TArg5                                                                                                                                           arg5,
                                                                                                                                TArg6                                                                                                                                           arg6,
                                                                                                                                TArg7                                                                                                                                           arg7,
                                                                                                                                [EnumeratorCancellation] CancellationToken                                                                                                      token
    )
    {
        await using DbConnection                connection = await db.ConnectAsync( token );
        IAsyncEnumerable<OneOf<TResult, Error>> result     = func( connection, default, arg1, arg2, arg3, arg4, arg5, arg6, arg7, token );

        await foreach ( OneOf<TResult, Error> item in result.WithCancellation( token ) ) { yield return item; }
    }
    public static async IAsyncEnumerable<OneOf<TResult, Error>> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>( this IConnectableDb                                                                                                                                    db,
                                                                                                                                       Func<DbConnection, DbTransaction?, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, CancellationToken, IAsyncEnumerable<OneOf<TResult, Error>>> func,
                                                                                                                                       TArg1                                                                                                                                                  arg1,
                                                                                                                                       TArg2                                                                                                                                                  arg2,
                                                                                                                                       TArg3                                                                                                                                                  arg3,
                                                                                                                                       TArg4                                                                                                                                                  arg4,
                                                                                                                                       TArg5                                                                                                                                                  arg5,
                                                                                                                                       TArg6                                                                                                                                                  arg6,
                                                                                                                                       TArg7                                                                                                                                                  arg7,
                                                                                                                                       TArg8                                                                                                                                                  arg8,
                                                                                                                                       [EnumeratorCancellation] CancellationToken                                                                                                             token
    )
    {
        await using DbConnection connection = await db.ConnectAsync( token );

        IAsyncEnumerable<OneOf<TResult, Error>> result = func( connection,
                                                               default,
                                                               arg1,
                                                               arg2,
                                                               arg3,
                                                               arg4,
                                                               arg5,
                                                               arg6,
                                                               arg7,
                                                               arg8,
                                                               token );


        await foreach ( OneOf<TResult, Error> item in result.WithCancellation( token ) ) { yield return item; }
    }


    public static async ValueTask<OneOf<TResult, Error>> Call<TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, CancellationToken, ValueTask<OneOf<TResult, Error>>> func, CancellationToken token = default )
    {
        await using DbConnection connection = await db.ConnectAsync( token );
        return await func( connection, default, token );
    }
    public static async ValueTask<OneOf<TResult, Error>> Call<TArg1, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, TArg1, CancellationToken, ValueTask<OneOf<TResult, Error>>> func, TArg1 arg1, CancellationToken token = default )
    {
        await using DbConnection connection = await db.ConnectAsync( token );
        return await func( connection, default, arg1, token );
    }
    public static async ValueTask<OneOf<TResult, Error>> Call<TArg1, TArg2, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, TArg1, TArg2, CancellationToken, ValueTask<OneOf<TResult, Error>>> func, TArg1 arg1, TArg2 arg2, CancellationToken token = default )
    {
        await using DbConnection connection = await db.ConnectAsync( token );
        return await func( connection, default, arg1, arg2, token );
    }
    public static async ValueTask<OneOf<TResult, Error>> Call<TArg1, TArg2, TArg3, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, TArg1, TArg2, TArg3, CancellationToken, ValueTask<OneOf<TResult, Error>>> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, CancellationToken token )
    {
        await using DbConnection connection = await db.ConnectAsync( token );
        return await func( connection, default, arg1, arg2, arg3, token );
    }
    public static async ValueTask<OneOf<TResult, Error>> Call<TArg1, TArg2, TArg3, TArg4, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, TArg1, TArg2, TArg3, TArg4, CancellationToken, ValueTask<OneOf<TResult, Error>>> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, CancellationToken token )
    {
        await using DbConnection connection = await db.ConnectAsync( token );
        return await func( connection, default, arg1, arg2, arg3, arg4, token );
    }
    public static async ValueTask<OneOf<TResult, Error>> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, TArg1, TArg2, TArg3, TArg4, TArg5, CancellationToken, ValueTask<OneOf<TResult, Error>>> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, CancellationToken token )
    {
        await using DbConnection connection = await db.ConnectAsync( token );
        return await func( connection, default, arg1, arg2, arg3, arg4, arg5, token );
    }
    public static async ValueTask<OneOf<TResult, Error>> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, CancellationToken, ValueTask<OneOf<TResult, Error>>> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, CancellationToken token )
    {
        await using DbConnection connection = await db.ConnectAsync( token );
        return await func( connection, default, arg1, arg2, arg3, arg4, arg5, arg6, token );
    }
    public static async ValueTask<OneOf<TResult, Error>> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, CancellationToken, ValueTask<OneOf<TResult, Error>>> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, CancellationToken token )
    {
        await using DbConnection connection = await db.ConnectAsync( token );
        return await func( connection, default, arg1, arg2, arg3, arg4, arg5, arg6, arg7, token );
    }
    public static async ValueTask<OneOf<TResult, Error>> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>( this IConnectableDb                                                                                                                             db,
                                                                                                                                Func<DbConnection, DbTransaction?, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, CancellationToken, ValueTask<OneOf<TResult, Error>>> func,
                                                                                                                                TArg1                                                                                                                                           arg1,
                                                                                                                                TArg2                                                                                                                                           arg2,
                                                                                                                                TArg3                                                                                                                                           arg3,
                                                                                                                                TArg4                                                                                                                                           arg4,
                                                                                                                                TArg5                                                                                                                                           arg5,
                                                                                                                                TArg6                                                                                                                                           arg6,
                                                                                                                                TArg7                                                                                                                                           arg7,
                                                                                                                                TArg8                                                                                                                                           arg8,
                                                                                                                                CancellationToken                                                                                                                               token
    )
    {
        await using DbConnection connection = await db.ConnectAsync( token );

        return await func( connection,
                           default,
                           arg1,
                           arg2,
                           arg3,
                           arg4,
                           arg5,
                           arg6,
                           arg7,
                           arg8,
                           token );
    }
}
