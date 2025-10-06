// Jakar.Extensions :: Jakar.Extensions
// 01/03/2025  16:01

namespace Jakar.Extensions;


public sealed class Disposables : IEnumerable<IDisposable>, IDisposable
{
    private readonly ConcurrentBag<IDisposable> __disposables = [];
    public Disposables() : base() { }
    public Disposables( IEnumerable<IDisposable>         enumerable ) => __disposables = [..enumerable];
    public Disposables( params ReadOnlySpan<IDisposable> enumerable ) => __disposables = [..enumerable];
    public void Dispose()
    {
        foreach ( IDisposable disposable in __disposables ) { disposable.Dispose(); }

        __disposables.Clear();
    }


    public void                     Add( IDisposable                      disposable )  => __disposables.Add(disposable);
    public void                     Add( params ReadOnlySpan<IDisposable> disposables ) => __disposables.Add(disposables);
    public IEnumerator<IDisposable> GetEnumerator()                                     => __disposables.GetEnumerator();
    IEnumerator IEnumerable.        GetEnumerator()                                     => GetEnumerator();


    public static void ClearAndDispose<TValue>( ref TValue? resource )
        where TValue : IDisposable
    {
        resource?.Dispose();
        resource = default;
    }

    public static ValueTask ClearAndDisposeAsync<TValue>( ref TValue? resource )
        where TValue : IDisposable
    {
        ValueTask task = CastAndDisposeAsync(resource);
        resource = default;
        return task;
    }

    public static async ValueTask CastAndDisposeAsync<TValue>( TValue? resource )
        where TValue : IDisposable
    {
        switch ( resource )
        {
            case null:
                return;

            case IAsyncDisposable resourceAsyncDisposable:
                await resourceAsyncDisposable.DisposeAsync();
                return;

            default:
                resource.Dispose();
                return;
        }
    }
}



public sealed class AsyncDisposables : IEnumerable<IAsyncDisposable>, IAsyncDisposable
{
    private readonly ConcurrentBag<IAsyncDisposable> __disposables = [];
    public AsyncDisposables() : base() { }
    public AsyncDisposables( IEnumerable<IAsyncDisposable>         enumerable ) => __disposables = [..enumerable];
    public AsyncDisposables( params ReadOnlySpan<IAsyncDisposable> enumerable ) => __disposables = [..enumerable];
    public async ValueTask DisposeAsync()
    {
        foreach ( IAsyncDisposable disposable in __disposables )
        {
            await disposable.DisposeAsync()
                            .ConfigureAwait(false);
        }

        __disposables.Clear();
    }


    public void                          Add( IAsyncDisposable                      disposable )  => __disposables.Add(disposable);
    public void                          Add( params ReadOnlySpan<IAsyncDisposable> disposables ) => __disposables.Add(disposables);
    public IEnumerator<IAsyncDisposable> GetEnumerator()                                          => __disposables.GetEnumerator();
    IEnumerator IEnumerable.             GetEnumerator()                                          => GetEnumerator();
}
