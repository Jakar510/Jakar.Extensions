// Jakar.Extensions :: Jakar.Extensions
// 11/25/2023  9:40 PM

namespace Jakar.Extensions;


public class LockerEnumerator<TValue>( ILockedCollection<TValue> collection ) : IEnumerator<TValue>, IEnumerable<TValue>
{
    private const    int                       START_INDEX = NOT_FOUND;
    private readonly ILockedCollection<TValue> _collection = collection;
    private          int                       _index      = START_INDEX;
    private          ReadOnlyMemory<TValue>    _memory;


    internal            bool   ShouldContinue { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _index < Length; }
    public ref readonly TValue Current        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref Span[_index]; }
    TValue IEnumerator<TValue>.Current        => Current;
    object? IEnumerator.       Current        => Current;


    public int                  Length { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _memory.Length; }
    public ReadOnlySpan<TValue> Span   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _memory.Span; }


    public void Dispose()
    {
        _memory = default;
        GC.SuppressFinalize( this );
    }
    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => this;
    IEnumerator IEnumerable.                GetEnumerator() => this;


    public bool MoveNext()
    {
        if ( _memory.IsEmpty ) { Reset(); }

        _index++;
        bool result = ShouldContinue;
        if ( result is false ) { Reset(); }

        return result;
    }
    public void Reset()
    {
        _memory = _collection.Copy();
        _index  = START_INDEX;
    }
}
