// Jakar.Extensions :: Jakar.Extensions
// 01/03/2025  16:01

namespace Jakar.Extensions;


public sealed class Disposables : IEnumerable<IDisposable>, IDisposable
{
    private readonly ConcurrentBag<IDisposable> _disposables = [];
    public Disposables() : base() { }
    public Disposables( IEnumerable<IDisposable> enumerable ) => _disposables = new ConcurrentBag<IDisposable>( enumerable );
    public void Dispose()
    {
        foreach ( IDisposable disposable in _disposables ) { disposable.Dispose(); }

        _disposables.Clear();
    }


    public void                     Add( IDisposable                      disposable )  => _disposables.Add( disposable );
    public void                     Add( params ReadOnlySpan<IDisposable> disposables ) => _disposables.Add( disposables );
    public IEnumerator<IDisposable> GetEnumerator()                                     => _disposables.GetEnumerator();
    IEnumerator IEnumerable.        GetEnumerator()                                     => GetEnumerator();
}



public sealed class AsyncDisposables : IEnumerable<IAsyncDisposable>, IAsyncDisposable
{
    private readonly ConcurrentBag<IAsyncDisposable> _disposables = [];
    public AsyncDisposables() : base() { }
    public AsyncDisposables( IEnumerable<IAsyncDisposable> enumerable ) => _disposables = new ConcurrentBag<IAsyncDisposable>( enumerable );
    public async ValueTask DisposeAsync()
    {
        foreach ( IAsyncDisposable disposable in _disposables ) { await disposable.DisposeAsync().ConfigureAwait( false ); }

        _disposables.Clear();
    }


    public void                          Add( IAsyncDisposable                      disposable )  => _disposables.Add( disposable );
    public void                          Add( params ReadOnlySpan<IAsyncDisposable> disposables ) => _disposables.Add( disposables );
    public IEnumerator<IAsyncDisposable> GetEnumerator()                                          => _disposables.GetEnumerator();
    IEnumerator IEnumerable.             GetEnumerator()                                          => GetEnumerator();
}