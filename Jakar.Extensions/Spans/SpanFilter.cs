// Jakar.Extensions :: Jakar.Extensions
// 2/13/2024  21:33

namespace Jakar.Extensions;


[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public ref struct SpanFilter<TValue>( scoped in ReadOnlySpan<TValue> span, Func<TValue, bool> func )
{
    private readonly ReadOnlySpan<TValue> __span  = span;
    private readonly Func<TValue, bool>   __func  = func;
    private volatile int                  __index = -1;

    public TValue Current { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; } = default!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly SpanFilter<TValue> GetEnumerator() => this;


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool MoveNext()
    {
        int index = __index + 1;
        while ( index < __span.Length && !__func(__span[index]) ) { index++; }

        Current = __span[index];
        __index = index;
        return index < __span.Length;
    }
    public void Reset()
    {
        Interlocked.Exchange(ref __index, -1);
        Current = default!;
    }
}
