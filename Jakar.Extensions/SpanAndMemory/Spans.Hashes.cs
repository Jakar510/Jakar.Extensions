// Jakar.Extensions :: Jakar.Extensions
// 11/30/2023  5:39 PM

using System.IO.Hashing;



namespace Jakar.Extensions;


public static partial class Spans
{
    private static readonly ArrayPool<byte> _bytePool = ArrayPool<byte>.Shared;


#if NET7_0_OR_GREATER
    public static UInt128 Hash( in ReadOnlySpan<bool> value )
    {
        const int SIZE = sizeof(bool);

        byte[] buffer = _bytePool.Rent( SIZE * value.Length );

        try
        {
            for ( var i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                var        range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                BitConverter.TryWriteBytes( span, value[i] );
            }

            return XxHash128.HashToUInt128( buffer );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static UInt128 Hash( in ReadOnlySpan<char> value )
    {
        const int SIZE   = sizeof(char);
        byte[]    buffer = _bytePool.Rent( SIZE * value.Length );

        try
        {
            for ( var i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                var        range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                BitConverter.TryWriteBytes( span, value[i] );
            }

            return XxHash128.HashToUInt128( buffer );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static UInt128 Hash( in ReadOnlySpan<short> value )
    {
        const int SIZE   = sizeof(short);
        byte[]    buffer = _bytePool.Rent( SIZE * value.Length );

        try
        {
            for ( var i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                var        range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                BitConverter.TryWriteBytes( span, value[i] );
            }

            return XxHash128.HashToUInt128( buffer );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static UInt128 Hash( in ReadOnlySpan<ushort> value )
    {
        const int SIZE   = sizeof(ushort);
        byte[]    buffer = _bytePool.Rent( SIZE * value.Length );

        try
        {
            for ( var i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                var        range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                BitConverter.TryWriteBytes( span, value[i] );
            }

            return XxHash128.HashToUInt128( buffer );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static UInt128 Hash( in ReadOnlySpan<int> value )
    {
        const int SIZE   = sizeof(int);
        byte[]    buffer = _bytePool.Rent( SIZE * value.Length );

        try
        {
            for ( var i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                var        range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                BitConverter.TryWriteBytes( span, value[i] );
            }

            return XxHash128.HashToUInt128( buffer );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static UInt128 Hash( in ReadOnlySpan<uint> value )
    {
        const int SIZE   = sizeof(uint);
        byte[]    buffer = _bytePool.Rent( SIZE * value.Length );

        try
        {
            for ( var i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                var        range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                BitConverter.TryWriteBytes( span, value[i] );
            }

            return XxHash128.HashToUInt128( buffer );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static UInt128 Hash( in ReadOnlySpan<long> value )
    {
        const int SIZE   = sizeof(long);
        byte[]    buffer = _bytePool.Rent( SIZE * value.Length );

        try
        {
            for ( var i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                var        range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                BitConverter.TryWriteBytes( span, value[i] );
            }

            return XxHash128.HashToUInt128( buffer );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static UInt128 Hash( in ReadOnlySpan<ulong> value )
    {
        const int SIZE   = sizeof(ulong);
        byte[]    buffer = _bytePool.Rent( SIZE * value.Length );

        try
        {
            for ( var i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                var        range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                BitConverter.TryWriteBytes( span, value[i] );
            }

            return XxHash128.HashToUInt128( buffer );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static UInt128 Hash( in ReadOnlySpan<Half> value )
    {
        const int SIZE   = sizeof(bool);
        byte[]    buffer = _bytePool.Rent( SIZE * value.Length );

        try
        {
            for ( var i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                var        range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                BitConverter.TryWriteBytes( span, value[i] );
            }

            return XxHash128.HashToUInt128( buffer );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static UInt128 Hash( in ReadOnlySpan<float> value )
    {
        const int SIZE   = sizeof(float);
        byte[]    buffer = _bytePool.Rent( SIZE * value.Length );

        try
        {
            for ( var i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                var        range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                BitConverter.TryWriteBytes( span, value[i] );
            }

            return XxHash128.HashToUInt128( buffer );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static UInt128 Hash( in ReadOnlySpan<double> value )
    {
        const int SIZE   = sizeof(double);
        byte[]    buffer = _bytePool.Rent( SIZE * value.Length );

        try
        {
            for ( var i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                var        range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                BitConverter.TryWriteBytes( span, value[i] );
            }

            return XxHash128.HashToUInt128( buffer );
        }
        finally { _bytePool.Return( buffer ); }
    }
#endif
}
