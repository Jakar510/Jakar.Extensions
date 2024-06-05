namespace Jakar.Database;


public static partial class DbExtensions
{
    public static async IAsyncEnumerable<ErrorOrResult<TResult>> Call<TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, Activity?, CancellationToken, IAsyncEnumerable<ErrorOrResult<TResult>>> func, Activity? activity, [EnumeratorCancellation] CancellationToken token )
    {
        await using DbConnection                 connection = await db.ConnectAsync( token );
        IAsyncEnumerable<ErrorOrResult<TResult>> enumerable = func( connection, null, activity, token );

        await foreach ( ErrorOrResult<TResult> result in enumerable.WithCancellation( token ) ) { yield return result; }
    }
    public static async IAsyncEnumerable<ErrorOrResult<TResult>> Call<TArg1, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, Activity?, TArg1, CancellationToken, IAsyncEnumerable<ErrorOrResult<TResult>>> func, Activity? activity, TArg1 arg1, [EnumeratorCancellation] CancellationToken token )
    {
        await using DbConnection                 connection = await db.ConnectAsync( token );
        IAsyncEnumerable<ErrorOrResult<TResult>> enumerable = func( connection, null, activity, arg1, token );

        await foreach ( ErrorOrResult<TResult> result in enumerable.WithCancellation( token ) ) { yield return result; }
    }
    public static async IAsyncEnumerable<ErrorOrResult<TResult>> Call<TArg1, TArg2, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, Activity?, TArg1, TArg2, CancellationToken, IAsyncEnumerable<ErrorOrResult<TResult>>> func, Activity? activity, TArg1 arg1, TArg2 arg2, [EnumeratorCancellation] CancellationToken token )
    {
        await using DbConnection                 connection = await db.ConnectAsync( token );
        IAsyncEnumerable<ErrorOrResult<TResult>> enumerable = func( connection, null, activity, arg1, arg2, token );

        await foreach ( ErrorOrResult<TResult> result in enumerable.WithCancellation( token ) ) { yield return result; }
    }
    public static async IAsyncEnumerable<ErrorOrResult<TResult>> Call<TArg1, TArg2, TArg3, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, Activity?, TArg1, TArg2, TArg3, CancellationToken, IAsyncEnumerable<ErrorOrResult<TResult>>> func, Activity? activity, TArg1 arg1, TArg2 arg2, TArg3 arg3, [EnumeratorCancellation] CancellationToken token )
    {
        await using DbConnection                 connection = await db.ConnectAsync( token );
        IAsyncEnumerable<ErrorOrResult<TResult>> enumerable = func( connection, null, activity, arg1, arg2, arg3, token );

        await foreach ( ErrorOrResult<TResult> result in enumerable.WithCancellation( token ) ) { yield return result; }
    }
    public static async IAsyncEnumerable<ErrorOrResult<TResult>> Call<TArg1, TArg2, TArg3, TArg4, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, Activity?, TArg1, TArg2, TArg3, TArg4, CancellationToken, IAsyncEnumerable<ErrorOrResult<TResult>>> func, Activity? activity, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, [EnumeratorCancellation] CancellationToken token )
    {
        await using DbConnection                 connection = await db.ConnectAsync( token );
        IAsyncEnumerable<ErrorOrResult<TResult>> enumerable = func( connection, null, activity, arg1, arg2, arg3, arg4, token );

        await foreach ( ErrorOrResult<TResult> result in enumerable.WithCancellation( token ) ) { yield return result; }
    }
    public static async IAsyncEnumerable<ErrorOrResult<TResult>> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, Activity?, TArg1, TArg2, TArg3, TArg4, TArg5, CancellationToken, IAsyncEnumerable<ErrorOrResult<TResult>>> func, Activity? activity, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, [EnumeratorCancellation] CancellationToken token )
    {
        await using DbConnection                 connection = await db.ConnectAsync( token );
        IAsyncEnumerable<ErrorOrResult<TResult>> enumerable = func( connection, null, activity, arg1, arg2, arg3, arg4, arg5, token );

        await foreach ( ErrorOrResult<TResult> result in enumerable.WithCancellation( token ) ) { yield return result; }
    }
    public static async IAsyncEnumerable<ErrorOrResult<TResult>> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, Activity?, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, CancellationToken, IAsyncEnumerable<ErrorOrResult<TResult>>> func, Activity? activity, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, [EnumeratorCancellation] CancellationToken token )
    {
        await using DbConnection                 connection = await db.ConnectAsync( token );
        IAsyncEnumerable<ErrorOrResult<TResult>> enumerable = func( connection, null, activity, arg1, arg2, arg3, arg4, arg5, arg6, token );

        await foreach ( ErrorOrResult<TResult> result in enumerable.WithCancellation( token ) ) { yield return result; }
    }
    public static async IAsyncEnumerable<ErrorOrResult<TResult>> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>( this IConnectableDb                                                                                                                                         db,
                                                                                                                                 Func<DbConnection, DbTransaction?, Activity?, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, CancellationToken, IAsyncEnumerable<ErrorOrResult<TResult>>> func,
                                                                                                                                 Activity?                                                                                                                                                   activity,
                                                                                                                                 TArg1                                                                                                                                                       arg1,
                                                                                                                                 TArg2                                                                                                                                                       arg2,
                                                                                                                                 TArg3                                                                                                                                                       arg3,
                                                                                                                                 TArg4                                                                                                                                                       arg4,
                                                                                                                                 TArg5                                                                                                                                                       arg5,
                                                                                                                                 TArg6                                                                                                                                                       arg6,
                                                                                                                                 TArg7                                                                                                                                                       arg7,
                                                                                                                                 [EnumeratorCancellation] CancellationToken                                                                                                                  token
    )
    {
        await using DbConnection connection = await db.ConnectAsync( token );

        IAsyncEnumerable<ErrorOrResult<TResult>> enumerable = func( connection,
                                                                    null,
                                                                    activity,
                                                                    arg1,
                                                                    arg2,
                                                                    arg3,
                                                                    arg4,
                                                                    arg5,
                                                                    arg6,
                                                                    arg7,
                                                                    token );

        await foreach ( ErrorOrResult<TResult> result in enumerable.WithCancellation( token ) ) { yield return result; }
    }
    public static async IAsyncEnumerable<ErrorOrResult<TResult>> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>( this IConnectableDb                                                                                                                                                db,
                                                                                                                                        Func<DbConnection, DbTransaction?, Activity?, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, CancellationToken, IAsyncEnumerable<ErrorOrResult<TResult>>> func,
                                                                                                                                        Activity?                                                                                                                                                          activity,
                                                                                                                                        TArg1                                                                                                                                                              arg1,
                                                                                                                                        TArg2                                                                                                                                                              arg2,
                                                                                                                                        TArg3                                                                                                                                                              arg3,
                                                                                                                                        TArg4                                                                                                                                                              arg4,
                                                                                                                                        TArg5                                                                                                                                                              arg5,
                                                                                                                                        TArg6                                                                                                                                                              arg6,
                                                                                                                                        TArg7                                                                                                                                                              arg7,
                                                                                                                                        TArg8                                                                                                                                                              arg8,
                                                                                                                                        [EnumeratorCancellation] CancellationToken                                                                                                                         token
    )
    {
        await using DbConnection connection = await db.ConnectAsync( token );

        IAsyncEnumerable<ErrorOrResult<TResult>> enumerable = func( connection,
                                                                    null,
                                                                    activity,
                                                                    arg1,
                                                                    arg2,
                                                                    arg3,
                                                                    arg4,
                                                                    arg5,
                                                                    arg6,
                                                                    arg7,
                                                                    arg8,
                                                                    token );


        await foreach ( ErrorOrResult<TResult> result in enumerable.WithCancellation( token ) ) { yield return result; }
    }


    public static async ValueTask<ErrorOrResult<TResult>> Call<TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, Activity?, CancellationToken, ValueTask<ErrorOrResult<TResult>>> func, Activity? activity, CancellationToken token )
    {
        await using DbConnection connection = await db.ConnectAsync( token );
        return await func( connection, null, activity, token );
    }
    public static async ValueTask<ErrorOrResult<TResult>> Call<TArg1, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, Activity?, TArg1, CancellationToken, ValueTask<ErrorOrResult<TResult>>> func, Activity? activity, TArg1 arg1, CancellationToken token )
    {
        await using DbConnection connection = await db.ConnectAsync( token );
        return await func( connection, null, activity, arg1, token );
    }
    public static async ValueTask<ErrorOrResult<TResult>> Call<TArg1, TArg2, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, Activity?, TArg1, TArg2, CancellationToken, ValueTask<ErrorOrResult<TResult>>> func, Activity? activity, TArg1 arg1, TArg2 arg2, CancellationToken token )
    {
        await using DbConnection connection = await db.ConnectAsync( token );
        return await func( connection, null, activity, arg1, arg2, token );
    }
    public static async ValueTask<ErrorOrResult<TResult>> Call<TArg1, TArg2, TArg3, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, Activity?, TArg1, TArg2, TArg3, CancellationToken, ValueTask<ErrorOrResult<TResult>>> func, Activity? activity, TArg1 arg1, TArg2 arg2, TArg3 arg3, CancellationToken token )
    {
        await using DbConnection connection = await db.ConnectAsync( token );
        return await func( connection, null, activity, arg1, arg2, arg3, token );
    }
    public static async ValueTask<ErrorOrResult<TResult>> Call<TArg1, TArg2, TArg3, TArg4, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, Activity?, TArg1, TArg2, TArg3, TArg4, CancellationToken, ValueTask<ErrorOrResult<TResult>>> func, Activity? activity, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, CancellationToken token )
    {
        await using DbConnection connection = await db.ConnectAsync( token );
        return await func( connection, null, activity, arg1, arg2, arg3, arg4, token );
    }
    public static async ValueTask<ErrorOrResult<TResult>> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, Activity?, TArg1, TArg2, TArg3, TArg4, TArg5, CancellationToken, ValueTask<ErrorOrResult<TResult>>> func, Activity? activity, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, CancellationToken token )
    {
        await using DbConnection connection = await db.ConnectAsync( token );
        return await func( connection, null, activity, arg1, arg2, arg3, arg4, arg5, token );
    }
    public static async ValueTask<ErrorOrResult<TResult>> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>( this IConnectableDb db, Func<DbConnection, DbTransaction?, Activity?, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, CancellationToken, ValueTask<ErrorOrResult<TResult>>> func, Activity? activity, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, CancellationToken token )
    {
        await using DbConnection connection = await db.ConnectAsync( token );
        return await func( connection, null, activity, arg1, arg2, arg3, arg4, arg5, arg6, token );
    }
    public static async ValueTask<ErrorOrResult<TResult>> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>( this IConnectableDb                                                                                                                                  db,
                                                                                                                          Func<DbConnection, DbTransaction?, Activity?, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, CancellationToken, ValueTask<ErrorOrResult<TResult>>> func,
                                                                                                                          Activity?                                                                                                                                            activity,
                                                                                                                          TArg1                                                                                                                                                arg1,
                                                                                                                          TArg2                                                                                                                                                arg2,
                                                                                                                          TArg3                                                                                                                                                arg3,
                                                                                                                          TArg4                                                                                                                                                arg4,
                                                                                                                          TArg5                                                                                                                                                arg5,
                                                                                                                          TArg6                                                                                                                                                arg6,
                                                                                                                          TArg7                                                                                                                                                arg7,
                                                                                                                          CancellationToken                                                                                                                                    token
    )
    {
        await using DbConnection connection = await db.ConnectAsync( token );

        return await func( connection,
                           null,
                           activity,
                           arg1,
                           arg2,
                           arg3,
                           arg4,
                           arg5,
                           arg6,
                           arg7,
                           token );
    }
    public static async ValueTask<ErrorOrResult<TResult>> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>( this IConnectableDb                                                                                                                                         db,
                                                                                                                                 Func<DbConnection, DbTransaction?, Activity?, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, CancellationToken, ValueTask<ErrorOrResult<TResult>>> func,
                                                                                                                                 Activity?                                                                                                                                                   activity,
                                                                                                                                 TArg1                                                                                                                                                       arg1,
                                                                                                                                 TArg2                                                                                                                                                       arg2,
                                                                                                                                 TArg3                                                                                                                                                       arg3,
                                                                                                                                 TArg4                                                                                                                                                       arg4,
                                                                                                                                 TArg5                                                                                                                                                       arg5,
                                                                                                                                 TArg6                                                                                                                                                       arg6,
                                                                                                                                 TArg7                                                                                                                                                       arg7,
                                                                                                                                 TArg8                                                                                                                                                       arg8,
                                                                                                                                 CancellationToken                                                                                                                                           token
    )
    {
        await using DbConnection connection = await db.ConnectAsync( token );

        return await func( connection,
                           null,
                           activity,
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
