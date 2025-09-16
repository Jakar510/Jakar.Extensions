// Jakar.Extensions :: Jakar.Extensions
// 2/13/2024  21:33

namespace Jakar.Extensions;


[method: MethodImpl( MethodImplOptions.AggressiveInlining )]
public ref struct SpanFilter<TValue>( scoped in ReadOnlySpan<TValue> span, Func<TValue, bool> func )
{
    private readonly ReadOnlySpan<TValue> _span  = span;
    private readonly Func<TValue, bool>   _func  = func;
    private volatile int                  _index = -1;

    public TValue Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; private set; } = default!;

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly SpanFilter<TValue> GetEnumerator() => this;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public bool MoveNext()
    {
        int index = _index + 1;
        while ( index < _span.Length && !_func( _span[index] ) ) { index++; }

        Current = _span[index];
        _index  = index;
        return index < _span.Length;
    }
    public void Reset()
    {
        Interlocked.Exchange( ref _index, -1 );
        Current = default!;
    }
}
