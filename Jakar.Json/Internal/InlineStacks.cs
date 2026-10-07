// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary> One bit per nesting level, inline (no allocation): 17 × 64 bits covers depths 0..<see cref="JsonWriterOptions.MAX_DEPTH_LIMIT"/>. </summary>
[InlineArray(LENGTH)]
internal struct BitStack
{
    private const int LENGTH = 17;

    private ulong __element;


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly bool Get( int index ) => ( this[index >> 6] & ( 1UL << index ) ) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Set( int index, bool value )
    {
        ref ulong word = ref this[index >> 6];
        ulong     mask = 1UL << index;

        word = value
                   ? word | mask
                   : word & ~mask;
    }
}



/// <summary> One <see cref="int"/> per nesting level for the first <see cref="LENGTH"/> levels: readers record names and indices here to report error paths. </summary>
[InlineArray(LENGTH)]
internal struct PathStack
{
    public const int LENGTH = 64;

    private int __element;
}
