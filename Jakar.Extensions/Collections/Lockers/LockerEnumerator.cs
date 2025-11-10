// Jakar.Extensions :: Jakar.Extensions
// 11/25/2023  9:40 PM

namespace Jakar.Extensions;


public class LockerEnumerator<TValue, TCloser>( ILockedCollection<TValue, TCloser> collection ) : IEnumerator<TValue>, IEnumerable<TValue>
    where TCloser : IDisposable
{
    private const    int                                START_INDEX  = 0;
    private readonly ILockedCollection<TValue, TCloser> __collection = collection;
    private          bool                               __isDisposed;
    private          ArrayBuffer<TValue>?              __owner;
    private          int                                __index = START_INDEX;


    private             ReadOnlyMemory<TValue> __Memory => __owner?.Memory ?? ReadOnlyMemory<TValue>.Empty;
    public ref readonly TValue                 Current  { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => ref __Memory.Span[__index]; }
    TValue IEnumerator<TValue>.                Current  => Current;
    object? IEnumerator.                       Current  => Current;


    public void Dispose()
    {
        __isDisposed = true;
        __owner?.Dispose();
        __owner = null;
        GC.SuppressFinalize(this);
    }
    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => this;
    IEnumerator IEnumerable.                GetEnumerator() => this;


    public bool MoveNext()
    {
        ThrowIfDisposed();
        if ( __Memory.IsEmpty ) { Reset(); }

        return (uint)++__index < (uint)__Memory.Length;
    }
    public void Reset()
    {
        ThrowIfDisposed();
        __owner?.Dispose();
        __owner = __collection.Copy();
        __index = START_INDEX;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(__isDisposed, this);
}
