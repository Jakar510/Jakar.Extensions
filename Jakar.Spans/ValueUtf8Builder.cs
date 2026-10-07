// Jakar.Spans
// 10/06/2026

using System.Text.Unicode;



namespace Jakar.Extensions;


/// <summary>
///     <para> A stack-only UTF-8 builder: the <see cref="byte"/> counterpart of <see cref="ValueStringBuilder"/>. </para>
///     <para>
///         Nothing is allocated on the GC heap except <see cref="ToArray"/> / <see cref="ToString()"/>: start from a caller buffer (<c>new ValueUtf8Builder(stackalloc byte[256])</c>) or from a rented
///         <see cref="ArrayPool{T}"/> array; growing rents a larger array and returns the previous one. Values are formatted in place through <see cref="IUtf8SpanFormattable"/>, and UTF-16 text is transcoded in place through <see cref="Utf8.FromUtf16"/>.
///     </para>
///     <para> Builder methods return <see langword="ref"/> this builder, so chained calls (<c>sb.Append("a"u8).Append('b')</c>) all apply to the same instance. </para>
/// </summary>
/// <remarks>
///     <see cref="ToArray"/>, <see cref="ToString()"/> and <see cref="TryCopyTo"/> dispose the builder; <see cref="Dispose"/> is idempotent, so a <see langword="using"/> declaration remains safe.
///     <para> Invalid UTF-16 (a lone surrogate) is written as U+FFFD (<c>EF BF BD</c>); a surrogate pair split across two <see cref="Append(char)"/> calls is therefore two replacement characters: append pairs as a span. </para>
/// </remarks>
public ref struct ValueUtf8Builder : IUtf8SpanFormattable, ISpanFormattable, IDisposable
{
    private const int        MAX_FORMAT_GROWTH = 1 << 24; // stop growing for a value that refuses to format after this many bytes
    private const int        MAX_UTF8_PER_CHAR = 3;       // a UTF-16 char never needs more than 3 UTF-8 bytes (a surrogate pair: 4 bytes for 2 chars)
    private       byte[]?    __arrayToReturnToPool;
    private       Span<byte> __bytes;
    private       int        __length;


    public readonly bool       IsEmpty  { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __length == 0; }
    public readonly int        Capacity { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __bytes.Length; }
    /// <summary> The unused storage after <see cref="Length"/>. </summary>
    public readonly Span<byte> Next     { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __bytes[__length..]; }
    /// <summary> The written bytes (<see cref="Length"/> bytes). </summary>
    public readonly Span<byte> Span     { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __bytes[..__length]; }
    /// <summary> The whole underlying storage (<see cref="Capacity"/> bytes). </summary>
    public readonly Span<byte> RawBytes { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __bytes; }
    public readonly Span<byte> this[ Range range ] => Span[range];
    public ref byte this[ Index            index ] => ref Span[index];
    public ref byte this[ int              index ] => ref Span[index];
    public readonly ReadOnlySpan<byte> Result { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __bytes[..__length]; }
    public          int                Length { [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => __length; set => __length = Math.Clamp(value, 0, __bytes.Length); }
    public readonly ReadOnlySpan<byte> Values { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __bytes[..__length]; }


    public ValueUtf8Builder() : this(DEFAULT_CAPACITY) { }
    /// <summary> Starts with a rented array of at least <paramref name="initialCapacity"/> bytes. </summary>
    public ValueUtf8Builder( int initialCapacity )
    {
        __arrayToReturnToPool = ArrayPool<byte>.Shared.Rent(initialCapacity);
        __bytes               = __arrayToReturnToPool;
        __length              = 0;
    }
    /// <summary> Starts with <paramref name="initialBuffer"/> as storage (typically <c>stackalloc byte[N]</c>); it is only replaced by a rented array if more room is needed. </summary>
    public ValueUtf8Builder( Span<byte> initialBuffer )
    {
        __arrayToReturnToPool = null;
        __bytes               = initialBuffer;
        __length              = 0;
    }
    /// <summary> Starts with a copy of <paramref name="span"/>. </summary>
    public ValueUtf8Builder( params ReadOnlySpan<byte> span ) : this(Math.Max(span.Length, DEFAULT_CAPACITY))
    {
        span.CopyTo(__bytes);
        __length = span.Length;
    }
    /// <summary> Returns any rented array to the pool and resets the builder. Safe to call more than once. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Dispose()
    {
        byte[]? toReturn = __arrayToReturnToPool;
        this = default; // so the pooled array can't be used again through this instance
        if ( toReturn is not null ) { ArrayPool<byte>.Shared.Return(toReturn); }
    }


    /// <summary> Clears the content (<see cref="Length"/> becomes 0), keeping the storage. </summary>
    [UnscopedRef] public ref ValueUtf8Builder Reset()
    {
        __length = 0;
        return ref this;
    }
    /// <summary> Ensures room for one value of <typeparamref name="TValue"/> (or <paramref name="format"/>'s length, whichever is larger) after <see cref="Length"/>. </summary>
    public void EnsureCapacity<TValue>( ref readonly ReadOnlySpan<char> format ) => EnsureCapacity(__length + Math.Max(format.Length, Sizes.GetBufferSize<TValue>()));
    /// <summary> Ensures <see cref="Capacity"/> is at least <paramref name="capacity"/>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public void EnsureCapacity( int capacity )
    {
        if ( (uint)capacity > (uint)__bytes.Length ) { Grow(capacity - __length); }
    }


    /// <summary> Get a pinnable reference to the builder. Does not ensure there is a null byte after <see cref="Length"/>. This overload is pattern matched by the C# 7.3+ compiler so you can omit the explicit method call, and write eg "fixed (byte* b = builder)" </summary>
    [Pure] public readonly ref byte GetPinnableReference() => ref MemoryMarshal.GetReference(__bytes);


    /// <summary> Get a pinnable reference to the builder. </summary>
    /// <param name="terminate"> Ensures that the builder has a null byte after <see cref="Length"/> </param>
    [Pure] public ref byte GetPinnableReference( bool terminate )
    {
        if ( terminate ) { return ref GetPinnableReference(0); }

        return ref GetPinnableReference();
    }


    /// <summary> Get a pinnable reference to the builder, with <paramref name="terminate"/> written just after <see cref="Length"/> (the length itself is unchanged). </summary>
    /// <param name="terminate"> The terminator written after <see cref="Length"/> </param>
    [Pure] public ref byte GetPinnableReference( byte terminate )
    {
        EnsureCapacity(__length + 1);
        __bytes[__length] = terminate;

        return ref GetPinnableReference();
    }


    [Pure] public readonly ReadOnlySpan<byte> AsSpan()                       => __bytes[..__length];
    [Pure] public readonly ReadOnlySpan<byte> Slice( int start )             => __bytes[start..__length];
    [Pure] public readonly ReadOnlySpan<byte> Slice( int start, int length ) => __bytes[..__length].Slice(start, length);


    /// <summary> Copies the content to <paramref name="destination"/> and disposes the builder. </summary>
    public bool TryCopyTo( scoped ref Span<byte> destination, out int bytesWritten )
    {
        bool copied = Values.TryCopyTo(destination);

        bytesWritten = copied
                           ? __length
                           : 0;

        Dispose();
        return copied;
    }


    // ─── Trim ────────────────────────────────────────────────────────────────

    [UnscopedRef] public ref ValueUtf8Builder Trim( byte                      value ) => ref TrimEnd(value).TrimStart(value);
    [UnscopedRef] public ref ValueUtf8Builder Trim( params ReadOnlySpan<byte> value ) => ref TrimEnd(value).TrimStart(value);
    [UnscopedRef] public ref ValueUtf8Builder TrimEnd( byte value )
    {
        __length = MemoryExtensions.TrimEnd(Values, value).Length;
        return ref this;
    }
    /// <remarks> An empty <paramref name="value"/> trims nothing. </remarks>
    [UnscopedRef] public ref ValueUtf8Builder TrimEnd( params ReadOnlySpan<byte> value )
    {
        if ( !value.IsEmpty ) { __length = MemoryExtensions.TrimEnd(Values, value).Length; }

        return ref this;
    }
    [UnscopedRef] public ref ValueUtf8Builder TrimStart( byte value )
    {
        RemoveStart(__length - MemoryExtensions.TrimStart(Values, value).Length);
        return ref this;
    }
    /// <remarks> An empty <paramref name="value"/> trims nothing. </remarks>
    [UnscopedRef] public ref ValueUtf8Builder TrimStart( params ReadOnlySpan<byte> value )
    {
        if ( !value.IsEmpty ) { RemoveStart(__length - MemoryExtensions.TrimStart(Values, value).Length); }

        return ref this;
    }
    private void RemoveStart( int count )
    {
        if ( count <= 0 ) { return; }

        __bytes[count..__length].CopyTo(__bytes);
        __length -= count;
    }


    // ─── Replace / Insert ────────────────────────────────────────────────────

    /// <summary> Overwrites the byte at <paramref name="index"/> (within <see cref="Length"/>). </summary>
    [UnscopedRef] public ref ValueUtf8Builder Replace( int index, byte value )
    {
        Span[index] = value;
        return ref this;
    }
    /// <summary> Overwrites <paramref name="count"/> bytes from <paramref name="index"/> (within <see cref="Length"/>). </summary>
    [UnscopedRef] public ref ValueUtf8Builder Replace( int index, byte value, int count )
    {
        Span.Slice(index, count).Fill(value);
        return ref this;
    }
    /// <summary> Overwrites bytes from <paramref name="index"/> with <paramref name="value"/> (within <see cref="Length"/>). </summary>
    [UnscopedRef] public ref ValueUtf8Builder Replace( int index, params ReadOnlySpan<byte> value )
    {
        value.CopyTo(Span[index..]);
        return ref this;
    }


    [UnscopedRef] public ref ValueUtf8Builder Insert( int index, byte value ) => ref Insert(index, value, 1);
    /// <summary> Inserts <paramref name="count"/> copies of <paramref name="value"/> at <paramref name="index"/> (0..<see cref="Length"/>). </summary>
    [UnscopedRef] public ref ValueUtf8Builder Insert( int index, byte value, int count )
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)index, (uint)__length, nameof(index));
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        if ( __length > __bytes.Length - count ) { Grow(count); }

        __bytes[index..__length].CopyTo(__bytes[( index + count )..]);
        __bytes.Slice(index, count).Fill(value);
        __length += count;
        return ref this;
    }
    /// <summary> Inserts <paramref name="value"/> at <paramref name="index"/> (0..<see cref="Length"/>). </summary>
    [UnscopedRef] public ref ValueUtf8Builder Insert( int index, params ReadOnlySpan<byte> value )
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)index, (uint)__length, nameof(index));

        int count = value.Length;
        if ( __length > __bytes.Length - count ) { Grow(count); }

        __bytes[index..__length].CopyTo(__bytes[( index + count )..]);
        value.CopyTo(__bytes[index..]);
        __length += count;
        return ref this;
    }


    // ─── Append: UTF-8 ───────────────────────────────────────────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining)] [UnscopedRef] public ref ValueUtf8Builder Append( byte value )
    {
        int        pos   = __length;
        Span<byte> bytes = __bytes;

        if ( (uint)pos < (uint)bytes.Length )
        {
            bytes[pos] = value;
            __length   = pos + 1;
        }
        else { GrowAndAppend(value); }

        return ref this;
    }
    [UnscopedRef] public ref ValueUtf8Builder Append( byte value, int count )
    {
        if ( count <= 0 ) { return ref this; }

        if ( __length > __bytes.Length - count ) { Grow(count); }

        __bytes.Slice(__length, count).Fill(value);
        __length += count;
        return ref this;
    }
    /// <summary> Appends UTF-8 bytes as is (e.g. a <c>"..."u8</c> literal); they aren't validated. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [UnscopedRef] public ref ValueUtf8Builder Append( scoped ReadOnlySpan<byte> value )
    {
        int pos = __length;
        if ( pos > __bytes.Length - value.Length ) { Grow(value.Length); }

        value.CopyTo(__bytes[pos..]);
        __length = pos + value.Length;
        return ref this;
    }


    // ─── Append: UTF-16 (transcoded) ─────────────────────────────────────────

    /// <summary> Appends <paramref name="value"/> encoded as UTF-8. A lone surrogate is written as U+FFFD; append surrogate pairs with <see cref="Append(ReadOnlySpan{char})"/>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [UnscopedRef] public ref ValueUtf8Builder Append( char value )
    {
        if ( char.IsAscii(value) ) { return ref Append((byte)value); }

        return ref AppendNonAscii(value);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [UnscopedRef] public ref ValueUtf8Builder Append( string? value )
    {
        if ( value is null ) { return ref this; }

        return ref Append(value.AsSpan());
    }
    /// <summary> Appends <paramref name="value"/> encoded as UTF-8 (vectorized through <see cref="Utf8.FromUtf16"/>). Lone surrogates are written as U+FFFD. </summary>
    [UnscopedRef] public ref ValueUtf8Builder Append( scoped ReadOnlySpan<char> value )
    {
        // Optimistic: reserve one byte per char (exact for ASCII); grow by the worst case only for what didn't fit.
        if ( __length > __bytes.Length - value.Length ) { Grow(value.Length); }

        while ( true )
        {
            OperationStatus status = Utf8.FromUtf16(value, __bytes[__length..], out int charsRead, out int bytesWritten, replaceInvalidSequences: true, isFinalBlock: true);
            __length += bytesWritten;

            if ( status != OperationStatus.DestinationTooSmall ) { return ref this; }

            value = value[charsRead..];
            Grow(value.Length * MAX_UTF8_PER_CHAR);
        }
    }


    // ─── Append: formatted values ────────────────────────────────────────────

    /// <summary> Formats <paramref name="value"/> directly into the builder as UTF-8, growing as needed (no intermediate string). </summary>
    [UnscopedRef] public ref ValueUtf8Builder AppendUtf8Formattable<TValue>( TValue value, scoped ReadOnlySpan<char> format = default, IFormatProvider? provider = null )
        where TValue : IUtf8SpanFormattable
    {
        AppendFormatted(value, format, provider);
        return ref this;
    }


    [UnscopedRef] public ref ValueUtf8Builder AppendJoin<TValue>( byte separator, ReadOnlySpan<TValue> values, scoped ReadOnlySpan<char> format = default, IFormatProvider? provider = null )
        where TValue : IUtf8SpanFormattable
    {
        for ( int i = 0; i < values.Length; i++ )
        {
            if ( i > 0 ) { Append(separator); }

            AppendFormatted(values[i], format, provider);
        }

        return ref this;
    }
    [UnscopedRef] public ref ValueUtf8Builder AppendJoin<TValue>( ReadOnlySpan<byte> separator, ReadOnlySpan<TValue> values, scoped ReadOnlySpan<char> format = default, IFormatProvider? provider = null )
        where TValue : IUtf8SpanFormattable
    {
        for ( int i = 0; i < values.Length; i++ )
        {
            if ( i > 0 ) { Append(separator); }

            AppendFormatted(values[i], format, provider);
        }

        return ref this;
    }


    // ─── Output ──────────────────────────────────────────────────────────────

    /// <summary> The content decoded as UTF-16. Does not dispose the builder. </summary>
    public readonly string ToString( string? format, IFormatProvider? formatProvider ) => Encoding.UTF8.GetString(Values);
    /// <summary> Copies the UTF-8 content to <paramref name="utf8Destination"/>. </summary>
    public readonly bool TryFormat( Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        if ( Values.TryCopyTo(utf8Destination) )
        {
            bytesWritten = __length;
            return true;
        }

        bytesWritten = 0;
        return false;
    }
    /// <summary> Decodes the content into <paramref name="destination"/> as UTF-16. </summary>
    public readonly bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        if ( Utf8.ToUtf16(Values, destination, out _, out charsWritten) == OperationStatus.Done ) { return true; }

        charsWritten = 0;
        return false;
    }
    /// <summary> The content as a new array; the builder is disposed (its pooled array returned). </summary>
    public byte[] ToArray()
    {
        byte[] result = [.. Values];
        Dispose();
        return result;
    }
    /// <summary> The content decoded as a string; the builder is disposed (its pooled array returned). </summary>
    public override string ToString()
    {
        string result = Encoding.UTF8.GetString(Values);
        Dispose();
        return result;
    }


    // ─── Internals ───────────────────────────────────────────────────────────

    [MethodImpl(MethodImplOptions.NoInlining)] [UnscopedRef] private ref ValueUtf8Builder AppendNonAscii( char value )
    {
        Rune rune = Rune.TryCreate(value, out Rune result)
                        ? result
                        : Rune.ReplacementChar;

        EnsureCapacity(__length + rune.Utf8SequenceLength);
        __length += rune.EncodeToUtf8(__bytes[__length..]);
        return ref this;
    }


    /// <summary> Formats <paramref name="value"/> into the free space; if it doesn't fit, grows and retries instead of allocating a string. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] private void AppendFormatted<TValue>( TValue value, scoped ReadOnlySpan<char> format, IFormatProvider? provider )
        where TValue : IUtf8SpanFormattable
    {
        if ( value.TryFormat(__bytes[__length..], out int bytesWritten, format, provider) ) { __length += bytesWritten; }
        else { AppendFormattedSlow(value, format, provider); }
    }
    [MethodImpl(MethodImplOptions.NoInlining)] private void AppendFormattedSlow<TValue>( TValue value, scoped ReadOnlySpan<char> format, IFormatProvider? provider )
        where TValue : IUtf8SpanFormattable
    {
        while ( __bytes.Length - __length < MAX_FORMAT_GROWTH )
        {
            Grow(Math.Max(__bytes.Length - __length + 1, 64));

            if ( value.TryFormat(__bytes[__length..], out int bytesWritten, format, provider) )
            {
                __length += bytesWritten;
                return;
            }
        }

        // A formatter that never succeeds into a span: fall back to its string, if it has one.
        if ( value is IFormattable formattable )
        {
            Append(formattable.ToString(format.IsEmpty
                                            ? null
                                            : new string(format),
                                        provider));

            return;
        }

        throw new FormatException($"The value did not format into {MAX_FORMAT_GROWTH} bytes.");
    }


    [MethodImpl(MethodImplOptions.NoInlining)] private void GrowAndAppend( byte value )
    {
        Grow(1);
        __bytes[__length++] = value;
    }


    /// <summary> Resize to at least <see cref="Length"/> + <paramref name="additionalCapacityBeyondPos"/>, doubling when that is larger, copying the content into a newly rented array and returning the previous rented array (if any). </summary>
    [MethodImpl(MethodImplOptions.NoInlining)] private void Grow( int additionalCapacityBeyondPos )
    {
        // Increase to at least the required size, but try to double, bounded by the max array length.
        int newCapacity = (int)Math.Max((uint)( __length + additionalCapacityBeyondPos ), Math.Min((uint)Math.Max(__bytes.Length, 16) * 2, (uint)Array.MaxLength));

        // Let Rent throw if the requested capacity is negative (caller bug or overflow).
        byte[] poolArray = ArrayPool<byte>.Shared.Rent(newCapacity);
        __bytes[..__length].CopyTo(poolArray);

        byte[]? toReturn                = __arrayToReturnToPool;
        __bytes = __arrayToReturnToPool = poolArray;
        if ( toReturn is not null ) { ArrayPool<byte>.Shared.Return(toReturn); }
    }
}
