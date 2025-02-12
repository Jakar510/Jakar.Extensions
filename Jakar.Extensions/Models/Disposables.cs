// Jakar.Extensions :: Jakar.Extensions
// 01/03/2025  16:01

namespace Jakar.Extensions;


public sealed class Disposables : ConcurrentBag<IDisposable>, IDisposable
{
    public Disposables() : base() { }
    public Disposables( IEnumerable<IDisposable> enumerable ) : base( enumerable ) { }
    public void Dispose()
    {
        foreach ( IDisposable disposable in this ) { disposable.Dispose(); }

        Clear();
    }
}



public sealed class AsyncDisposables : ConcurrentBag<IAsyncDisposable>, IAsyncDisposable
{
    public AsyncDisposables() : base() { }
    public AsyncDisposables( IEnumerable<IAsyncDisposable> enumerable ) : base( enumerable ) { }
    public async ValueTask DisposeAsync()
    {
        foreach ( IAsyncDisposable disposable in this ) { await disposable.DisposeAsync().ConfigureAwait( false ); }

        Clear();
    }
}
