// Jakar.Extensions :: Jakar.Extensions
// 11/25/2023  9:40 PM

namespace Jakar.Extensions;


public class LockerEnumerator<TValue>( ILockedCollection<TValue> collection ) : IEnumerator<TValue>, IEnumerable<TValue>
{
    private const    int                       START_INDEX = 0;
    private readonly ILockedCollection<TValue> _collection = collection;
    private          int                       _index      = START_INDEX;
    private          ReadOnlyMemory<TValue>    _memory;
    private          bool                      _isDisposed;

    public ref readonly TValue  Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref Span[_index]; }
    TValue IEnumerator<TValue>. Current => Current;
    object? IEnumerator.        Current => Current;
    public int                  Length  { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _memory.Length; }
    public ReadOnlySpan<TValue> Span    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _memory.Span; }


    public void Dispose()
    {
        _isDisposed = true;
        _memory     = ReadOnlyMemory<TValue>.Empty;
        GC.SuppressFinalize( this );
    }
    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => this;
    IEnumerator IEnumerable.                GetEnumerator() => this;


    public bool MoveNext()
    {
        ThrowIfDisposed();
        if ( _memory.IsEmpty ) { Reset(); }

        if ( (uint)_index >= (uint)_memory.Length ) { return false; }

        _index++;
        return true;
    }
    public void Reset()
    {
        ThrowIfDisposed();
        _memory = _collection.Copy();
        _index  = START_INDEX;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private void ThrowIfDisposed()
    {
    #if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf( _isDisposed, this );
    #else
        if ( _isDisposed ) { throw new ObjectDisposedException( nameof(AsyncLockerEnumerator<TValue>) ); }
    #endif
    }
}
