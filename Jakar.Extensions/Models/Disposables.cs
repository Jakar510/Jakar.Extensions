// Jakar.Extensions :: Jakar.Extensions
// 01/03/2025  16:01

namespace Jakar.Extensions;


public sealed class Disposables() : List<IDisposable>( Buffers.DEFAULT_CAPACITY ), IDisposable
{
    public void Dispose()
    {
        foreach ( IDisposable disposable in this ) { disposable.Dispose(); }

        Clear();
    }
}



public sealed class AsyncDisposables() : List<IAsyncDisposable>( Buffers.DEFAULT_CAPACITY ), IAsyncDisposable
{
    public async ValueTask DisposeAsync()
    {
        foreach ( IAsyncDisposable disposable in this ) { await disposable.DisposeAsync().ConfigureAwait( false ); }

        Clear();
    }
}
