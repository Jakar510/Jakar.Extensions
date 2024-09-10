// Jakar.Extensions :: Jakar.Extensions
// 06/06/2022  2:20 PM


namespace Jakar.Extensions;


public static class Hashes
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int GetHash<T>( this IEnumerable<T> values )
    {
        HashCode hash = new HashCode();
        foreach ( T value in values ) { hash.Add( value ); }

        return hash.ToHashCode();
    }


    [Pure]
    public static UInt128 Hash128( this ReadOnlySpan<bool> value, long seed = 0 )
    {
        const int SIZE   = sizeof(bool);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ReadOnlySpan<string> values, long seed = 0 )
    {
        int    length = values.Sum( static x => x.Length );
        char[] buffer = ArrayPool<char>.Shared.Rent( length );

        try
        {
            Span<char> span = buffer;

            foreach ( string value in values )
            {
                value.CopyTo( span );
                span = span[value.Length..];
            }

            return Hash128( buffer.AsSpan( ..length ), seed );
        }
        finally { ArrayPool<char>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ReadOnlySpan<char> value, long seed = 0 )
    {
        const int SIZE   = sizeof(char);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ReadOnlySpan<short> value, long seed = 0 )
    {
        const int SIZE   = sizeof(short);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ReadOnlySpan<ushort> value, long seed = 0 )
    {
        const int SIZE   = sizeof(ushort);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ReadOnlySpan<int> value, long seed = 0 )
    {
        const int SIZE   = sizeof(int);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ReadOnlySpan<uint> value, long seed = 0 )
    {
        const int SIZE   = sizeof(uint);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ReadOnlySpan<long> value, long seed = 0 )
    {
        const int SIZE   = sizeof(long);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ReadOnlySpan<ulong> value, long seed = 0 )
    {
        const int SIZE   = sizeof(ulong);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ReadOnlySpan<Half> value, long seed = 0 )
    {
        const int SIZE   = sizeof(bool);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ReadOnlySpan<float> value, long seed = 0 )
    {
        const int SIZE   = sizeof(float);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ReadOnlySpan<double> value, long seed = 0 )
    {
        const int SIZE   = sizeof(double);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }


    [Pure]
    public static ulong Hash( this ReadOnlySpan<Half> value, long seed = 0 )
    {
        const int SIZE   = sizeof(bool);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }


    [Pure]
    public static ulong Hash( this ReadOnlySpan<bool> value, long seed = 0 )
    {
        const int SIZE   = sizeof(bool);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static ulong Hash( this ReadOnlySpan<string> values, long seed = 0 )
    {
        int    length = values.Sum( static x => x.Length );
        char[] buffer = ArrayPool<char>.Shared.Rent( length );

        try
        {
            Span<char> span = buffer;

            foreach ( string value in values )
            {
                value.CopyTo( span );
                span = span[value.Length..];
            }

            return Hash( buffer.AsSpan( ..length ), seed );
        }
        finally { ArrayPool<char>.Shared.Return( buffer ); }
    }

    [Pure]
    public static ulong Hash( this ReadOnlySpan<char> value, long seed = 0 )
    {
        const int SIZE   = sizeof(char);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static ulong Hash( this ReadOnlySpan<short> value, long seed = 0 )
    {
        const int SIZE   = sizeof(short);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static ulong Hash( this ReadOnlySpan<ushort> value, long seed = 0 )
    {
        const int SIZE   = sizeof(ushort);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static ulong Hash( this ReadOnlySpan<int> value, long seed = 0 )
    {
        const int SIZE   = sizeof(int);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static ulong Hash( this ReadOnlySpan<uint> value, long seed = 0 )
    {
        const int SIZE   = sizeof(uint);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static ulong Hash( this ReadOnlySpan<long> value, long seed = 0 )
    {
        const int SIZE   = sizeof(long);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static ulong Hash( this ReadOnlySpan<ulong> value, long seed = 0 )
    {
        const int SIZE   = sizeof(ulong);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static ulong Hash( this ReadOnlySpan<float> value, long seed = 0 )
    {
        const int SIZE   = sizeof(float);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static ulong Hash( this ReadOnlySpan<double> value, long seed = 0 )
    {
        const int SIZE   = sizeof(double);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new Range( start, start + SIZE );
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }


    public static string GetHash( this OneOf<byte[], string> data )
    {
        if ( data.IsT0 ) { return GetHash( data.AsT0 ); }

        if ( data.IsT1 ) { return GetHash( data.AsT1 ); }

        throw new InvalidOperationException( "Invalid data type" );
    }
    public static string GetHash( this OneOf<ReadOnlyMemory<byte>, byte[], string> data )
    {
        if ( data.IsT0 ) { return GetHash( data.AsT0.Span ); }

        if ( data.IsT1 ) { return GetHash( data.AsT1 ); }

        if ( data.IsT2 ) { return GetHash( data.AsT2 ); }

        throw new InvalidOperationException( "Invalid data type" );
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string GetHash( byte[]                  data )                    => GetHash( new ReadOnlySpan<byte>( data ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string GetHash( this ReadOnlySpan<byte> data )                    => data.Hash_SHA256();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string GetHash( string                  data )                    => GetHash( data, Encoding.Default );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string GetHash( this ReadOnlySpan<char> data, Encoding encoding ) { return data.Hash_SHA256( encoding ); }


    [Pure]
    public static byte[] ToBytes( this string data, Encoding? encoding = null )
    {
        ReadOnlySpan<char> span = data;
        return span.ToBytes( encoding );
    }
    [Pure]
    public static byte[] ToBytes( this ReadOnlySpan<char> data, Encoding? encoding = null )
    {
        encoding ??= Encoding.Default;
        int                       count  = encoding.GetByteCount( data );
        using IMemoryOwner<byte>? buffer = MemoryPool<byte>.Shared.Rent( count );
        Span<byte>                span   = buffer.Memory.Span[..count];
        encoding.GetBytes( data, span );
        return span.ToArray();
    }


    public static string Hash( this HashAlgorithm hasher, scoped in ReadOnlySpan<char> data, Encoding? encoding = null ) => hasher.Hash( data.ToBytes( encoding ) );
    public static string Hash( this HashAlgorithm hasher, scoped in ReadOnlySpan<byte> data )
    {
        using IMemoryOwner<byte> buffer = MemoryPool<byte>.Shared.Rent( BaseRecord.UNICODE_CAPACITY );
        Span<byte>               span   = buffer.Memory.Span;
        if ( hasher.TryComputeHash( data, span, out int bytesWritten ) is false ) { throw new InvalidOperationException( nameof(span) ); }

        span = span[..bytesWritten];
        Span<char>   hexChars     = stackalloc char[span.Length * 2];
        const string HEX_ALPHABET = "0123456789ABCDEF";

        for ( int i = 0; i < span.Length; i++ )
        {
            byte b = span[i];
            hexChars[i * 2]     = HEX_ALPHABET[b >> 4];
            hexChars[i * 2 + 1] = HEX_ALPHABET[b & 0x0F];
        }

        return hexChars.ToString();
    }


    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public static string Hash_MD5( this ReadOnlySpan<byte> data )
    {
        using MD5 hasher = MD5.Create();
        return hasher.Hash( data );
    }
    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public static string Hash_MD5( this ReadOnlySpan<char> data, Encoding? encoding = null )
    {
        using MD5 hasher = MD5.Create();
        return hasher.Hash( data, encoding );
    }


    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static string Hash_SHA1( this ReadOnlySpan<byte> data )
    {
        using SHA1 hasher = SHA1.Create();
        return hasher.Hash( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static string Hash_SHA1( this ReadOnlySpan<char> data, Encoding? encoding = null )
    {
        using SHA1 hasher = SHA1.Create();
        return hasher.Hash( data, encoding );
    }


    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static string Hash_SHA256( this ReadOnlySpan<byte> data )
    {
        using SHA256 hasher = SHA256.Create();
        return hasher.Hash( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static string Hash_SHA256( this ReadOnlySpan<char> data, Encoding? encoding = null )
    {
        using SHA256 hasher = SHA256.Create();
        return hasher.Hash( data, encoding );
    }


    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static string Hash_SHA384( this ReadOnlySpan<byte> data )
    {
        using SHA384 hasher = SHA384.Create();
        return hasher.Hash( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static string Hash_SHA384( this ReadOnlySpan<char> data, Encoding? encoding = null )
    {
        using SHA384 hasher = SHA384.Create();
        return hasher.Hash( data, encoding );
    }


    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static string Hash_SHA512( this ReadOnlySpan<byte> data )
    {
        using SHA512 hasher = SHA512.Create();
        return hasher.Hash( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static string Hash_SHA512( this ReadOnlySpan<char> data, Encoding? encoding = null )
    {
        using SHA512 hasher = SHA512.Create();
        return hasher.Hash( data, encoding );
    }


    public static async ValueTask<string> HashAsync( this HashAlgorithm hasher, ReadOnlyMemory<byte> data )
    {
        await using MemoryStream stream = new MemoryStream();
        await stream.WriteAsync( data );
        stream.Seek( 0, SeekOrigin.Begin );
        byte[] hash = await hasher.ComputeHashAsync( stream );

        return BitConverter.ToString( hash );
    }
    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public static async ValueTask<string> HashAsync_MD5( this ReadOnlyMemory<byte> data )
    {
        using MD5 hasher = MD5.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static async ValueTask<string> HashAsync_SHA1( this ReadOnlyMemory<byte> data )
    {
        using SHA1 hasher = SHA1.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static async ValueTask<string> HashAsync_SHA256( this ReadOnlyMemory<byte> data )
    {
        using SHA256 hasher = SHA256.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static async ValueTask<string> HashAsync_SHA384( this ReadOnlyMemory<byte> data )
    {
        using SHA384 hasher = SHA384.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static async ValueTask<string> HashAsync_SHA512( this ReadOnlyMemory<byte> data )
    {
        using SHA512 hasher = SHA512.Create();
        return await hasher.HashAsync( data );
    }


    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public static async ValueTask<string> HashAsync_MD5( this byte[] data )
    {
        using MD5 hasher = MD5.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static async ValueTask<string> HashAsync_SHA1( this byte[] data )
    {
        using SHA1 hasher = SHA1.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static async ValueTask<string> HashAsync_SHA256( this byte[] data )
    {
        using SHA256 hasher = SHA256.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static async ValueTask<string> HashAsync_SHA384( this byte[] data )
    {
        using SHA384 hasher = SHA384.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static async ValueTask<string> HashAsync_SHA512( this byte[] data )
    {
        using SHA512 hasher = SHA512.Create();
        return await hasher.HashAsync( data );
    }
    public static async ValueTask<string> HashAsync( this byte[] data, HashAlgorithm hasher )
    {
        await using MemoryStream stream = new MemoryStream( data );
        byte[]                   hash   = await hasher.ComputeHashAsync( stream );
        return BitConverter.ToString( hash );
    }


    public static UInt128 Hash( this string data ) => Hash( data, Encoding.Default );
    public static UInt128 Hash( this string data, Encoding encoding )
    {
        OneOf<byte[], string> one = data.TryGetData();

        return one.IsT0
                   ? Hash( one.AsT0 )
                   : Hash( one.AsT1.AsSpan(), encoding );
    }
    public static UInt128 Hash( this ReadOnlySpan<char> data, Encoding encoding )
    {
        using IMemoryOwner<byte> buffer = MemoryPool<byte>.Shared.Rent( encoding.GetByteCount( data ) );
        Span<byte>               span   = buffer.Memory.Span;
        int                      size   = encoding.GetBytes( data, span );
        span = span[..size];
        return Hash( span );
    }
    public static UInt128 Hash( this ReadOnlySpan<byte> data ) => XxHash128.HashToUInt128( data );
}
