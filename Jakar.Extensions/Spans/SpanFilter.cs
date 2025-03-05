// Jakar.Extensions :: Jakar.Extensions
// 2/13/2024  21:33

namespace Jakar.Extensions;


[method: MethodImpl( MethodImplOptions.AggressiveInlining )]
public ref struct SpanFilter<T>( scoped in ReadOnlySpan<T> span, Func<T, bool> func )
{
    private readonly ReadOnlySpan<T> _span  = span;
    private readonly Func<T, bool>   _func  = func;
    private          int             _index = NOT_FOUND;

    public T Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; private set; } = default!;

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly SpanFilter<T> GetEnumerator() => this;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public bool MoveNext()
    {
        int index = _index + 1;
        while ( index < _span.Length && _func( _span[index] ) is false ) { index++; }

        Current = _span[index];
        _index  = index;
        return index < _span.Length;
    }
    public void Reset()
    {
        Interlocked.Exchange( ref _index, NOT_FOUND );
        Current = default!;
    }
}
