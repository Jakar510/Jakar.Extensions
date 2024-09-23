// Jakar.Extensions :: Jakar.Extensions
// 11/25/2023  9:40 PM

namespace Jakar.Extensions;


public class LockerEnumerator<TValue>( ILockedCollection<TValue> collection ) : IEnumerator<TValue>, IEnumerable<TValue>
{
    private const    int                       START_INDEX = 0;
    private readonly ILockedCollection<TValue> _collection = collection;
    private          bool                      _isDisposed;
    private          int                       _index = START_INDEX;
    private          FilterBuffer<TValue>?     _owner;


    private             ReadOnlyMemory<TValue> _Memory => _owner?.Memory ?? ReadOnlyMemory<TValue>.Empty;
    public ref readonly TValue                 Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref _Memory.Span[_index]; }
    TValue IEnumerator<TValue>.                Current => Current;
    object? IEnumerator.                       Current => Current;


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

        return (uint)++_index < (uint)_Memory.Length;
    }
    public void Reset()
    {
        ThrowIfDisposed();
        _owner?.Dispose();
        _owner = _collection.Copy();
        _index = START_INDEX;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf( _isDisposed, this );
}
