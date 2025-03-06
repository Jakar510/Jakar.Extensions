namespace Jakar.Extensions;


public static class Buffers
{
    public const int NOT_FOUND        = -1;
    public const int DEFAULT_CAPACITY = 64;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void EnsureCapacity<TValue>( ref Buffer<TValue> buffer, int additionalRequestedCapacity )
        where TValue : IEquatable<TValue> => EnsureCapacity( ref buffer, (uint)additionalRequestedCapacity );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void EnsureCapacity<TValue>( ref Buffer<TValue> buffer, uint additionalRequestedCapacity )
        where TValue : IEquatable<TValue>
    {
        if ( buffer.Length + additionalRequestedCapacity > buffer.Capacity ) { buffer = buffer.Grow( additionalRequestedCapacity ); }
    }


    public static int GetLength( in ulong capacity, in ulong requestedCapacity )
    {
        Guard.IsGreaterThan( requestedCapacity, capacity );
        Guard.IsLessThanOrEqualTo( requestedCapacity, int.MaxValue );

        ulong result = Math.Max( requestedCapacity, capacity * 2 );
        return (int)Math.Min( result, int.MaxValue );
    }
}
