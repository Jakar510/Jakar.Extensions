// Jakar.Extensions :: Jakar.Extensions
// 11/30/2023  5:39 PM

using System.IO.Hashing;



namespace Jakar.Extensions;


public static partial class Spans
{
    private static readonly ArrayPool<byte> _bytePool = ArrayPool<byte>.Shared;


#if NET7_0_OR_GREATER
    public static UInt128 Hash128( in ReadOnlySpan<bool> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static UInt128 Hash128( in ReadOnlySpan<char> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static UInt128 Hash128( in ReadOnlySpan<short> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static UInt128 Hash128( in ReadOnlySpan<ushort> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static UInt128 Hash128( in ReadOnlySpan<int> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static UInt128 Hash128( in ReadOnlySpan<uint> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static UInt128 Hash128( in ReadOnlySpan<long> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static UInt128 Hash128( in ReadOnlySpan<ulong> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static UInt128 Hash128( in ReadOnlySpan<Half> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static UInt128 Hash128( in ReadOnlySpan<float> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static UInt128 Hash128( in ReadOnlySpan<double> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }

#endif


#if NET6_0_OR_GREATER

    public static ulong Hash( in ReadOnlySpan<Half> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
#endif


    public static ulong Hash( in ReadOnlySpan<bool> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static ulong Hash( in ReadOnlySpan<char> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static ulong Hash( in ReadOnlySpan<short> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static ulong Hash( in ReadOnlySpan<ushort> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static ulong Hash( in ReadOnlySpan<int> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static ulong Hash( in ReadOnlySpan<uint> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static ulong Hash( in ReadOnlySpan<long> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static ulong Hash( in ReadOnlySpan<ulong> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static ulong Hash( in ReadOnlySpan<float> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
    public static ulong Hash( in ReadOnlySpan<double> value, long seed = 0 )
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
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { _bytePool.Return( buffer ); }
    }
}
