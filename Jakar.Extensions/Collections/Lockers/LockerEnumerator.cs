// Jakar.Extensions :: Jakar.Extensions
// 11/25/2023  9:40 PM

namespace Jakar.Extensions;


public class LockerEnumerator<TValue>( ILockedCollection<TValue> collection ) : IEnumerator<TValue>, IEnumerable<TValue>
{
    private const    int                       START_INDEX = 0;
    private readonly ILockedCollection<TValue> _collection = collection;
    private          bool                      _isDisposed;
    private          int                       _index = START_INDEX;
    private          IMemoryOwner<TValue>?     _owner;


    private             ReadOnlyMemory<TValue> _Memory => _owner?.Memory ?? Memory<TValue>.Empty;
    public ref readonly TValue                 Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref Span[_index]; }
    TValue IEnumerator<TValue>.                Current => Current;
    object? IEnumerator.                       Current => Current;
    public int                                 Length  { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _Memory.Length; }
    public ReadOnlySpan<TValue>                Span    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _Memory.Span; }


    public void Dispose()
    {
        _isDisposed = true;
        _owner?.Dispose();
        _owner = null;
        GC.SuppressFinalize( this );
    }
    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => this;
    IEnumerator IEnumerable.                GetEnumerator() => this;


    public bool MoveNext()
    {
        ThrowIfDisposed();
        if ( _Memory.IsEmpty ) { Reset(); }

        if ( (uint)_index >= (uint)_Memory.Length ) { return false; }

        _index++;
        return true;
    }
    public void Reset()
    {
        ThrowIfDisposed();
        _owner?.Dispose();
        _owner = null;
        _index = START_INDEX;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf( _isDisposed, this );
}
