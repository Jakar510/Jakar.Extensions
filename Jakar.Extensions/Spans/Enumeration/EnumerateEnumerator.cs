// Jakar.Extensions :: Jakar.Extensions
// 11/22/2023  9:56 AM

namespace Jakar.Extensions;


[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public ref struct EnumerateEnumerator<TValue>( ReadOnlySpan<TValue> span )
{
    private readonly ReadOnlySpan<TValue> __buffer = span;
    private          int                  __index  = 0;

    public (int Index, TValue Value) Current { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; } = default;

    public EnumerateEnumerator( int startIndex, ReadOnlySpan<TValue> span ) : this(span) => __index = startIndex;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly EnumerateEnumerator<TValue> GetEnumerator() => this;


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool MoveNext()
    {
        int index = int.CreateChecked(__index);

        if ( index >= __buffer.Length )
        {
            __index = 0;
            return false;
        }

        Current = ( __index, __buffer[index] );
        __index++;
        return true;
    }
}
