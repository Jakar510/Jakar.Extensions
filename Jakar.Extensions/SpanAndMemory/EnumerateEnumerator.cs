// Jakar.Extensions :: Jakar.Extensions
// 11/22/2023  9:56 AM

namespace Jakar.Extensions;


public ref struct EnumerateEnumerator<T>
{
    private readonly ReadOnlySpan<T> _span;
    private          int             _index;


    public (int Index, T Value) Current { [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get; private set; } = default;


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public EnumerateEnumerator( ReadOnlySpan<T> span, int index = -1 )
    {
        _span  = span;
        _index = index;
    }
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public readonly EnumerateEnumerator<T> GetEnumerator() => this;


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public bool MoveNext()
    {
        int index = _index + 1;
        if ( index >= _span.Length ) { return false; }

        _index  = index;
        Current = new ValueTuple<int, T>( _index, _span[_index] );
        return true;
    }
}
