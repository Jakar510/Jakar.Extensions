// Jakar.Extensions :: Jakar.Extensions
// 06/06/2022  2:20 PM


namespace Jakar.Extensions;


public static class Hashes
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int GetHash<TValue>( this IEnumerable<TValue> values )
    {
        HashCode hash = new();
        foreach ( TValue value in values ) { hash.Add(value); }

        return hash.ToHashCode();
    }



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
            int                     length = values.Sum(static x => x.Length);
            using ArrayBuffer<char> owner  = new(length);
            Span<char>              span   = owner.Span;

            foreach ( string value in values )
            {
                value.CopyTo(span);
                span = span[value.Length..];
            }

            ReadOnlySpan<char> result = owner.Span[..length];
            return result.Hash128(seed);
        }
        [Pure] public ulong Hash( long seed = 0 )
        {
            int                     length = values.Sum(static x => x.Length);
            using ArrayBuffer<char> owner  = new(length);
            Span<char>              span   = owner.Span;

            foreach ( string value in values )
            {
                value.CopyTo(span);
                span = span[value.Length..];
            }

            ReadOnlySpan<char> result = owner.Values[..length];
            return result.Hash(seed);
        }
    }



    extension<TValue>( ref readonly ReadOnlySpan<TValue> value )
        where TValue : unmanaged
    {
        [Pure] public unsafe UInt128 Hash128( long seed = 0 )
        {
            int                     size   = sizeof(TValue);
            int                     length = size * value.Length;
            using ArrayBuffer<byte> owner  = new(length);
            Span<byte>              span   = owner.Span;

            for ( int i = 0; i < value.Length; i++ )
            {
                int        start   = i * size;
                Range      range   = new(start, start + size);
                Span<byte> section = span[range];
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(section), value[i]);
            }

            ReadOnlySpan<byte> result = owner.Span[..length];
            return XxHash128.HashToUInt128(result, seed);
        }
        [Pure] public unsafe ulong Hash( long seed = 0 )
        {
            int                     size   = sizeof(TValue);
            int                     length = size * value.Length;
            using ArrayBuffer<byte> owner  = new(length);
            Span<byte>              span   = owner.Span;

            for ( int i = 0; i < value.Length; i++ )
            {
                int        start   = i * size;
                Range      range   = new(start, start + size);
                Span<byte> section = span[range];
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(section), value[i]);
            }

            ReadOnlySpan<byte> result = owner.Span[..length];
            return XxHash64.HashToUInt64(result, seed);
        }
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
        if ( data.IsT0 ) { return GetHash(data.AsT0); }

        if ( data.IsT1 ) { return GetHash(data.AsT1); }

        throw new InvalidOperationException("Invalid data type");
    }
    public static string GetHash( this OneOf<ReadOnlyMemory<byte>, byte[], string> data )
    {
        if ( data.IsT0 )
        {
            ReadOnlySpan<byte> span = data.AsT0.Span;
            return span.GetHash();
        }

        if ( data.IsT1 ) { return GetHash(data.AsT1); }

        if ( data.IsT2 ) { return GetHash(data.AsT2); }

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


    [Pure] public static byte[] ToBytes( this ref readonly ReadOnlySpan<char> data, Encoding? encoding = null )
    {
        encoding ??= Encoding.Default;
        int                     count = encoding.GetByteCount(data);
        using ArrayBuffer<byte> owner = new(count);
        Span<byte>              span  = owner.Span[..count];
        encoding.GetBytes(data, span);
        return span.ToArray();
    }



    extension( HashAlgorithm hasher )
    {
        public string Hash( Encoding? encoding, params ReadOnlySpan<char> data ) => hasher.Hash(data.ToBytes(encoding));
        public string Hash( params ReadOnlySpan<byte> data )
        {
            using ArrayBuffer<byte> owner = new(HASH);
            Span<byte>              span  = owner.Span;
            if ( !hasher.TryComputeHash(data, span, out int bytesWritten) ) { throw new InvalidOperationException($"{hasher.GetType().Name}.{nameof(hasher.TryComputeHash)} failed"); }

            return Convert.ToHexString(span[..bytesWritten]);
        }
    }



    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public static string Hash_MD5( this ref readonly ReadOnlySpan<byte> data )
    {
        using MD5 hasher = MD5.Create();
        return hasher.Hash(data);
    }
    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public static string Hash_MD5( this ref readonly ReadOnlySpan<char> data, Encoding? encoding = null )
    {
        using MD5 hasher = MD5.Create();
        return hasher.Hash(encoding, data);
    }
    /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
    public static string Hash_MD5( this string data, Encoding? encoding = null )
    {
        using MD5 hasher = MD5.Create();
        return hasher.Hash(encoding, data);
    }


    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static string Hash_SHA1( this ref readonly ReadOnlySpan<byte> data )
    {
        using SHA1 hasher = SHA1.Create();
        return hasher.Hash(data);
    }
    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static string Hash_SHA1( this ref readonly ReadOnlySpan<char> data, Encoding? encoding = null )
    {
        using SHA1 hasher = SHA1.Create();
        return hasher.Hash(encoding, data);
    }
    /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
    public static string Hash_SHA1( this string data, Encoding? encoding = null )
    {
        using SHA1 hasher = SHA1.Create();
        return hasher.Hash(encoding, data);
    }


    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static string Hash_SHA256( this ref readonly ReadOnlySpan<byte> data )
    {
        using SHA256 hasher = SHA256.Create();
        return hasher.Hash(data);
    }
    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static string Hash_SHA256( this ref readonly ReadOnlySpan<char> data, Encoding? encoding = null )
    {
        using SHA256 hasher = SHA256.Create();
        return hasher.Hash(encoding, data);
    }
    /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
    public static string Hash_SHA256( this string data, Encoding? encoding = null )
    {
        using SHA256 hasher = SHA256.Create();
        return hasher.Hash(encoding, data);
    }


    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static string Hash_SHA384( this ref readonly ReadOnlySpan<byte> data )
    {
        using SHA384 hasher = SHA384.Create();
        return hasher.Hash(data);
    }
    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static string Hash_SHA384( this ref readonly ReadOnlySpan<char> data, Encoding? encoding = null )
    {
        using SHA384 hasher = SHA384.Create();
        return hasher.Hash(encoding, data);
    }
    /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
    public static string Hash_SHA384( this string data, Encoding? encoding = null )
    {
        using SHA384 hasher = SHA384.Create();
        return hasher.Hash(encoding, data);
    }


    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static string Hash_SHA512( this ref readonly ReadOnlySpan<byte> data )
    {
        using SHA512 hasher = SHA512.Create();
        return hasher.Hash(data);
    }
    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static string Hash_SHA512( this ref readonly ReadOnlySpan<char> data, Encoding? encoding = null )
    {
        using SHA512 hasher = SHA512.Create();
        return hasher.Hash(encoding, data);
    }
    /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
    public static string Hash_SHA512( this string data, Encoding? encoding = null )
    {
        using SHA512 hasher = SHA512.Create();
        return hasher.Hash(encoding, data);
    }


    public static async ValueTask<string> HashAsync( this HashAlgorithm hasher, ReadOnlyMemory<byte> data )
    {
        await using MemoryStream stream = new();

        await stream.WriteAsync(data)
                    .ConfigureAwait(false);

        stream.Seek(0, SeekOrigin.Begin);

        byte[] hash = await hasher.ComputeHashAsync(stream)
                                  .ConfigureAwait(false);

        return BitConverter.ToString(hash);
    }



    extension( ReadOnlyMemory<byte> data )
    {
        /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
        public async ValueTask<string> HashAsync_MD5()
        {
            using MD5 hasher = MD5.Create();

            return await hasher.HashAsync(data)
                               .ConfigureAwait(false);
        }
        /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
        public async ValueTask<string> HashAsync_SHA1()
        {
            using SHA1 hasher = SHA1.Create();

            return await hasher.HashAsync(data)
                               .ConfigureAwait(false);
        }
        /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
        public async ValueTask<string> HashAsync_SHA256()
        {
            using SHA256 hasher = SHA256.Create();

            return await hasher.HashAsync(data)
                               .ConfigureAwait(false);
        }
        /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
        public async ValueTask<string> HashAsync_SHA384()
        {
            using SHA384 hasher = SHA384.Create();

            return await hasher.HashAsync(data)
                               .ConfigureAwait(false);
        }
        /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
        public async ValueTask<string> HashAsync_SHA512()
        {
            using SHA512 hasher = SHA512.Create();

            return await hasher.HashAsync(data)
                               .ConfigureAwait(false);
        }
    }



    extension( byte[] data )
    {
        /// <summary> Calculates a file hash using <see cref="MD5"/> </summary>
        public async ValueTask<string> HashAsync_MD5()
        {
            using MD5 hasher = MD5.Create();

            return await hasher.HashAsync(data)
                               .ConfigureAwait(false);
        }
        /// <summary> Calculates a file hash using <see cref="SHA1"/> </summary>
        public async ValueTask<string> HashAsync_SHA1()
        {
            using SHA1 hasher = SHA1.Create();

            return await hasher.HashAsync(data)
                               .ConfigureAwait(false);
        }
        /// <summary> Calculates a file hash using <see cref="SHA256"/> </summary>
        public async ValueTask<string> HashAsync_SHA256()
        {
            using SHA256 hasher = SHA256.Create();

            return await hasher.HashAsync(data)
                               .ConfigureAwait(false);
        }
        /// <summary> Calculates a file hash using <see cref="SHA384"/> </summary>
        public async ValueTask<string> HashAsync_SHA384()
        {
            using SHA384 hasher = SHA384.Create();

            return await hasher.HashAsync(data)
                               .ConfigureAwait(false);
        }
        /// <summary> Calculates a file hash using <see cref="SHA512"/> </summary>
        public async ValueTask<string> HashAsync_SHA512()
        {
            using SHA512 hasher = SHA512.Create();

            return await hasher.HashAsync(data)
                               .ConfigureAwait(false);
        }
        public async ValueTask<string> HashAsync( HashAlgorithm hasher )
        {
            await using MemoryStream stream = new(data);

            byte[] hash = await hasher.ComputeHashAsync(stream)
                                      .ConfigureAwait(false);

            return BitConverter.ToString(hash);
        }
    }



    extension( string value )
    {
        public OneOf<byte[], string> TryGetData()
        {
            try { return Convert.FromBase64String(value); }
            catch ( FormatException ) { return value; }
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
}
