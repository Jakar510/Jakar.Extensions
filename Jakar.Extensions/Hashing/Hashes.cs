// Jakar.Extensions :: Jakar.Extensions
// 06/06/2022  2:20 PM


namespace Jakar.Extensions;


public static class Hashes
{
    public const int MaxStackBytes = 32 * 1024; // tune as needed
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int GetHash<TValue>( this IEnumerable<TValue> values )
    {
        HashCode hash = new();
        foreach ( TValue value in values ) { hash.Add(value); }

        return hash.ToHashCode();
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
        Span<byte>               span   = owner.Values;

        for ( int i = 0; i < value.Length; i++ )
        {
            int   start = i * SIZE;
            Range range = new(start, start + SIZE);
            if ( BitConverter.TryWriteBytes( span[range], value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
        }

        ReadOnlySpan<byte> result = owner.Values[..length];
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
           Span<byte>               span   = owner.Values;

           for ( int i = 0; i < value.Length; i++ )
           {
               int   start = i * SIZE;
               Range range = new(start, start + SIZE);
               if ( BitConverter.TryWriteBytes( span[range], value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
           }

           ReadOnlySpan<byte> result = owner.Values[..length];
           return XxHash64.HashToUInt64( result, seed );
       }

    [Pure]
    public static ulong Hash( this ref readonly ReadOnlySpan<char> value, long seed = 0 )
    {
        const int                SIZE   = sizeof(char);
        int                      length = SIZE * value.Length;
        using IMemoryOwner<byte> owner  = MemoryPool<byte>.Shared.Rent( length );
        Span<byte>               span   = owner.Values;

        for ( int i = 0; i < value.Length; i++ )
        {
            int   start = i * SIZE;
            Range range = new(start, start + SIZE);
            if ( BitConverter.TryWriteBytes( span[range], value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
        }

        ReadOnlySpan<byte> result = owner.Values[..length];
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
        Span<byte>               span   = owner.Values;

        for ( int i = 0; i < value.Length; i++ )
        {
            int   start = i * size;
            Range range = new(start, start + size);
            if ( BitConverter.TryWriteBytes( span[range], value[i] ) is false ) { throw new InvalidOperationException( nameof(BitConverter.TryWriteBytes) ); }
        }

        ReadOnlySpan<byte> result = owner.Values[..length];
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
        if ( data.IsT0 ) { return data.AsT0.GetHash(); }

        if ( data.IsT1 ) { return data.AsT1.GetHash(); }

        throw new InvalidOperationException("Invalid data type");
    }
    public static string GetHash( this OneOf<ReadOnlyMemory<byte>, byte[], string> data )
    {
        if ( data.IsT0 )
        {
            ReadOnlySpan<byte> span = data.AsT0.Span;
            return span.GetHash();
        }

        if ( data.IsT1 ) { return data.AsT1.GetHash(); }

        if ( data.IsT2 ) { return data.AsT2.GetHash(); }

        throw new InvalidOperationException("Invalid data type");
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static string GetHash( this byte[] data )
    {
        ReadOnlySpan<byte> span = data;
        return span.GetHash();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static string GetHash( this string data )
    {
        ReadOnlySpan<char> span = data;
        return span.GetHash(Encoding.Default);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static string GetHash( this ref readonly ReadOnlySpan<byte> data )                    => data.Hash_SHA256();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static string GetHash( this ref readonly ReadOnlySpan<char> data, Encoding encoding ) => data.Hash_SHA256(encoding);


    [Pure] public static byte[] ToBytes( this string data, Encoding? encoding = null )
    {
        ReadOnlySpan<char> span = data;
        return span.ToBytes(encoding);
    }


    /// <remarks> Encodes straight into the result array (no pooled intermediate and second copy). </remarks>
    [Pure] public static byte[] ToBytes( this ref readonly ReadOnlySpan<char> data, Encoding? encoding = null )
    {
        encoding ??= Encoding.Default;
        byte[] result = new byte[encoding.GetByteCount(data)];
        encoding.GetBytes(data, result);
        return result;
    }


    // The Hash_* helpers use the static one-shot HashData APIs: no HashAlgorithm instance is created or disposed, the digest is written to the stack,
    // and text is encoded into a stack or pooled buffer instead of a new array. Output (uppercase hex, no separators) is unchanged.

    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public static string Hash_MD5( this ref readonly ReadOnlySpan<byte> data )                            => ToHex(HashKind.MD5, data);
    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public static string Hash_MD5( this ref readonly ReadOnlySpan<char> data, Encoding? encoding = null ) => ToHex(HashKind.MD5, data, encoding);
    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public static string Hash_MD5( this              string             data, Encoding? encoding = null ) => ToHex(HashKind.MD5, data, encoding);


    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static string Hash_SHA1( this ref readonly ReadOnlySpan<byte> data )                            => ToHex(HashKind.SHA1, data);
    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static string Hash_SHA1( this ref readonly ReadOnlySpan<char> data, Encoding? encoding = null ) => ToHex(HashKind.SHA1, data, encoding);
    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static string Hash_SHA1( this              string             data, Encoding? encoding = null ) => ToHex(HashKind.SHA1, data, encoding);


    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static string Hash_SHA256( this ref readonly ReadOnlySpan<byte> data )                            => ToHex(HashKind.SHA256, data);
    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static string Hash_SHA256( this ref readonly ReadOnlySpan<char> data, Encoding? encoding = null ) => ToHex(HashKind.SHA256, data, encoding);
    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static string Hash_SHA256( this              string             data, Encoding? encoding = null ) => ToHex(HashKind.SHA256, data, encoding);


    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static string Hash_SHA384( this ref readonly ReadOnlySpan<byte> data )                            => ToHex(HashKind.SHA384, data);
    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static string Hash_SHA384( this ref readonly ReadOnlySpan<char> data, Encoding? encoding = null ) => ToHex(HashKind.SHA384, data, encoding);
    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static string Hash_SHA384( this              string             data, Encoding? encoding = null ) => ToHex(HashKind.SHA384, data, encoding);


    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static string Hash_SHA512( this ref readonly ReadOnlySpan<byte> data )                            => ToHex(HashKind.SHA512, data);
    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static string Hash_SHA512( this ref readonly ReadOnlySpan<char> data, Encoding? encoding = null ) => ToHex(HashKind.SHA512, data, encoding);
    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static string Hash_SHA512( this              string             data, Encoding? encoding = null ) => ToHex(HashKind.SHA512, data, encoding);


    /// <summary> Hashes <paramref name="data"/> with <paramref name="hasher"/> and formats it like <see cref="BitConverter.ToString(byte[])"/> (<c>AB-CD-...</c>). </summary>
    /// <remarks> The data is already in memory, so it is hashed directly instead of being copied into a <see cref="MemoryStream"/> and hashed "asynchronously". Failures still surface through the returned task. </remarks>
    public static ValueTask<string> HashAsync( this HashAlgorithm hasher, ReadOnlyMemory<byte> data )
    {
        try { return new ValueTask<string>(ToDashedHex(hasher, data.Span)); }
        catch ( Exception e ) { return ValueTask.FromException<string>(e); }
    }


    public static UInt128 Hash( this ref readonly ReadOnlySpan<char> data, Encoding encoding )
    {
        int                     length = ( encoding.GetByteCount(data) );
        using ArrayBuffer<byte> owner  = new(length);
        Span<byte>              span   = owner.Span;
        int                     size   = encoding.GetBytes(data, span);
        ReadOnlySpan<byte>      result = span[..size];
        return result.Hash();
    }
    public static UInt128 Hash( this ref readonly ReadOnlySpan<byte> data ) => XxHash128.HashToUInt128(data);



    extension( string value )
    {
        [Pure] public UInt128 Hash128( long seed = 0 )
        {
            ReadOnlySpan<char> result = value;
            return result.Hash128(seed);
        }
        [Pure] public ulong Hash( long seed = 0 )
        {
            ReadOnlySpan<char> result = value;
            return result.Hash(seed);
        }
    }



    extension( ref readonly ReadOnlySpan<string> values )
    {
        [Pure] public UInt128 Hash128( long seed = 0 )
        {
            using ArrayBuffer<byte> owner = Concatenate(values, out int length);
            return XxHash128.HashToUInt128(owner.Span[..length], seed);
        }

        [Pure] public ulong Hash( long seed = 0 )
        {
            using ArrayBuffer<byte> owner = Concatenate(values, out int length);
            return XxHash64.HashToUInt64(owner.Span[..length], seed);
        }
    }



    extension<TValue>( ref readonly ReadOnlySpan<TValue> value )
        where TValue : unmanaged
    {
        /// <remarks> On little-endian machines the values' bytes are already in the normalized layout, so they are hashed in place without copying. </remarks>
        [Pure] public UInt128 Hash128( long seed = 0 )
        {
            if ( value.IsEmpty ) { return UInt128.Zero; }

            if ( BitConverter.IsLittleEndian ) { return XxHash128.HashToUInt128(MemoryMarshal.AsBytes(value), seed); }

            using ArrayBuffer<byte> owner = ToLittleEndianBytes(value, out int length);
            return XxHash128.HashToUInt128(owner.Span[..length], seed);
        }

        /// <remarks> On little-endian machines the values' bytes are already in the normalized layout, so they are hashed in place without copying. </remarks>
        [Pure] public ulong Hash( long seed = 0 )
        {
            if ( value.IsEmpty ) { return 0; }

            if ( BitConverter.IsLittleEndian ) { return XxHash64.HashToUInt64(MemoryMarshal.AsBytes(value), seed); }

            using ArrayBuffer<byte> owner = ToLittleEndianBytes(value, out int length);
            return XxHash64.HashToUInt64(owner.Span[..length], seed);
        }
    }



    extension( HashAlgorithm hasher )
    {
        public string Hash( Encoding? encoding, params ReadOnlySpan<char> data )
        {
            encoding ??= Encoding.Default;
            byte[]? rented = null;
            int     max    = encoding.GetMaxByteCount(data.Length);

            Span<byte> buffer = max <= STACK_BYTES
                                    ? stackalloc byte[max]
                                    : rented = ArrayPool<byte>.Shared.Rent(encoding.GetByteCount(data));

            int written = encoding.GetBytes(data, buffer);

            try { return hasher.Hash(buffer[..written]); }
            finally
            {
                buffer[..written].Clear();
                if ( rented is not null ) { ArrayPool<byte>.Shared.Return(rented); }
            }
        }
        public string Hash( params ReadOnlySpan<byte> data )
        {
            int size = HashSize(hasher);

            Span<byte> buffer = size <= MAX_DIGEST_BYTES
                                    ? stackalloc byte[MAX_DIGEST_BYTES]
                                    : new byte[size];

            if ( !hasher.TryComputeHash(data, buffer, out int bytesWritten) ) { throw new InvalidOperationException($"{hasher.GetType().Name}.{nameof(hasher.TryComputeHash)} failed"); }

            return Convert.ToHexString(buffer[..bytesWritten]);
        }
    }



    extension( ReadOnlyMemory<byte> data )
    {
        /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
        public ValueTask<string> HashAsync_MD5()    => ToDashedHexAsync(HashKind.MD5,    data.Span);
        /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
        public ValueTask<string> HashAsync_SHA1()   => ToDashedHexAsync(HashKind.SHA1,   data.Span);
        /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
        public ValueTask<string> HashAsync_SHA256() => ToDashedHexAsync(HashKind.SHA256, data.Span);
        /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
        public ValueTask<string> HashAsync_SHA384() => ToDashedHexAsync(HashKind.SHA384, data.Span);
        /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
        public ValueTask<string> HashAsync_SHA512() => ToDashedHexAsync(HashKind.SHA512, data.Span);
    }



    extension( byte[] data )
    {
        /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
        public ValueTask<string> HashAsync_MD5()    => ToDashedHexAsync(HashKind.MD5,    data);
        /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
        public ValueTask<string> HashAsync_SHA1()   => ToDashedHexAsync(HashKind.SHA1,   data);
        /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
        public ValueTask<string> HashAsync_SHA256() => ToDashedHexAsync(HashKind.SHA256, data);
        /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
        public ValueTask<string> HashAsync_SHA384() => ToDashedHexAsync(HashKind.SHA384, data);
        /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
        public ValueTask<string> HashAsync_SHA512() => ToDashedHexAsync(HashKind.SHA512, data);
        public ValueTask<string> HashAsync( HashAlgorithm hasher )
        {
            try
            {
                ArgumentNullException.ThrowIfNull(data);
                return new ValueTask<string>(ToDashedHex(hasher, data));
            }
            catch ( Exception e ) { return ValueTask.FromException<string>(e); }
        }
    }



    extension( string value )
    {
        /// <summary> The decoded bytes if <paramref name="value"/> is valid base64 (same rules as <see cref="Convert.FromBase64String"/>), otherwise the string itself. </summary>
        /// <remarks> Uses <see cref="Convert.TryFromBase64String"/>, so non-base64 input no longer costs a thrown and caught <see cref="FormatException"/>. </remarks>
        public OneOf<byte[], string> TryGetData()
        {
            ArgumentNullException.ThrowIfNull(value);

            int     max    = ( value.Length / 4 + 1 ) * 3;
            byte[]? rented = null;

            Span<byte> buffer = max <= STACK_BYTES
                                    ? stackalloc byte[max]
                                    : rented = ArrayPool<byte>.Shared.Rent(max);

            try
            {
                return Convert.TryFromBase64String(value, buffer, out int written)
                           ? buffer[..written].ToArray()
                           : value;
            }
            finally
            {
                if ( rented is not null ) { ArrayPool<byte>.Shared.Return(rented, true); }
            }
        }
        public UInt128 Hash( Encoding encoding )
        {
            OneOf<byte[], string> one = value.TryGetData();

            if ( one.IsT0 )
            {
                ReadOnlySpan<byte> span = one.AsT0;
                return span.Hash();
            }
            else
            {
                ReadOnlySpan<char> span = one.AsT1;
                return span.Hash(encoding);
            }
        }
    }



    // ─── Helpers ─────────────────────────────────────────────────────────────

    private const int STACK_BYTES      = 1024;
    private const int MAX_DIGEST_BYTES = 64; // SHA-512



    private enum HashKind : byte
    {
        MD5,
        SHA1,
        SHA256,
        SHA384,
        SHA512
    }



    private static int HashData( HashKind kind, ReadOnlySpan<byte> source, Span<byte> destination ) => kind switch
                                                                                                       {
                                                                                                           HashKind.MD5    => MD5.HashData(source, destination),
                                                                                                           HashKind.SHA1   => SHA1.HashData(source, destination),
                                                                                                           HashKind.SHA256 => SHA256.HashData(source, destination),
                                                                                                           HashKind.SHA384 => SHA384.HashData(source, destination),
                                                                                                           _               => SHA512.HashData(source, destination)
                                                                                                       };


    private static string ToHex( HashKind kind, ReadOnlySpan<byte> data )
    {
        Span<byte> hash = stackalloc byte[MAX_DIGEST_BYTES];
        int        size = HashData(kind, data, hash);
        return Convert.ToHexString(hash[..size]);
    }
    private static string ToHex( HashKind kind, ReadOnlySpan<char> data, Encoding? encoding )
    {
        encoding ??= Encoding.Default;
        byte[]? rented = null;
        int     max    = encoding.GetMaxByteCount(data.Length);

        Span<byte> buffer = max <= STACK_BYTES
                                ? stackalloc byte[max]
                                : rented = ArrayPool<byte>.Shared.Rent(encoding.GetByteCount(data));

        int written = encoding.GetBytes(data, buffer);

        try { return ToHex(kind, buffer[..written]); }
        finally
        {
            // The input may be a secret (e.g. a password being hashed), so don't leave it in a pooled array.
            buffer[..written].Clear();
            if ( rented is not null ) { ArrayPool<byte>.Shared.Return(rented); }
        }
    }


    private static ValueTask<string> ToDashedHexAsync( HashKind kind, byte[]? data )
    {
        if ( data is null ) { return ValueTask.FromException<string>(new ArgumentNullException(nameof(data))); }

        return ToDashedHexAsync(kind, data.AsSpan());
    }
    private static ValueTask<string> ToDashedHexAsync( HashKind kind, ReadOnlySpan<byte> data )
    {
        try
        {
            Span<byte> hash = stackalloc byte[MAX_DIGEST_BYTES];
            int        size = HashData(kind, data, hash);
            return new ValueTask<string>(ToDashedHex(hash[..size]));
        }
        catch ( Exception e ) { return ValueTask.FromException<string>(e); }
    }
    private static string ToDashedHex( HashAlgorithm hasher, ReadOnlySpan<byte> data )
    {
        int size = HashSize(hasher);

        Span<byte> buffer = size <= MAX_DIGEST_BYTES
                                ? stackalloc byte[MAX_DIGEST_BYTES]
                                : new byte[size];

        if ( !hasher.TryComputeHash(data, buffer, out int written) ) { throw new InvalidOperationException($"{hasher.GetType().Name}.{nameof(hasher.TryComputeHash)} failed"); }

        return ToDashedHex(buffer[..written]);
    }


    /// <summary> Same output as <see cref="BitConverter.ToString(byte[])"/>: uppercase hex pairs separated by '-'. </summary>
    private static string ToDashedHex( ReadOnlySpan<byte> bytes )
    {
        if ( bytes.IsEmpty ) { return EMPTY; }

        return string.Create(bytes.Length * 3 - 1,
                             bytes,
                             static ( chars, source ) =>
                             {
                                 for ( int i = 0, j = 0; i < source.Length; i++, j += 3 )
                                 {
                                     byte value = source[i];
                                     chars[j]     = HexConverter.ToCharUpper(value >> 4);
                                     chars[j + 1] = HexConverter.ToCharUpper(value);
                                     if ( j + 2 < chars.Length ) { chars[j + 2] = '-'; }
                                 }
                             });
    }


    /// <summary> The digest size in bytes, or <see cref="MAX_DIGEST_BYTES"/> when the algorithm doesn't report one. </summary>
    private static int HashSize( HashAlgorithm hasher ) => hasher.HashSize > 0
                                                               ? ( hasher.HashSize + 7 ) / 8
                                                               : MAX_DIGEST_BYTES;


    /// <summary>
    ///     UTF-8 bytes of all <paramref name="values"/> back to back, zero-padded to at least 2 bytes per char.
    ///     The padding keeps results identical to the previous implementation for ASCII input (it sized its buffer at 2 bytes per char and hashed the unused tail);
    ///     unlike it, the tail is now always zeroed (it was read from a non-cleared pooled array) and multi-byte characters no longer overlap or overflow.
    /// </summary>
    [MustDisposeResource] public static ArrayBuffer<byte> Concatenate( ReadOnlySpan<string> values, out int length )
    {
        Encoding encoding = Encoding.Default;
        int      chars    = 0;
        int      bytes    = 0;

        foreach ( string value in values )
        {
            chars += value.Length;
            bytes += encoding.GetByteCount(value);
        }

        length = Math.Max(chars * sizeof(char), bytes);
        ArrayBuffer<byte> owner  = new(length);
        Span<byte>        buffer = owner.Span[..length];
        int               offset = 0;

        foreach ( string value in values )
        {
            Span<byte> destination = buffer[offset..];
            int        written     = encoding.GetBytes(value, destination);
            if ( !BitConverter.IsLittleEndian ) { destination[..written].Reverse(); } // kept from the previous implementation

            offset += written;
        }

        buffer[offset..].Clear();
        return owner;
    }


    private static unsafe ArrayBuffer<byte> ToLittleEndianBytes<TValue>( ReadOnlySpan<TValue> value, out int length )
        where TValue : unmanaged
    {
        int size = sizeof(TValue);
        length = value.Length * size;
        ArrayBuffer<byte> owner  = new(length);
        Span<byte>        buffer = owner.Span[..length];

        for ( int i = 0; i < value.Length; i++ )
        {
            Span<byte> span = buffer.Slice(i * size, size);
            MemoryMarshal.Write(span, in value[i]);
            span.Reverse();
        }

        return owner;
    }
}
