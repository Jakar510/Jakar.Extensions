// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:39 PM

using Dapper.Contrib.Extensions;



namespace Jakar.Database;


public abstract class Database<TID> : ObservableClass, IAsyncDisposable where TID : IComparable<TID>, IEquatable<TID>
{
    protected readonly HashSet<IAsyncDisposable> _disposables = new();


    protected Database() : base() { }
    public virtual async ValueTask DisposeAsync()
    {
        foreach ( IAsyncDisposable disposable in _disposables ) { await disposable.DisposeAsync(); }

        _disposables.Clear();
    }


    public static string GetTableName<TRecord>() where TRecord : class => GetTableName(typeof(TRecord));
    public static string GetTableName( in Type type ) => type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;


    protected TValue AddDisposable<TValue>( TValue value ) where TValue : IAsyncDisposable
    {
        _disposables.Add(value);
        return value;
    }
    protected abstract TTable CreateTable<TTable, TRecord>() where TTable : IDbTable<TRecord, TID>
                                                             where TRecord : BaseTableRecord<TRecord, TID>;


    public abstract DbConnection Connect();
    public abstract Task<DbConnection> ConnectAsync( CancellationToken token );


    public async Task<TResult> Call<TResult>( Func<DbConnection, DbTransaction, CancellationToken, Task<TResult>> func, CancellationToken token )
    {
        await using DbConnection  conn        = await ConnectAsync(token);
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
    public async Task<TResult> Call<TArg1, TResult>( Func<DbConnection, DbTransaction, TArg1, CancellationToken, Task<TResult>> func, TArg1 arg1, CancellationToken token )
    {
        await using DbConnection  conn        = await ConnectAsync(token);
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
    public async Task<TResult> Call<TArg1, TArg2, TResult>( Func<DbConnection, DbTransaction, TArg1, TArg2, CancellationToken, Task<TResult>> func, TArg1 arg1, TArg2 arg2, CancellationToken token )
    {
        await using DbConnection  conn        = await ConnectAsync(token);
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
    public async Task<TResult> Call<TArg1, TArg2, TArg3, TResult>( Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, CancellationToken, Task<TResult>> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, CancellationToken token )
    {
        await using DbConnection  conn        = await ConnectAsync(token);
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
    public async Task<TResult> Call<TArg1, TArg2, TArg3, TArg4, TResult>( Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, CancellationToken, Task<TResult>> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, CancellationToken token )
    {
        await using DbConnection  conn        = await ConnectAsync(token);
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
    public async Task<TResult> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>( Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, CancellationToken, Task<TResult>> func,
                                                                                 TArg1                                                                                                  arg1,
                                                                                 TArg2                                                                                                  arg2,
                                                                                 TArg3                                                                                                  arg3,
                                                                                 TArg4                                                                                                  arg4,
                                                                                 TArg5                                                                                                  arg5,
                                                                                 CancellationToken                                                                                      token
    )
    {
        await using DbConnection  conn        = await ConnectAsync(token);
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
    public async Task<TResult> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>( Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, CancellationToken, Task<TResult>> func,
                                                                                        TArg1                                                                                                         arg1,
                                                                                        TArg2                                                                                                         arg2,
                                                                                        TArg3                                                                                                         arg3,
                                                                                        TArg4                                                                                                         arg4,
                                                                                        TArg5                                                                                                         arg5,
                                                                                        TArg6                                                                                                         arg6,
                                                                                        CancellationToken                                                                                             token
    )
    {
        await using DbConnection  conn        = await ConnectAsync(token);
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
    public async Task<TResult> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>( Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, CancellationToken, Task<TResult>> func,
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
        await using DbConnection  conn        = await ConnectAsync(token);
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
    public async Task<TResult> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>( Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, CancellationToken, Task<TResult>> func,
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
        await using DbConnection  conn        = await ConnectAsync(token);
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


    public async IAsyncEnumerable<TResult> Call<TResult>( Func<DbConnection, DbTransaction, CancellationToken, IAsyncEnumerable<TResult>> func, [EnumeratorCancellation] CancellationToken token )
    {
        await using DbConnection  conn        = await ConnectAsync(token);
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
    public async IAsyncEnumerable<TResult> Call<TArg1, TResult>( Func<DbConnection, DbTransaction, TArg1, CancellationToken, IAsyncEnumerable<TResult>> func, TArg1 arg1, [EnumeratorCancellation] CancellationToken token )
    {
        await using DbConnection  conn        = await ConnectAsync(token);
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
    public async IAsyncEnumerable<TResult> Call<TArg1, TArg2, TResult>( Func<DbConnection, DbTransaction, TArg1, TArg2, CancellationToken, IAsyncEnumerable<TResult>> func, TArg1 arg1, TArg2 arg2, [EnumeratorCancellation] CancellationToken token )
    {
        await using DbConnection  conn        = await ConnectAsync(token);
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
    public async IAsyncEnumerable<TResult> Call<TArg1, TArg2, TArg3, TResult>( Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, CancellationToken, IAsyncEnumerable<TResult>> func,
                                                                               TArg1                                                                                                arg1,
                                                                               TArg2                                                                                                arg2,
                                                                               TArg3                                                                                                arg3,
                                                                               [EnumeratorCancellation] CancellationToken                                                           token
    )
    {
        await using DbConnection  conn        = await ConnectAsync(token);
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
    public async IAsyncEnumerable<TResult> Call<TArg1, TArg2, TArg3, TArg4, TResult>( Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, CancellationToken, IAsyncEnumerable<TResult>> func,
                                                                                      TArg1                                                                                                       arg1,
                                                                                      TArg2                                                                                                       arg2,
                                                                                      TArg3                                                                                                       arg3,
                                                                                      TArg4                                                                                                       arg4,
                                                                                      [EnumeratorCancellation] CancellationToken                                                                  token
    )
    {
        await using DbConnection  conn        = await ConnectAsync(token);
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
    public async IAsyncEnumerable<TResult> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>( Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, CancellationToken, IAsyncEnumerable<TResult>> func,
                                                                                             TArg1                                                                                                              arg1,
                                                                                             TArg2                                                                                                              arg2,
                                                                                             TArg3                                                                                                              arg3,
                                                                                             TArg4                                                                                                              arg4,
                                                                                             TArg5                                                                                                              arg5,
                                                                                             [EnumeratorCancellation] CancellationToken                                                                         token
    )
    {
        await using DbConnection  conn        = await ConnectAsync(token);
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
    public async IAsyncEnumerable<TResult> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>( Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, CancellationToken, IAsyncEnumerable<TResult>> func,
                                                                                                    TArg1                                                                                                                     arg1,
                                                                                                    TArg2                                                                                                                     arg2,
                                                                                                    TArg3                                                                                                                     arg3,
                                                                                                    TArg4                                                                                                                     arg4,
                                                                                                    TArg5                                                                                                                     arg5,
                                                                                                    TArg6                                                                                                                     arg6,
                                                                                                    [EnumeratorCancellation] CancellationToken                                                                                token
    )
    {
        await using DbConnection  conn        = await ConnectAsync(token);
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
    public async IAsyncEnumerable<TResult> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>( Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, CancellationToken, IAsyncEnumerable<TResult>> func,
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
        await using DbConnection  conn        = await ConnectAsync(token);
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
    public async IAsyncEnumerable<TResult> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(
        Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, CancellationToken, IAsyncEnumerable<TResult>> func,
        TArg1                                                                                                                                   arg1,
        TArg2                                                                                                                                   arg2,
        TArg3                                                                                                                                   arg3,
        TArg4                                                                                                                                   arg4,
        TArg5                                                                                                                                   arg5,
        TArg6                                                                                                                                   arg6,
        TArg7                                                                                                                                   arg7,
        TArg8                                                                                                                                   arg8,
        [EnumeratorCancellation] CancellationToken                                                                                              token
    )
    {
        await using DbConnection  conn        = await ConnectAsync(token);
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



public abstract class Database<TDatabase, TID> : Database<TID>, IEquatable<TDatabase>, IComparable<TDatabase>, ICloneable where TDatabase : Database<TDatabase, TID>
                                                                                                                          where TID : IComparable<TID>, IEquatable<TID>
{
    protected Database() : base() { }


    public abstract bool Equals( TDatabase?   other );
    public abstract int CompareTo( TDatabase? other );
    public abstract TDatabase Clone();
    object ICloneable.Clone() => Clone();
}
