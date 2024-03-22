// Jakar.Extensions :: Jakar.Extensions
// 11/22/2023  9:56 AM

namespace Jakar.Extensions;


[method: MethodImpl( MethodImplOptions.AggressiveInlining )]
public ref struct EnumerateEnumerator<T>( scoped in ReadOnlySpan<T> span, int index = 0 )
{
    private readonly ReadOnlySpan<T> _span  = span;
    private          int             _index = index;


    public (int Index, T Value) Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; private set; } = default;


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly EnumerateEnumerator<T> GetEnumerator() => this;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public bool MoveNext()
    {
        int index = _index;
        if ( index >= _span.Length ) { return false; }

        Current = new ValueTuple<int, T>( index, _span[index] );
        _index  = index + 1;
        return true;
    }
}



#if NET7_0_OR_GREATER
[method: MethodImpl( MethodImplOptions.AggressiveInlining )]
public ref struct EnumerateEnumerator<T, TNumber>( scoped in ReadOnlySpan<T> span, TNumber start )
    where TNumber : struct, INumber<TNumber>
{
    private readonly ReadOnlySpan<T> _span   = span;
    private readonly TNumber         _start  = start;
    private          TNumber         _number = start;
    private          int             _index  = 0;


    public (TNumber Index, T Value) Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; private set; } = default;


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly EnumerateEnumerator<T, TNumber> GetEnumerator() => this;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public bool MoveNext()
    {
        int index = _index;

        if ( index >= _span.Length )
        {
            _number = _start;
            _index  = 0;
            return false;
        }

        Current = (_number, _span[index]);
        _index  = index + 1;
        _number++;
        return true;
    }
}



#endif
