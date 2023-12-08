// Jakar.Extensions :: Jakar.Extensions
// 11/22/2023  9:56 AM

namespace Jakar.Extensions;


public ref struct EnumerateEnumerator<T>
{
    private readonly ReadOnlySpan<T> _span;
    private          int             _index;


    public (int Index, T Value) Current { [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get; private set; } = default;


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public EnumerateEnumerator( ReadOnlySpan<T> span, int index = 0 )
    {
        _span  = span;
        _index = index;
    }
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public readonly EnumerateEnumerator<T> GetEnumerator() => this;


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public bool MoveNext()
    {
        int index = _index;
        if ( index + 1 >= _span.Length ) { return false; }

        Current = new ValueTuple<int, T>( index, _span[index] );
        _index  = index + 1;
        return true;
    }
}



#if NET7_0_OR_GREATER
public ref struct EnumerateEnumerator<T, TNumber>
    where TNumber : struct, INumber<TNumber>
{
    private readonly ReadOnlySpan<T> _span;
    private readonly TNumber         _start;
    private          TNumber         _number;
    private          int             _index;


    public (TNumber Index, T Value) Current { [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get; private set; } = default;


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public EnumerateEnumerator( ReadOnlySpan<T> span, TNumber start )
    {
        _span   = span;
        _start  = start;
        _number = start;
        _index  = 0;
    }
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public readonly EnumerateEnumerator<T, TNumber> GetEnumerator() => this;


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public bool MoveNext()
    {
        int index = _index;

        if ( index + 1 >= _span.Length )
        {
            _number = _start;
            _index  = 0;
            return false;
        }

        Current = new ValueTuple<TNumber, T>( _number, _span[index] );
        _index  = index + 1;
        _number++;
        return true;
    }
}



#endif
