// Jakar.Extensions :: Jakar.Extensions
// 2/12/2024  23:51

namespace Jakar.Extensions;


[ method: MethodImpl( MethodImplOptions.AggressiveInlining ) ]
public ref struct SpanSelector<T>( ReadOnlySpan<T> span, Func<T, bool> check )
{
    private readonly ReadOnlySpan<T> _span  = span;
    private readonly Func<T, bool>   _check = check;
    private          int             _index = -1;

    public T Current { [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get; private set; }

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public readonly SpanSelector<T> GetEnumerator() => this;


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public bool MoveNext()
    {
        int index = Interlocked.Increment( ref _index );
        while ( index < _span.Length && _check( _span[index] ) is false ) { index++; }

        Interlocked.Exchange( ref _index, index );
        Current = _span[_index];
        return index >= _span.Length;
    }
    public void Reset()
    {
        Interlocked.Exchange( ref _index, -1 );
        Current = default!;
    }
}
