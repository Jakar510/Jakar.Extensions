// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:39 PM

using System.Collections.Concurrent;



namespace Jakar.Database;


public abstract class Database<TID> : ObservableClass, IConnectableDb, IAsyncDisposable where TID : IComparable<TID>, IEquatable<TID>
{
    protected readonly ConcurrentBag<IAsyncDisposable> _disposables = new();


    protected Database() : base() { }
    public virtual async ValueTask DisposeAsync()
    {
        foreach ( IAsyncDisposable disposable in _disposables ) { await disposable.DisposeAsync(); }

        _disposables.Clear();
    }


    protected TValue AddDisposable<TValue>( TValue value ) where TValue : IAsyncDisposable
    {
        _disposables.Add(value);
        return value;
    }


    protected abstract DbConnection CreateConnection();
    public DbConnection Connect()
    {
        DbConnection connection = CreateConnection();
        connection.Open();
        return connection;
    }
    public async Task<DbConnection> ConnectAsync( CancellationToken token )
    {
        DbConnection connection = CreateConnection();
        await connection.OpenAsync(token);
        return connection;
    }
}



[SuppressMessage("ReSharper", "UnusedType.Global")]
public abstract class Database<TDatabase, TID> : Database<TID>, IEquatable<TDatabase>, ICloneable where TDatabase : Database<TDatabase, TID>
                                                                                                  where TID : IComparable<TID>, IEquatable<TID>
{
    protected Database() : base() { }


    public abstract bool Equals( TDatabase? other );
    public abstract TDatabase Clone();
    object ICloneable.Clone() => Clone();
}
