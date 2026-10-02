// Jakar.Extensions :: Jakar.Extensions
// 2/12/2024  23:51

namespace Jakar.Extensions;


[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public ref struct SpanSelector<TValue, TNext>( ReadOnlySpan<TValue> span, Func<TValue, TNext> func )
{
    private readonly ReadOnlySpan<TValue> __span  = span;
    private readonly Func<TValue, TNext>  __func  = func;
    private volatile int                  __index = -1;

    public TNext Current { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; } = default!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly SpanSelector<TValue, TNext> GetEnumerator() => this;


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool MoveNext()
    {
        int index = __index + 1;
        if ( index >= __span.Length ) { return false; }

        Current = __func(__span[index]);
        __index = index;
        return true;
    }
    public void Reset()
    {
        Interlocked.Exchange(ref __index, -1);
        Current = default!;
    }
}
