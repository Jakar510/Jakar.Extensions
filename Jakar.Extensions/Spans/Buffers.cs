namespace Jakar.Extensions;


public static class Buffers
{
    public const int NOT_FOUND        = -1;
    public const int DEFAULT_CAPACITY = 64;


    public static int GetLength( in ulong capacity, in ulong requestedCapacity )
    {
        Guard.IsGreaterThan( requestedCapacity, capacity );
        Guard.IsLessThanOrEqualTo( requestedCapacity, int.MaxValue );

        ulong result = Math.Max( requestedCapacity, capacity * 2 );
        return (int)Math.Min( result, int.MaxValue );
    }


    public static Buffer<T> AsBuffer<T>( this ReadOnlySpan<T> span ) => new(span);
    public static void Trim<T>( this Buffer<T> buffer, scoped in T value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = buffer.buffer.Trim( value );
        buffer.Length = span.Length;
        span.CopyTo( buffer.buffer );
    }
    public static void Trim<T>( this Buffer<T> buffer, scoped in ReadOnlySpan<T> value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = buffer.buffer.Trim( value );
        buffer.Length = span.Length;
        span.CopyTo( buffer.buffer );
    }
    public static void TrimStart<T>( this Buffer<T> buffer, scoped in T value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = buffer.buffer.TrimStart( value );
        buffer.Length = span.Length;
        span.CopyTo( buffer.buffer );
    }
    public static void TrimStart<T>( this Buffer<T> buffer, scoped in ReadOnlySpan<T> value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = buffer.buffer.TrimStart( value );
        buffer.Length = span.Length;
        span.CopyTo( buffer.buffer );
    }
    public static void TrimEnd<T>( this Buffer<T> buffer, scoped in T value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = buffer.buffer.TrimEnd( value );
        buffer.Length = span.Length;
        span.CopyTo( buffer.buffer );
    }
    public static void TrimEnd<T>( this Buffer<T> buffer, scoped in ReadOnlySpan<T> value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = buffer.buffer.TrimEnd( value );
        buffer.Length = span.Length;
        span.CopyTo( buffer.buffer );
    }
}
