// Jakar.Extensions :: Jakar.Extensions
// 2/12/2024  23:51

namespace Jakar.Extensions;


[method: MethodImpl( MethodImplOptions.AggressiveInlining )]
public ref struct SpanSelector<T, TNext>( ReadOnlySpan<T> span, Func<T, TNext> func )
{
    private readonly ReadOnlySpan<T> _span  = span;
    private readonly Func<T, TNext>  _func  = func;
    private          int             _index = NOT_FOUND;

    public TNext Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; private set; } = default!;

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly SpanSelector<T, TNext> GetEnumerator() => this;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public bool MoveNext()
    {
        int index = _index + 1;
        if ( index >= _span.Length ) { return false; }

        Current = _func( _span[index] );
        _index  = index;
        return true;
    }
    public void Reset()
    {
        Interlocked.Exchange( ref _index, NOT_FOUND );
        Current = default!;
    }
}
