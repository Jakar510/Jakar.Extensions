// Jakar.Extensions :: Jakar.Extensions
// 11/25/2023  9:40 PM

namespace Jakar.Extensions;


public class LockerEnumerator<TValue>( ILockedCollection<TValue> collection ) : IEnumerator<TValue>, IEnumerable<TValue>
{
    private const    int                       START_INDEX = -1;
    private readonly ILockedCollection<TValue> _collection = collection;
    private          int                       _index      = START_INDEX;
    private          ReadOnlyMemory<TValue>    _cache;


    public              int                  Length          { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _cache.Length; }
    public              ReadOnlySpan<TValue> Span            { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _cache.Span; }
    public ref readonly TValue               Current         { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref Span[_index]; }
    TValue IEnumerator<TValue>.              Current         { get => Current; }
    object? IEnumerator.                     Current         => Current;
    private bool                             _ShouldContinue { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ++_index < Length; }


    public void Dispose()
    {
        _cache = default;
        GC.SuppressFinalize( this );
    }
    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => this;
    IEnumerator IEnumerable.                GetEnumerator() => this;


    public bool MoveNext()
    {
        if ( _cache.IsEmpty ) { Reset(); }

        bool result = _ShouldContinue;
        if ( result is false ) { Reset(); }

        return result;
    }
    public void Reset()
    {
        _cache = _collection.Copy();
        _index = START_INDEX;
    }
}
