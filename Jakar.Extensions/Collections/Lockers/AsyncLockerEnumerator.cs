// Jakar.Extensions :: Jakar.Extensions
// 11/25/2023  9:40 PM

namespace Jakar.Extensions;


public class AsyncLockerEnumerator<TValue, TCloser>( ILockedCollection<TValue, TCloser> collection, CancellationToken token = default ) : IAsyncEnumerable<TValue>, IAsyncEnumerator<TValue>
    where TCloser : IDisposable
{
    private const    int                                START_INDEX = 0;
    private readonly ILockedCollection<TValue, TCloser> __collection = collection;
    private          bool                               __isDisposed;
    private          CancellationToken                  __token = token;
    private          FilterBuffer<TValue>?              __buffer;
    private          int                                __index = START_INDEX;


    private ReadOnlyMemory<TValue>  __Memory        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => __buffer?.Memory ?? ReadOnlyMemory<TValue>.Empty; }
    TValue IAsyncEnumerator<TValue>.Current        => Current;
    public ref readonly TValue      Current        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref __Memory.Span[__index]; }
    internal            bool        ShouldContinue { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => __token.ShouldContinue() && (uint)__index < (uint)__Memory.Length; }


    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize( this );
        __isDisposed = true;
        __buffer?.Dispose();
        __buffer = null;
        return ValueTask.CompletedTask;
    }


    public async ValueTask<bool> MoveNextAsync()
    {
        ThrowIfDisposed();

        if ( __Memory.IsEmpty )
        {
            __buffer?.Dispose();
            __buffer = await __collection.CopyAsync( __token );
            __index  = START_INDEX;
        }

        if ( __token.IsCancellationRequested || (uint)__index >= (uint)__Memory.Length ) { return false; }

        __index++;
        return true;
    }


    IAsyncEnumerator<TValue> IAsyncEnumerable<TValue>.GetAsyncEnumerator( CancellationToken token ) => GetAsyncEnumerator( token );
    public AsyncLockerEnumerator<TValue, TCloser> GetAsyncEnumerator( CancellationToken token = default )
    {
        ThrowIfDisposed();
        __buffer?.Dispose();
        __buffer = null;
        __index  = START_INDEX;
        __token  = token;
        return this;
    }
    public override string ToString() => $"AsyncEnumerator<{typeof(TValue).Name}>( {nameof(__index)} : {__index}, {nameof(ShouldContinue)} : {ShouldContinue} )";


    [MethodImpl( MethodImplOptions.AggressiveInlining )] private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf( __isDisposed, this );
}
