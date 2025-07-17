namespace Jakar.Extensions;


public static class Buffers
{
    public const int DEFAULT_CAPACITY = 64;
    public const int NOT_FOUND        = -1;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EnsureCapacity<TValue>( [MustDisposeResource] this ref Buffer<TValue> buffer, int additionalRequestedCapacity )
        where TValue : IEquatable<TValue>
    {
        uint capacity = (uint)additionalRequestedCapacity;

        if ( (uint)buffer.Length + capacity <= (uint)buffer.Capacity ) { return; }

        buffer = buffer.Grow(capacity);
    }


    public static int GetLength( in ulong capacity, in ulong requestedCapacity )
    {
        Guard.IsGreaterThan(requestedCapacity, capacity);
        Guard.IsLessThanOrEqualTo(requestedCapacity, int.MaxValue);

        ulong result = Math.Max(requestedCapacity, capacity * 2);
        return (int)Math.Min(result, int.MaxValue);
    }
}
