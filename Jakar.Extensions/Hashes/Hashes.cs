// Jakar.Extensions :: Jakar.Extensions
// 06/06/2022  2:20 PM


namespace Jakar.Extensions;


public static class Hashes
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int GetHash<TValue>( this IEnumerable<TValue> values )
    {
        HashCode hash = new();
        foreach ( TValue value in values ) { hash.Add( value ); }

        return hash.ToHashCode();
    }


    [Pure]
    public static UInt128 Hash128( this string value, long seed = 0 )
    {
        ReadOnlySpan<char> result = value;
        return result.Hash128( seed );
    }
    [Pure]
    public static ulong Hash( this string value, long seed = 0 )
    {
        ReadOnlySpan<char> result = value;
        return result.Hash( seed );
    }


    [Pure]
    public static UInt128 Hash128( this ref readonly ReadOnlySpan<string> values, long seed = 0 )
    {
        int                      length = values.Sum( static x => x.Length );
        using IMemoryOwner<char> owner  = MemoryPool<char>.Shared.Rent( length );
        Span<char>               span   = owner.Memory.Span;

        foreach ( string value in values )
        {
            value.CopyTo( span );
            span = span[value.Length..];
        }

        ReadOnlySpan<char> result = owner.Memory.Span[..length];
        return result.Hash128( seed );
    }
    [Pure]
    public static ulong Hash( this ref readonly ReadOnlySpan<string> values, long seed = 0 )
    {
        int                      length = values.Sum( static x => x.Length );
        using IMemoryOwner<char> owner  = MemoryPool<char>.Shared.Rent( length );
        Span<char>               span   = owner.Memory.Span;

        foreach ( string value in values )
        {
            value.CopyTo( span );
            span = span[value.Length..];
        }

        ReadOnlySpan<char> result = owner.Memory.Span[..length];
        return result.Hash( seed );
    }


    [Pure]
    public static unsafe UInt128 Hash128<TValue>( this ref readonly ReadOnlySpan<TValue> value, long seed = 0 )
        where TValue : unmanaged
    {
        int                      size   = sizeof(TValue);
        int                      length = size * value.Length;
        using IMemoryOwner<byte> owner  = MemoryPool<byte>.Shared.Rent( length );
        Span<byte>               span   = owner.Memory.Span;

        for ( int i = 0; i < value.Length; i++ )
        {
            int        start   = i * size;
            Range      range   = new(start, start + size);
            Span<byte> section = span[range];
            Unsafe.WriteUnaligned( ref MemoryMarshal.GetReference( section ), value[i] );
        }

        ReadOnlySpan<byte> result = owner.Memory.Span[..length];
        return XxHash128.HashToUInt128( result, seed );
    }

    [Pure]
    public static unsafe ulong Hash<TValue>( this ref readonly ReadOnlySpan<TValue> value, long seed = 0 )
        where TValue : unmanaged
    {
        int                      size   = sizeof(TValue);
        int                      length = size * value.Length;
        using IMemoryOwner<byte> owner  = MemoryPool<byte>.Shared.Rent( length );
        Span<byte>               span   = owner.Memory.Span;

        for ( int i = 0; i < value.Length; i++ )
        {
            int        start   = i * size;
            Range      range   = new(start, start + size);
            Span<byte> section = span[range];
            Unsafe.WriteUnaligned( ref MemoryMarshal.GetReference( section ), value[i] );
        }

        ReadOnlySpan<byte> result = owner.Memory.Span[..length];
        return XxHash64.HashToUInt64( result, seed );
    }


    /*
    [Pure]
    public static UInt128 Hash128( this ref readonly ReadOnlySpan<bool> value, long seed = 0 )
    {
        const int SIZE   = sizeof(bool);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new(start, start + SIZE);
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ref readonly ReadOnlySpan<char> value, long seed = 0 )
    {
        const int SIZE   = sizeof(char);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new(start, start + SIZE);
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ref readonly ReadOnlySpan<short> value, long seed = 0 )
    {
        const int SIZE   = sizeof(short);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new(start, start + SIZE);
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ref readonly ReadOnlySpan<ushort> value, long seed = 0 )
    {
        const int SIZE   = sizeof(ushort);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new(start, start + SIZE);
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ref readonly ReadOnlySpan<int> value, long seed = 0 )
    {
        const int SIZE   = sizeof(int);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new(start, start + SIZE);
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ref readonly ReadOnlySpan<uint> value, long seed = 0 )
    {
        const int SIZE   = sizeof(uint);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new(start, start + SIZE);
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ref readonly ReadOnlySpan<long> value, long seed = 0 )
    {
        const int SIZE   = sizeof(long);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new(start, start + SIZE);
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ref readonly ReadOnlySpan<ulong> value, long seed = 0 )
    {
        const int SIZE   = sizeof(ulong);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new(start, start + SIZE);
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ref readonly ReadOnlySpan<Half> value, long seed = 0 )
    {
        const int SIZE   = sizeof(bool);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new(start, start + SIZE);
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ref readonly ReadOnlySpan<float> value, long seed = 0 )
    {
        const int SIZE   = sizeof(float);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new(start, start + SIZE);
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash128.HashToUInt128( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static UInt128 Hash128( this ref readonly ReadOnlySpan<double> value, long seed = 0 )
    {
        const int                SIZE   = sizeof(double);
        int                      length = SIZE * value.Length;
        using IMemoryOwner<byte> owner  = MemoryPool<byte>.Shared.Rent( length );
        Span<byte>               span   = owner.Memory.Span;

        for ( int i = 0; i < value.Length; i++ )
        {
            int   start = i * SIZE;
            Range range = new(start, start + SIZE);
            if ( BitConverter.TryWriteBytes( span[range], value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
        }

        ReadOnlySpan<byte> result = owner.Memory.Span[..length];
        return XxHash64.HashToUInt64( result, seed );
    }
    */

    /*
       [Pure]
       public static ulong Hash( this ref readonly ReadOnlySpan<bool> value, long seed = 0 )
       {
           const int                SIZE   = sizeof(bool);
           int                      length = SIZE * value.Length;
           using IMemoryOwner<byte> owner  = MemoryPool<byte>.Shared.Rent( length );
           Span<byte>               span   = owner.Memory.Span;

           for ( int i = 0; i < value.Length; i++ )
           {
               int   start = i * SIZE;
               Range range = new(start, start + SIZE);
               if ( BitConverter.TryWriteBytes( span[range], value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
           }

           ReadOnlySpan<byte> result = owner.Memory.Span[..length];
           return XxHash64.HashToUInt64( result, seed );
       }

    [Pure]
    public static ulong Hash( this ref readonly ReadOnlySpan<char> value, long seed = 0 )
    {
        const int                SIZE   = sizeof(char);
        int                      length = SIZE * value.Length;
        using IMemoryOwner<byte> owner  = MemoryPool<byte>.Shared.Rent( length );
        Span<byte>               span   = owner.Memory.Span;

        for ( int i = 0; i < value.Length; i++ )
        {
            int   start = i * SIZE;
            Range range = new(start, start + SIZE);
            if ( BitConverter.TryWriteBytes( span[range], value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
        }

        ReadOnlySpan<byte> result = owner.Memory.Span[..length];
        return XxHash64.HashToUInt64( result, seed );
    }

    [Pure]
    public static ulong Hash( this ref readonly ReadOnlySpan<short> value, long seed = 0 )
    {
        const int SIZE   = sizeof(short);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new(start, start + SIZE);
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static ulong Hash( this ref readonly ReadOnlySpan<ushort> value, long seed = 0 )
    {
        const int SIZE   = sizeof(ushort);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new(start, start + SIZE);
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static ulong Hash( this ref readonly ReadOnlySpan<int> value, long seed = 0 )
    {
        const int SIZE   = sizeof(int);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new(start, start + SIZE);
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static ulong Hash( this ref readonly ReadOnlySpan<uint> value, long seed = 0 )
    {
        const int SIZE   = sizeof(uint);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new(start, start + SIZE);
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static ulong Hash( this ref readonly ReadOnlySpan<long> value, long seed = 0 )
    {
        const int SIZE   = sizeof(long);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new(start, start + SIZE);
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static ulong Hash( this ref readonly ReadOnlySpan<ulong> value, long seed = 0 )
    {
        const int SIZE   = sizeof(ulong);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new(start, start + SIZE);
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static ulong Hash( this ref readonly ReadOnlySpan<float> value, long seed = 0 )
    {
        const int SIZE   = sizeof(float);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new(start, start + SIZE);
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }

    [Pure]
    public static unsafe ulong Hash( this ref readonly ReadOnlySpan<Half> value, long seed = 0 )
    {
        int                      size   = sizeof(Half);
        int                      length = size * value.Length;
        using IMemoryOwner<byte> owner  = MemoryPool<byte>.Shared.Rent( length );
        Span<byte>               span   = owner.Memory.Span;

        for ( int i = 0; i < value.Length; i++ )
        {
            int   start = i * size;
            Range range = new(start, start + size);
            if ( BitConverter.TryWriteBytes( span[range], value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
        }

        ReadOnlySpan<byte> result = owner.Memory.Span[..length];
        return XxHash64.HashToUInt64( result, seed );
    }

    [Pure]
    public static ulong Hash( this ref readonly ReadOnlySpan<double> value, long seed = 0 )
    {
        const int SIZE   = sizeof(double);
        byte[]    buffer = ArrayPool<byte>.Shared.Rent( SIZE * value.Length );

        try
        {
            for ( int i = 0; i < value.Length; i++ )
            {
                int        start = i * SIZE;
                Range      range = new(start, start + SIZE);
                Span<byte> span  = buffer.AsSpan( range );
                if ( BitConverter.TryWriteBytes( span, value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
            }

            return XxHash64.HashToUInt64( buffer, seed );
        }
        finally { ArrayPool<byte>.Shared.Return( buffer ); }
    }
    */


    public static string GetHash( this OneOf<byte[], string> data )
    {
        if ( data.IsT0 ) { return GetHash( data.AsT0 ); }

        if ( data.IsT1 ) { return GetHash( data.AsT1 ); }

        throw new InvalidOperationException( "Invalid data type" );
    }
    public static string GetHash( this OneOf<ReadOnlyMemory<byte>, byte[], string> data )
    {
        if ( data.IsT0 )
        {
            ReadOnlySpan<byte> span = data.AsT0.Span;
            return span.GetHash();
        }

        if ( data.IsT1 ) { return GetHash( data.AsT1 ); }

        if ( data.IsT2 ) { return GetHash( data.AsT2 ); }

        throw new InvalidOperationException( "Invalid data type" );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetHash( this byte[] data )
    {
        ReadOnlySpan<byte> span = data;
        return span.GetHash();
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetHash( this string data )
    {
        ReadOnlySpan<char> span = data;
        return span.GetHash( Encoding.Default );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string GetHash( this ref readonly ReadOnlySpan<byte> data )                    => data.Hash_SHA256();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static string GetHash( this ref readonly ReadOnlySpan<char> data, Encoding encoding ) => data.Hash_SHA256( encoding );


    [Pure]
    public static byte[] ToBytes( this string data, Encoding? encoding = null )
    {
        ReadOnlySpan<char> span = data;
        return span.ToBytes( encoding );
    }


    [Pure]
    public static byte[] ToBytes( this ref readonly ReadOnlySpan<char> data, Encoding? encoding = null )
    {
        encoding ??= Encoding.Default;
        int                      count  = encoding.GetByteCount( data );
        using IMemoryOwner<byte> buffer = MemoryPool<byte>.Shared.Rent( count );
        Span<byte>               span   = buffer.Memory.Span[..count];
        encoding.GetBytes( data, span );
        return span.ToArray();
    }


    public static string Hash( this HashAlgorithm hasher, Encoding? encoding, params ReadOnlySpan<char> data ) => hasher.Hash( data.ToBytes( encoding ) );
    public static string Hash( this HashAlgorithm hasher, params ReadOnlySpan<byte> data )
    {
        using IMemoryOwner<byte> buffer = MemoryPool<byte>.Shared.Rent( UNICODE_CAPACITY );
        Span<byte>               span   = buffer.Memory.Span;
        if ( !hasher.TryComputeHash( data, span, out int bytesWritten ) ) { throw new InvalidOperationException( nameof(span) ); }

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
    public static string Hash_MD5( this ref readonly ReadOnlySpan<byte> data )
    {
        using var hasher = MD5.Create();
        return hasher.Hash( data );
    }
    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public static string Hash_MD5( this ref readonly ReadOnlySpan<char> data, Encoding? encoding = null )
    {
        using var hasher = MD5.Create();
        return hasher.Hash( encoding, data );
    }
    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public static string Hash_MD5( this string data, Encoding? encoding = null )
    {
        using var hasher = MD5.Create();
        return hasher.Hash( encoding, data );
    }


    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static string Hash_SHA1( this ref readonly ReadOnlySpan<byte> data )
    {
        using var hasher = SHA1.Create();
        return hasher.Hash( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static string Hash_SHA1( this ref readonly ReadOnlySpan<char> data, Encoding? encoding = null )
    {
        using var hasher = SHA1.Create();
        return hasher.Hash( encoding, data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static string Hash_SHA1( this string data, Encoding? encoding = null )
    {
        using var hasher = SHA1.Create();
        return hasher.Hash( encoding, data );
    }


    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static string Hash_SHA256( this ref readonly ReadOnlySpan<byte> data )
    {
        using var hasher = SHA256.Create();
        return hasher.Hash( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static string Hash_SHA256( this ref readonly ReadOnlySpan<char> data, Encoding? encoding = null )
    {
        using var hasher = SHA256.Create();
        return hasher.Hash( encoding, data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static string Hash_SHA256( this string data, Encoding? encoding = null )
    {
        using var hasher = SHA256.Create();
        return hasher.Hash( encoding, data );
    }


    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static string Hash_SHA384( this ref readonly ReadOnlySpan<byte> data )
    {
        using var hasher = SHA384.Create();
        return hasher.Hash( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static string Hash_SHA384( this ref readonly ReadOnlySpan<char> data, Encoding? encoding = null )
    {
        using var hasher = SHA384.Create();
        return hasher.Hash( encoding, data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static string Hash_SHA384( this string data, Encoding? encoding = null )
    {
        using var hasher = SHA384.Create();
        return hasher.Hash( encoding, data );
    }


    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static string Hash_SHA512( this ref readonly ReadOnlySpan<byte> data )
    {
        using var hasher = SHA512.Create();
        return hasher.Hash( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static string Hash_SHA512( this ref readonly ReadOnlySpan<char> data, Encoding? encoding = null )
    {
        using var hasher = SHA512.Create();
        return hasher.Hash( encoding, data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static string Hash_SHA512( this string data, Encoding? encoding = null )
    {
        using var hasher = SHA512.Create();
        return hasher.Hash( encoding, data );
    }


    public static async ValueTask<string> HashAsync( this HashAlgorithm hasher, ReadOnlyMemory<byte> data )
    {
        await using MemoryStream stream = new();
        await stream.WriteAsync( data );
        stream.Seek( 0, SeekOrigin.Begin );
        byte[] hash = await hasher.ComputeHashAsync( stream );

        return BitConverter.ToString( hash );
    }
    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public static async ValueTask<string> HashAsync_MD5( this ReadOnlyMemory<byte> data )
    {
        using var hasher = MD5.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static async ValueTask<string> HashAsync_SHA1( this ReadOnlyMemory<byte> data )
    {
        using var hasher = SHA1.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static async ValueTask<string> HashAsync_SHA256( this ReadOnlyMemory<byte> data )
    {
        using var hasher = SHA256.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static async ValueTask<string> HashAsync_SHA384( this ReadOnlyMemory<byte> data )
    {
        using var hasher = SHA384.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static async ValueTask<string> HashAsync_SHA512( this ReadOnlyMemory<byte> data )
    {
        using var hasher = SHA512.Create();
        return await hasher.HashAsync( data );
    }


    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public static async ValueTask<string> HashAsync_MD5( this byte[] data )
    {
        using var hasher = MD5.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static async ValueTask<string> HashAsync_SHA1( this byte[] data )
    {
        using var hasher = SHA1.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static async ValueTask<string> HashAsync_SHA256( this byte[] data )
    {
        using var hasher = SHA256.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static async ValueTask<string> HashAsync_SHA384( this byte[] data )
    {
        using var hasher = SHA384.Create();
        return await hasher.HashAsync( data );
    }
    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static async ValueTask<string> HashAsync_SHA512( this byte[] data )
    {
        using var hasher = SHA512.Create();
        return await hasher.HashAsync( data );
    }
    public static async ValueTask<string> HashAsync( this byte[] data, HashAlgorithm hasher )
    {
        await using MemoryStream stream = new(data);
        byte[]                   hash   = await hasher.ComputeHashAsync( stream );
        return BitConverter.ToString( hash );
    }


    public static OneOf<byte[], string> TryGetData( this string value )
    {
        try { return Convert.FromBase64String( value ); }
        catch ( FormatException ) { return value; }
    }


    public static UInt128 Hash( this string data, Encoding encoding )
    {
        OneOf<byte[], string> one = data.TryGetData();

        if ( one.IsT0 )
        {
            ReadOnlySpan<byte> span = one.AsT0;
            return span.Hash();
        }
        else
        {
            ReadOnlySpan<char> span = one.AsT1;
            return span.Hash( encoding );
        }
    }
    public static UInt128 Hash( this ref readonly ReadOnlySpan<char> data, Encoding encoding )
    {
        using IMemoryOwner<byte> buffer = MemoryPool<byte>.Shared.Rent( encoding.GetByteCount( data ) );
        Span<byte>               span   = buffer.Memory.Span;
        int                      size   = encoding.GetBytes( data, span );
        ReadOnlySpan<byte>       result = span[..size];
        return result.Hash();
    }
    public static UInt128 Hash( this ref readonly ReadOnlySpan<byte> data ) => XxHash128.HashToUInt128( data );
}
