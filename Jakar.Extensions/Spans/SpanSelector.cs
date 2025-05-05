// Jakar.Extensions :: Jakar.Extensions
// 2/12/2024  23:51

namespace Jakar.Extensions;


[method: MethodImpl( MethodImplOptions.AggressiveInlining )]
public ref struct SpanSelector<TValue, TNext>( ReadOnlySpan<TValue> span, Func<TValue, TNext> func )
{
    private readonly ReadOnlySpan<TValue> _span  = span;
    private readonly Func<TValue, TNext>  _func  = func;
    private          int                  _index = NOT_FOUND;

    public TNext Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; private set; } = default!;

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly SpanSelector<TValue, TNext> GetEnumerator() => this;


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
