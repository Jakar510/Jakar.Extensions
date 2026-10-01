// Jakar.Extensions :: Jakar.Extensions
// 06/07/2022  3:25 PM


namespace Jakar.Extensions;


/// <summary>
///     <para> A stack-only string builder, based on System.Text.ValueStringBuilder. </para>
///     <para>
///         Nothing is allocated on the GC heap except the final <see cref="ToString()"/>: start from a caller buffer (<c>new ValueStringBuilder(stackalloc char[256])</c>) or from a rented
///         <see cref="ArrayPool{T}"/> array; growing rents a larger array and returns the previous one. Values are formatted in place through <see cref="ISpanFormattable"/>.
///     </para>
///     <para> Builder methods return <see langword="ref"/> this builder, so chained calls (<c>sb.Append(a).Append(b)</c>) all apply to the same instance. </para>
/// </summary>
/// <remarks> <see cref="ToString()"/> and <see cref="TryCopyTo"/> dispose the builder; <see cref="Dispose"/> is idempotent, so a <see langword="using"/> declaration remains safe. </remarks>
public ref struct ValueStringBuilder : ISpanFormattable, IDisposable
{
    private const int        MAX_FORMAT_GROWTH = 1 << 24; // stop growing for a value that refuses to format after this many chars and fall back to ToString
    private       char[]?    __arrayToReturnToPool;
    private       Span<char> __chars;
    private       int        __length;


    public readonly bool       IsEmpty  { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __length == 0; }
    public readonly int        Capacity { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __chars.Length; }
    /// <summary> The unused storage after <see cref="Length"/>. </summary>
    public readonly Span<char> Next     { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __chars[__length..]; }
    /// <summary> The written characters (<see cref="Length"/> chars). </summary>
    public readonly Span<char> Span     { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __chars[..__length]; }
    /// <summary> The whole underlying storage (<see cref="Capacity"/> chars). </summary>
    public readonly Span<char> RawChars { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __chars; }
    public readonly Span<char> this[ Range range ] => Span[range];
    public ref char this[ Index            index ] => ref Span[index];
    public ref char this[ int              index ] => ref Span[index];
    public readonly ReadOnlySpan<char> Result { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __chars[..__length]; }
    public          int                Length { [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => __length; set => __length = Math.Clamp(value, 0, __chars.Length); }
    public readonly ReadOnlySpan<char> Values { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __chars[..__length]; }


    public ValueStringBuilder() : this(DEFAULT_CAPACITY) { }
    /// <summary> Starts with a rented array of at least <paramref name="initialCapacity"/> chars. </summary>
    public ValueStringBuilder( int initialCapacity )
    {
        __arrayToReturnToPool = ArrayPool<char>.Shared.Rent(initialCapacity);
        __chars               = __arrayToReturnToPool;
        __length              = 0;
    }
    /// <summary> Starts with <paramref name="initialBuffer"/> as storage (typically <c>stackalloc char[N]</c>); it is only replaced by a rented array if more room is needed. </summary>
    public ValueStringBuilder( Span<char> initialBuffer )
    {
        __arrayToReturnToPool = null;
        __chars               = initialBuffer;
        __length              = 0;
    }
    /// <summary> Starts with a copy of <paramref name="span"/>. </summary>
    public ValueStringBuilder( params ReadOnlySpan<char> span ) : this(Math.Max(span.Length, DEFAULT_CAPACITY))
    {
        span.CopyTo(__chars);
        __length = span.Length;
    }
    /// <summary> Returns any rented array to the pool and resets the builder. Safe to call more than once. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Dispose()
    {
        char[]? toReturn = __arrayToReturnToPool;
        this = default; // so the pooled array can't be used again through this instance
        if ( toReturn is not null ) { ArrayPool<char>.Shared.Return(toReturn); }
    }


    /// <summary> Clears the content (<see cref="Length"/> becomes 0), keeping the storage. </summary>
    [UnscopedRef] public ref ValueStringBuilder Reset()
    {
        __length = 0;
        return ref this;
    }
    /// <summary> Ensures room for one value of <typeparamref name="TValue"/> (or <paramref name="format"/>'s length, whichever is larger) after <see cref="Length"/>. </summary>
    public void EnsureCapacity<TValue>( ref readonly ReadOnlySpan<char> format ) => EnsureCapacity(__length + Math.Max(format.Length, Sizes.GetBufferSize<TValue>()));
    /// <summary> Ensures <see cref="Capacity"/> is at least <paramref name="capacity"/>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public void EnsureCapacity( int capacity )
    {
        if ( (uint)capacity > (uint)__chars.Length ) { Grow(capacity - __length); }
    }


    /// <summary> Get a pinnable reference to the builder. Does not ensure there is a null char after <see cref="Length"/> . This overload is pattern matched  the C# 7.3+ compiler so you can omit the explicit method call, and write eg "fixed (char* c = builder)" </summary>
    [Pure] public readonly ref char GetPinnableReference() => ref MemoryMarshal.GetReference(__chars);


    /// <summary> Get a pinnable reference to the builder. </summary>
    /// <param name="terminate"> Ensures that the builder has a null char after <see cref="Length"/> </param>
    [Pure] public ref char GetPinnableReference( bool terminate )
    {
        if ( terminate ) { return ref GetPinnableReference('\0'); }

        return ref GetPinnableReference();
    }


    /// <summary> Get a pinnable reference to the builder, with <paramref name="terminate"/> written just after <see cref="Length"/> (the length itself is unchanged). </summary>
    /// <param name="terminate"> The terminator written after <see cref="Length"/> </param>
    [Pure] public ref char GetPinnableReference( char terminate )
    {
        EnsureCapacity(__length + 1);
        __chars[__length] = terminate;

        return ref GetPinnableReference();
    }


    [Pure] public readonly ReadOnlySpan<char> AsSpan()                       => __chars[..__length];
    [Pure] public readonly ReadOnlySpan<char> Slice( int start )             => __chars[start..__length];
    [Pure] public readonly ReadOnlySpan<char> Slice( int start, int length ) => __chars[..__length].Slice(start, length);


    /// <summary> Copies the content to <paramref name="destination"/> and disposes the builder. </summary>
    public bool TryCopyTo( scoped ref Span<char> destination, out int charsWritten )
    {
        bool copied = Values.TryCopyTo(destination);

        charsWritten = copied
                           ? __length
                           : 0;

        Dispose();
        return copied;
    }


    // ─── Trim ────────────────────────────────────────────────────────────────

    [UnscopedRef] public ref ValueStringBuilder Trim( char                      value ) => ref TrimEnd(value).TrimStart(value);
    [UnscopedRef] public ref ValueStringBuilder Trim( params ReadOnlySpan<char> value ) => ref TrimEnd(value).TrimStart(value);
    [UnscopedRef] public ref ValueStringBuilder TrimEnd( char value )
    {
        __length = MemoryExtensions.TrimEnd(Values, value).Length;
        return ref this;
    }
    /// <remarks> An empty <paramref name="value"/> trims nothing. </remarks>
    [UnscopedRef] public ref ValueStringBuilder TrimEnd( params ReadOnlySpan<char> value )
    {
        if ( !value.IsEmpty ) { __length = MemoryExtensions.TrimEnd(Values, value).Length; }

        return ref this;
    }
    [UnscopedRef] public ref ValueStringBuilder TrimStart( char value )
    {
        RemoveStart(__length - MemoryExtensions.TrimStart(Values, value).Length);
        return ref this;
    }
    /// <remarks> An empty <paramref name="value"/> trims nothing. </remarks>
    [UnscopedRef] public ref ValueStringBuilder TrimStart( params ReadOnlySpan<char> value )
    {
        if ( !value.IsEmpty ) { RemoveStart(__length - MemoryExtensions.TrimStart(Values, value).Length); }

        return ref this;
    }
    private void RemoveStart( int count )
    {
        if ( count <= 0 ) { return; }

        __chars[count..__length].CopyTo(__chars);
        __length -= count;
    }


    // ─── Replace / Insert ────────────────────────────────────────────────────

    /// <summary> Overwrites the char at <paramref name="index"/> (within <see cref="Length"/>). </summary>
    [UnscopedRef] public ref ValueStringBuilder Replace( int index, char value )
    {
        Span[index] = value;
        return ref this;
    }
    /// <summary> Overwrites <paramref name="count"/> chars from <paramref name="index"/> (within <see cref="Length"/>). </summary>
    [UnscopedRef] public ref ValueStringBuilder Replace( int index, char value, int count )
    {
        Span.Slice(index, count).Fill(value);
        return ref this;
    }
    /// <summary> Overwrites chars from <paramref name="index"/> with <paramref name="value"/> (within <see cref="Length"/>). </summary>
    [UnscopedRef] public ref ValueStringBuilder Replace( int index, params ReadOnlySpan<char> value )
    {
        value.CopyTo(Span[index..]);
        return ref this;
    }


    [UnscopedRef] public ref ValueStringBuilder Insert( int index, char value ) => ref Insert(index, value, 1);
    /// <summary> Inserts <paramref name="count"/> copies of <paramref name="value"/> at <paramref name="index"/> (0..<see cref="Length"/>). </summary>
    [UnscopedRef] public ref ValueStringBuilder Insert( int index, char value, int count )
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)index, (uint)__length, nameof(index));
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        if ( __length > __chars.Length - count ) { Grow(count); }

        __chars[index..__length].CopyTo(__chars[( index + count )..]);
        __chars.Slice(index, count).Fill(value);
        __length += count;
        return ref this;
    }
    /// <summary> Inserts <paramref name="value"/> at <paramref name="index"/> (0..<see cref="Length"/>). </summary>
    [UnscopedRef] public ref ValueStringBuilder Insert( int index, params ReadOnlySpan<char> value )
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)index, (uint)__length, nameof(index));

        int count = value.Length;
        if ( __length > __chars.Length - count ) { Grow(count); }

        __chars[index..__length].CopyTo(__chars[( index + count )..]);
        value.CopyTo(__chars[index..]);
        __length += count;
        return ref this;
    }


    // ─── Append ──────────────────────────────────────────────────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining)] [UnscopedRef] public ref ValueStringBuilder Append( char c )
    {
        int        pos   = __length;
        Span<char> chars = __chars;

        if ( (uint)pos < (uint)chars.Length )
        {
            chars[pos] = c;
            __length   = pos + 1;
        }
        else { GrowAndAppend(c); }

        return ref this;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [UnscopedRef] public ref ValueStringBuilder Append( string? value )
    {
        if ( value is null ) { return ref this; }

        int pos = __length;

        // Very common case: single-char strings (separators, symbols).
        if ( value.Length == 1 && (uint)pos < (uint)__chars.Length )
        {
            __chars[pos] = value[0];
            __length     = pos + 1;
            return ref this;
        }

        return ref Append(value.AsSpan());
    }
    [UnscopedRef] public ref ValueStringBuilder Append( IEnumerable<string> values )
    {
        foreach ( string value in values ) { Append(value); }

        return ref this;
    }
    [UnscopedRef] public ref ValueStringBuilder Append( params ReadOnlySpan<string> values )
    {
        int total = 0;
        foreach ( string value in values ) { total += value.Length; }

        EnsureCapacity(__length + total);
        foreach ( string value in values ) { AppendUnchecked(value); }

        return ref this;
    }
    [UnscopedRef] public ref ValueStringBuilder Append( char c, int count )
    {
        if ( count <= 0 ) { return ref this; }

        if ( __length > __chars.Length - count ) { Grow(count); }

        __chars.Slice(__length, count).Fill(c);
        __length += count;
        return ref this;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] [UnscopedRef] public ref ValueStringBuilder Append( scoped ReadOnlySpan<char> value )
    {
        int pos = __length;
        if ( pos > __chars.Length - value.Length ) { Grow(value.Length); }

        value.CopyTo(__chars[pos..]);
        __length = pos + value.Length;
        return ref this;
    }


    // ─── AppendFormat ────────────────────────────────────────────────────────

    [UnscopedRef] public ref ValueStringBuilder AppendFormat<TValue>( ReadOnlySpan<char> format, TValue arg0, IFormatProvider? provider = null )
        where TValue : ISpanFormattable
    {
        AppendFormatHelper(provider, format, arg0);
        return ref this;
    }
    [UnscopedRef] public ref ValueStringBuilder AppendFormat<TValue>( ReadOnlySpan<char> format, TValue arg0, TValue arg1, IFormatProvider? provider = null )
        where TValue : ISpanFormattable
    {
        AppendFormatHelper(provider, format, arg0, arg1);
        return ref this;
    }
    [UnscopedRef] public ref ValueStringBuilder AppendFormat<TValue>( ReadOnlySpan<char> format, TValue arg0, TValue arg1, TValue arg2, IFormatProvider? provider = null )
        where TValue : ISpanFormattable
    {
        AppendFormatHelper(provider, format, arg0, arg1, arg2);
        return ref this;
    }
    [UnscopedRef] public ref ValueStringBuilder AppendFormat<TValue>( ReadOnlySpan<char> format, IFormatProvider? provider, params ReadOnlySpan<TValue> args )
        where TValue : ISpanFormattable
    {
        if ( args.IsEmpty )
        {
            // To preserve the original exception behavior, throw an exception about format if both args and format are null. The actual null check for format is  AppendFormatHelper.
            string paramName = format.IsEmpty
                                   ? nameof(format)
                                   : nameof(args);

            throw new ArgumentNullException(paramName);
        }

        AppendFormatHelper(provider, format, args);
        return ref this;
    }


    // ─── AppendJoin ──────────────────────────────────────────────────────────

    [RequiresDynamicCode("Jakar.Extensions.ArrayExtensions.ArrayAccessor<TElement>.GetCollectionGetter()")] [UnscopedRef]
    public ref ValueStringBuilder AppendJoin( char separator, IEnumerable<string> enumerable )
    {
        ReadOnlySpan<string> span = enumerable.GetInternalArray();
        return ref AppendJoin(separator, span);
    }
    [RequiresDynamicCode("Jakar.Extensions.ArrayExtensions.ArrayAccessor<TElement>.GetCollectionGetter()")] [UnscopedRef]
    public ref ValueStringBuilder AppendJoin( ReadOnlySpan<char> separator, IEnumerable<string> enumerable )
    {
        ReadOnlySpan<string> span = enumerable.GetInternalArray();
        return ref AppendJoin(separator, span);
    }


    [UnscopedRef] public ref ValueStringBuilder AppendJoin( char separator, params ReadOnlySpan<string> span )
    {
        if ( span.IsEmpty ) { return ref this; }

        int total = span.Length - 1;
        foreach ( string? value in span ) { total += value?.Length ?? 0; }

        EnsureCapacity(__length + total);
        AppendUnchecked(span[0]);

        for ( int i = 1; i < span.Length; i++ )
        {
            __chars[__length++] = separator;
            AppendUnchecked(span[i]);
        }

        return ref this;
    }
    [UnscopedRef] public ref ValueStringBuilder AppendJoin( ReadOnlySpan<char> separator, params ReadOnlySpan<string> span )
    {
        if ( span.IsEmpty ) { return ref this; }

        int total = separator.Length * ( span.Length - 1 );
        foreach ( string value in span ) { total += value.Length; }

        EnsureCapacity(__length + total);
        AppendUnchecked(span[0]);

        for ( int i = 1; i < span.Length; i++ )
        {
            separator.CopyTo(__chars[__length..]);
            __length += separator.Length;
            AppendUnchecked(span[i]);
        }

        return ref this;
    }


    [UnscopedRef] public ref ValueStringBuilder AppendJoin<TValue>( char separator, ReadOnlySpan<TValue> enumerable, ReadOnlySpan<char> format = default, IFormatProvider? provider = null )
        where TValue : ISpanFormattable
    {
        for ( int i = 0; i < enumerable.Length; i++ )
        {
            if ( i > 0 ) { Append(separator); }

            AppendFormatted(enumerable[i], format, provider);
        }

        return ref this;
    }
    [UnscopedRef] public ref ValueStringBuilder AppendJoin<TValue>( ReadOnlySpan<char> separator, ReadOnlySpan<TValue> enumerable, ReadOnlySpan<char> format = default, IFormatProvider? provider = null )
        where TValue : ISpanFormattable
    {
        for ( int i = 0; i < enumerable.Length; i++ )
        {
            if ( i > 0 ) { Append(separator); }

            AppendFormatted(enumerable[i], format, provider);
        }

        return ref this;
    }
    [UnscopedRef] public ref ValueStringBuilder AppendJoin<TValue>( char separator, IEnumerable<TValue> enumerable, ReadOnlySpan<char> format = default, IFormatProvider? provider = null )
        where TValue : ISpanFormattable
    {
        bool first = true;

        foreach ( TValue value in enumerable )
        {
            if ( !first ) { Append(separator); }

            first = false;
            AppendFormatted(value, format, provider);
        }

        return ref this;
    }
    [UnscopedRef] public ref ValueStringBuilder AppendJoin<TValue>( ReadOnlySpan<char> separator, IEnumerable<TValue> enumerable, ReadOnlySpan<char> format = default, IFormatProvider? provider = null )
        where TValue : ISpanFormattable
    {
        bool first = true;

        foreach ( TValue value in enumerable )
        {
            if ( !first ) { Append(separator); }

            first = false;
            AppendFormatted(value, format, provider);
        }

        return ref this;
    }


    /// <summary> Formats <paramref name="value"/> directly into the builder, growing as needed (no intermediate string). </summary>
    [UnscopedRef] public ref ValueStringBuilder AppendSpanFormattable<TValue>( TValue value, ReadOnlySpan<char> format, IFormatProvider? provider = null )
        where TValue : ISpanFormattable
    {
        AppendFormatted(value, format, provider);
        return ref this;
    }


    /// <summary> Composite formatting (<c>{index[,alignment][:format]}</c>, <c>{{</c>/<c>}}</c> escapes) with the same rules and exceptions as <see cref="string.Format(IFormatProvider, string, object[])"/>. </summary>
    /// <remarks> Literal text is copied in runs, and arguments are formatted (and padded) in place, so nothing is allocated unless <paramref name="provider"/> supplies an <see cref="ICustomFormatter"/>. </remarks>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="FormatException"> </exception>
    internal void AppendFormatHelper<TValue>( IFormatProvider? provider, ReadOnlySpan<char> formatSpan, params ReadOnlySpan<TValue> args )
        where TValue : ISpanFormattable
    {
        // Undocumented exclusive limits on the range for Argument Hole Count and Argument Hole Alignment.
        const int INDEX_LIMIT = 1000000; // Note:            0 <= ArgIndex < IndexLimit
        const int WIDTH_LIMIT = 1000000; // Note:  -WidthLimit <  ArgAlign < WidthLimit

        if ( formatSpan.IsEmpty ) { throw new ArgumentNullException(nameof(formatSpan)); }

        EnsureCapacity(__length + formatSpan.Length);

        ICustomFormatter? customFormatter = provider?.GetFormat(typeof(ICustomFormatter)) as ICustomFormatter;
        int               pos             = 0;

        while ( true )
        {
            // Copy literal text up to the next brace in one go.
            int brace = formatSpan[pos..].IndexOfAny('{', '}');

            if ( brace < 0 )
            {
                Append(formatSpan[pos..]);
                return;
            }

            Append(formatSpan.Slice(pos, brace));
            pos += brace;

            char ch = formatSpan[pos];

            // Escaped "{{" or "}}".
            if ( pos + 1 < formatSpan.Length && formatSpan[pos + 1] == ch )
            {
                Append(ch);
                pos += 2;
                continue;
            }

            // A lone closing brace is an error.
            if ( ch == '}' ) { ThrowFormatError(); }

            //
            // Start of parsing of Argument Hole.
            // Argument Hole ::= { Count (, WS* Alignment WS*)? (: Formatting)? }
            //
            pos++;

            // If reached end of text then error (Unexpected end of text)
            // or character is not a digit then error (Unexpected Character)
            if ( pos == formatSpan.Length || ( ch = formatSpan[pos] ) < '0' || ch > '9' ) { ThrowFormatError(); }

            int index = 0;

            do
            {
                index = index * 10 + ch - '0';
                if ( ++pos == formatSpan.Length ) { ThrowFormatError(); } // If reached end of text then error (Unexpected end of text)

                ch = formatSpan[pos]; // so long as character is digit and value of the index is less than 1000000 ( index limit )
            }
            while ( ch is >= '0' and <= '9' && index < INDEX_LIMIT );

            // If value of index is not within the range of the arguments passed  then error (Count out of range)
            if ( index >= args.Length ) { throw new FormatException("Format Count Out Of Range"); }

            // Consume optional whitespace.
            while ( pos < formatSpan.Length && ( ch = formatSpan[pos] ) == ' ' ) { pos++; }

            //
            //  Start of parsing of optional Alignment
            //  Alignment ::= comma WS* minus? ('0'-'9')+ WS*
            //
            bool leftJustify = false;
            int  width       = 0;

            if ( ch == ',' )
            {
                pos++;

                // Consume Optional whitespace
                while ( pos < formatSpan.Length && formatSpan[pos] == ' ' ) { pos++; }

                // If reached the end of the text then error (Unexpected end of text)
                if ( pos == formatSpan.Length ) { ThrowFormatError(); }

                ch = formatSpan[pos];

                if ( ch == '-' )
                {
                    leftJustify = true;
                    pos++;

                    if ( pos == formatSpan.Length ) { ThrowFormatError(); }

                    ch = formatSpan[pos];
                }

                if ( ch < '0' || ch > '9' ) { ThrowFormatError(); }

                do
                {
                    width = width * 10 + ch - '0';
                    pos++;

                    if ( pos == formatSpan.Length ) { ThrowFormatError(); }

                    ch = formatSpan[pos];
                }
                while ( ch is >= '0' and <= '9' && width < WIDTH_LIMIT );
            }

            // Consume optional whitespace
            while ( pos < formatSpan.Length && ( ch = formatSpan[pos] ) == ' ' ) { pos++; }

            //
            // Start of parsing of optional formatting parameter.
            //
            ReadOnlySpan<char> itemFormatSpan = default;

            if ( ch == ':' )
            {
                pos++;
                int startPos = pos;

                while ( true )
                {
                    if ( pos == formatSpan.Length ) { ThrowFormatError(); }

                    ch = formatSpan[pos];

                    if ( ch == '}' ) { break; } // Argument hole closed

                    if ( ch == '{' ) { ThrowFormatError(); } // Braces inside the argument hole are not supported

                    pos++;
                }

                if ( pos > startPos ) { itemFormatSpan = formatSpan.Slice(startPos, pos - startPos); }
            }
            else if ( ch != '}' )
            {
                ThrowFormatError(); // Unexpected character
            }

            pos++;

            // Construct the output for this arg hole.
            TValue arg = args[index];

            if ( customFormatter is not null )
            {
                string? itemFormat = itemFormatSpan.IsEmpty
                                         ? null
                                         : new string(itemFormatSpan);

                string custom = customFormatter.Format(itemFormat, arg, provider);

                if ( !string.IsNullOrWhiteSpace(custom) )
                {
                    AppendPadded(custom, width, leftJustify);
                    continue;
                }
            }

            int start = __length;
            AppendFormatted(arg, itemFormatSpan, provider);
            PadFrom(start, width, leftJustify);
        }
    }


    [DoesNotReturn] private static void ThrowFormatError() => throw new FormatException("Invalid Format String");


    // ─── Output ──────────────────────────────────────────────────────────────

    /// <summary> The content as a string. Does not dispose the builder. </summary>
    public readonly string ToString( string? format, IFormatProvider? formatProvider ) => Values.ToString();
    public readonly bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        if ( Values.TryCopyTo(destination) )
        {
            charsWritten = __length;
            return true;
        }

        charsWritten = 0;
        return false;
    }
    /// <summary> The content as a string; the builder is disposed (its pooled array returned). </summary>
    public override string ToString()
    {
        string result = Values.ToString();
        Dispose();
        return result;
    }


    // ─── Internals ───────────────────────────────────────────────────────────

    /// <summary> Formats <paramref name="value"/> into the free space; if it doesn't fit, grows and retries instead of allocating a string. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] private void AppendFormatted<TValue>( TValue value, ReadOnlySpan<char> format, IFormatProvider? provider )
        where TValue : ISpanFormattable
    {
        if ( value.TryFormat(__chars[__length..], out int charsWritten, format, provider) ) { __length += charsWritten; }
        else { AppendFormattedSlow(value, format, provider); }
    }
    [MethodImpl(MethodImplOptions.NoInlining)] private void AppendFormattedSlow<TValue>( TValue value, ReadOnlySpan<char> format, IFormatProvider? provider )
        where TValue : ISpanFormattable
    {
        while ( __chars.Length - __length < MAX_FORMAT_GROWTH )
        {
            Grow(Math.Max(__chars.Length - __length + 1, 64));

            if ( value.TryFormat(__chars[__length..], out int charsWritten, format, provider) )
            {
                __length += charsWritten;
                return;
            }
        }

        // A formatter that never succeeds into a span: fall back to its string.
        Append(value.ToString(format.IsEmpty
                                  ? null
                                  : new string(format),
                              provider));
    }


    /// <summary> Pads the text written since <paramref name="start"/> to <paramref name="width"/> chars: spaces after it when <paramref name="leftJustify"/>, otherwise before it (shifted in place). </summary>
    private void PadFrom( int start, int width, bool leftJustify )
    {
        int padding = width - ( __length - start );
        if ( padding <= 0 ) { return; }

        if ( leftJustify )
        {
            Append(' ', padding);
            return;
        }

        if ( __length > __chars.Length - padding ) { Grow(padding); }

        __chars[start..__length].CopyTo(__chars[( start + padding )..]);
        __chars.Slice(start, padding).Fill(' ');
        __length += padding;
    }
    private void AppendPadded( string value, int width, bool leftJustify )
    {
        int padding = width - value.Length;
        if ( !leftJustify && padding > 0 ) { Append(' ', padding); }

        Append(value.AsSpan());
        if ( leftJustify && padding > 0 ) { Append(' ', padding); }
    }


    /// <summary> Appends without a capacity check; callers have already ensured room. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] private void AppendUnchecked( string? value )
    {
        if ( value is null ) { return; }

        value.CopyTo(__chars[__length..]);
        __length += value.Length;
    }


    [MethodImpl(MethodImplOptions.NoInlining)] private void GrowAndAppend( char c )
    {
        Grow(1);
        __chars[__length++] = c;
    }


    /// <summary> Resize to at least <see cref="Length"/> + <paramref name="additionalCapacityBeyondPos"/>, doubling when that is larger, copying the content into a newly rented array and returning the previous rented array (if any). </summary>
    [MethodImpl(MethodImplOptions.NoInlining)] private void Grow( int additionalCapacityBeyondPos )
    {
        // Increase to at least the required size, but try to double, bounded by the max array length.
        int newCapacity = (int)Math.Max((uint)( __length + additionalCapacityBeyondPos ), Math.Min((uint)Math.Max(__chars.Length, 16) * 2, (uint)Array.MaxLength));

        // Let Rent throw if the requested capacity is negative (caller bug or overflow).
        char[] poolArray = ArrayPool<char>.Shared.Rent(newCapacity);
        __chars[..__length].CopyTo(poolArray);

        char[]? toReturn                = __arrayToReturnToPool;
        __chars = __arrayToReturnToPool = poolArray;
        if ( toReturn is not null ) { ArrayPool<char>.Shared.Return(toReturn); }
    }
}
