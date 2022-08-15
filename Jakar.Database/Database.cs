// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:39 PM

namespace Jakar.Database;


public abstract class Database : ObservableClass, IAsyncDisposable
{
    protected Database() : base() { }
    public virtual ValueTask DisposeAsync() => default;


    protected abstract TTable CreateTable<TTable, TRecord>() where TTable : DbTable<TRecord>
                                                             where TRecord : BaseTableRecord<TRecord>;


    public abstract DbConnection Connect();
    public abstract Task<DbConnection> ConnectAsync( CancellationToken token );


    protected async Task<TResult> Call<TResult>( Func<DbConnection, DbTransaction, CancellationToken, Task<TResult>> func, CancellationToken token )
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
    protected async Task<TResult> Call<TArg1, TResult>( Func<DbConnection, DbTransaction, TArg1, CancellationToken, Task<TResult>> func, TArg1 arg1, CancellationToken token )
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
    protected async Task<TResult> Call<TArg1, TArg2, TResult>( Func<DbConnection, DbTransaction, TArg1, TArg2, CancellationToken, Task<TResult>> func, TArg1 arg1, TArg2 arg2, CancellationToken token )
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
    protected async Task<TResult> Call<TArg1, TArg2, TArg3, TResult>( Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, CancellationToken, Task<TResult>> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, CancellationToken token )
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
    protected async Task<TResult> Call<TArg1, TArg2, TArg3, TArg4, TResult>( Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, CancellationToken, Task<TResult>> func,
                                                                             TArg1                                                                                           arg1,
                                                                             TArg2                                                                                           arg2,
                                                                             TArg3                                                                                           arg3,
                                                                             TArg4                                                                                           arg4,
                                                                             CancellationToken                                                                               token
    )
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
    protected async Task<TResult> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>( Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, CancellationToken, Task<TResult>> func,
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
    protected async Task<TResult> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>( Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, CancellationToken, Task<TResult>> func,
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
    protected async Task<TResult> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>( Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, CancellationToken, Task<TResult>> func,
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
    protected async Task<TResult> Call<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>( Func<DbConnection, DbTransaction, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, CancellationToken, Task<TResult>> func,
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
}



public abstract class Database<TDatabase> : Database, IEquatable<TDatabase>, IComparable<TDatabase>, ICloneable where TDatabase : Database<TDatabase>
{
    protected Database() : base() { }


    public abstract bool Equals( TDatabase?   other );
    public abstract int CompareTo( TDatabase? other );
    public abstract TDatabase Clone();
    object ICloneable.Clone() => Clone();
}
